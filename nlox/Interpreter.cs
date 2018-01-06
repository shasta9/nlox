using System;
using System.Collections.Generic;
using static NLox.TokenType;

namespace NLox {
   internal class Interpreter : Expr.IExprVisitor<object>, Stmt.IStmtVisitor<Nothing> {
      private Environment environment;
      private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

      public Interpreter() {
         Globals = new Environment();
         environment = Globals;
         Globals.Define("clock", new Clock());
      }

      public Environment Globals { get; }

      public void Interpret(List<Stmt> statements) {
         try {
            foreach (var statement in statements) {
               Execute(statement);
            }
         }
         catch (RuntimeError error) {
            nLox.RuntimeError(error);
         }
      }

      private string Stringify(object obj) {
         if (obj == null) return "nil";
         return obj.ToString();
      }

      public object VisitAssignExpr(Expr.Assign expr) {
         object value = Evaluate(expr.Value);
         if (locals.TryGetValue(expr, out int distance)) {
            environment.AssignAt(distance, expr.Name, value);
         }
         else {
            Globals.Assign(expr.Name, value);
         }
         return value;
      }

      public object VisitBinaryExpr(Expr.Binary expr) {
         object left = Evaluate(expr.Left);
         object right = Evaluate(expr.Right);
         switch (expr.Opr.Type) {
            case BANG_EQUAL:
               return !IsEqual(left, right);
            case EQUAL_EQUAL:
               return IsEqual(left, right);
            case GREATER:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left > (double)right;
            case GREATER_EQUAL:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left >= (double)right;
            case LESS:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left < (double)right;
            case LESS_EQUAL:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left <= (double)right;
            case MINUS:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left - (double)right;
            case PLUS:
               if (left is double d1 && right is double d2) {
                  return d1 + d2;
               }
               if (left is string s1 && right is string s2) {
                  return s1 + s2;
               }
               throw new RuntimeError(expr.Opr, "Operands must be two numbers or two strings.");
            case SLASH:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left / (double)right;
            case STAR:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left * (double)right;
         }
         // unreachable
         return null;
      }

      public object VisitCallExpr(Expr.Call expr) {
         object callee = Evaluate(expr.Callee);
         List<object> arguments = new List<object>();
         foreach (var argument in expr.Arguments) {
            arguments.Add(Evaluate(argument));
         }
         if (!(callee is ICallable)) {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
         }
         ICallable function = (ICallable)callee;
         if (arguments.Count != function.Arity()) {
            throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
         }
         return function.Call(this, arguments);
      }

      public object VisitGetExpr(Expr.Get expr) {
         object objekt = Evaluate(expr.Objekt);
         if (objekt is LoxInstance) {
            return ((LoxInstance) objekt).Get(expr.Name);
         }
         throw new RuntimeError(expr.Name,"Only instances have properties.");
      }

      public object VisitGroupingExpr(Expr.Grouping expr) {
         return Evaluate(expr.Xpression);
      }

      public object VisitLiteralExpr(Expr.Literal expr) {
         return expr.Value;
      }

      public object VisitLogicalExpr(Expr.Logical expr) {
         object left = Evaluate(expr.Left);
         if (expr.Opr.Type == OR) {
            if (IsTruthy(left)) return left;
         }
         else {
            if (!IsTruthy(left)) return left;
         }
         return Evaluate(expr.Right);
      }

      public object VisitSetExpr(Expr.Set expr) {
         object objekt = Evaluate(expr.Objekt);
         if (!(objekt is LoxInstance)) {
            throw new RuntimeError(expr.Name, "Only instances have fields.");
         }

         object value = Evaluate(expr.Value);
         ((LoxInstance) objekt).Set(expr.Name, value);
         return value;
      }

      public object VisitThisExpr(Expr.This expr) {
         return LookUpVariable(expr.Keyword, expr);
      }

      public object VisitUnaryExpr(Expr.Unary expr) {
         object right = Evaluate(expr.Right);
         switch (expr.Opr.Type) {
            case BANG:
               return !IsTruthy(right);
            case MINUS:
               CheckNumberOperand(expr.Opr, right);
               return -(double)right;
         }
         // unreachable
         return null;
      }

      public object VisitVariableExpr(Expr.Variable expr) {
         return LookUpVariable(expr.Name, expr);
      }

      private object LookUpVariable(Token name, Expr expr) {
         if (locals.TryGetValue(expr, out int distance)) {
            return environment.GetAt(distance, name.Lexeme);
         }
         return Globals.Get(name);
      }

      private void CheckNumberOperand(Token opr, object operand) {
         if (operand is double) return;
         throw new RuntimeError(opr, "Operand must be a number.");
      }


      private void CheckNumberOperands(Token opr, object left, object right) {
         if (left is double && right is double) return;
         throw new RuntimeError(opr, "Operands must be numbers.");
      }

      private object Evaluate(Expr expr) {
         return expr.Accept(this);
      }

      private void Execute(Stmt stmt) {
         stmt.Accept(this);
      }

      public void Resolve(Expr expr, int depth) {
         locals.Add(expr, depth);
      }

      public Nothing VisitBlockStmt(Stmt.Block stmt) {
         ExecuteBlock(stmt.Statements, new Environment(environment));
         return Nothing.AtAll;
      }

      public void ExecuteBlock(List<Stmt> statements, Environment blockEnvironment) {
         Environment previous = environment;
         try {
            environment = blockEnvironment;
            foreach (var statement in statements) {
               Execute(statement);
            }
         }
         finally {
            environment = previous;
         }
      }

      public Nothing VisitClassStmt(Stmt.Class stmt) {
         environment.Define(stmt.Name.Lexeme, null);
         object superclass = null;
         if (stmt.Superclass != null) {
            superclass = Evaluate(stmt.Superclass);
            if (!(superclass is LoxClass)) {
               throw new RuntimeError(stmt.Name, "Superclass must be a class.");
            }
         }
         Dictionary<string, LoxFunction>methods = new Dictionary<string, LoxFunction>();
         foreach (var method in stmt.Methods) {
            LoxFunction function = new LoxFunction(method, environment, method.Name.Lexeme.Equals("init"));
            methods.Add(method.Name.Lexeme, function);
         }
         LoxClass klass = new LoxClass(stmt.Name.Lexeme, (LoxClass)superclass, methods);
         environment.Assign(stmt.Name, klass);
         return Nothing.AtAll;
      }

      public Nothing VisitExpressionStmt(Stmt.Expression stmt) {
         Evaluate(stmt.Xpression);
         return Nothing.AtAll;
      }

      public Nothing VisitFunctionStmt(Stmt.Function stmt) {
         LoxFunction function = new LoxFunction(stmt, environment, false);
         environment.Define(stmt.Name.Lexeme, function);
         return Nothing.AtAll;
      }

      public Nothing VisitIfStmt(Stmt.If stmt) {
         if (IsTruthy(Evaluate(stmt.Condition))) {
            Execute(stmt.ThenBranch);
         }
         else if (stmt.ElseBranch != null) {
            Execute(stmt.ElseBranch);
         }
         return Nothing.AtAll;
      }

      public Nothing VisitPrintStmt(Stmt.Print stmt) {
         object value = Evaluate(stmt.Xpression);
         Console.WriteLine(Stringify(value));
         return Nothing.AtAll;
      }

      public Nothing VisitReturnStmt(Stmt.Return stmt) {
         object value = null;
         if (stmt.Value != null) value = Evaluate(stmt.Value);
         throw new Return(value);
      }

      public Nothing VisitVarStmt(Stmt.Var stmt) {
         object value = null;
         if (stmt.Initializer != null) {
            value = Evaluate(stmt.Initializer);
         }
         environment.Define(stmt.Name.Lexeme, value);
         return Nothing.AtAll;
      }

      public Nothing VisitWhileStmt(Stmt.While stmt) {
         while (IsTruthy(Evaluate(stmt.Condition))) {
            Execute(stmt.Body);
         }
         return Nothing.AtAll;
      }

      private bool IsTruthy(object obj) {
         if (obj == null) return false;
         if (obj is bool b) return b;
         return true;
      }

      private bool IsEqual(object a, object b) {
         // nil is only equal to nil
         if (a == null && b == null) return true;
         if (a == null) return false;
         return a.Equals(b);
      }
   }
}
