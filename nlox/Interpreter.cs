using System;
using System.Collections.Generic;
using static NLox.TokenType;

namespace NLox {
   internal class Interpreter : Expr.IExprVisitor<object>, Stmt.IStmtVisitor<Nothing> {

      private Environment environment = new Environment();

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
         environment.Assign(expr.Name, value);
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

      public object VisitGroupingExpr(Expr.Grouping expr) {
         return Evaluate(expr.Expression);
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
         return environment.Get(expr.Name);
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

      public Nothing VisitBlockStmt(Stmt.Block stmt) {
         ExecuteBlock(stmt.Statements, new Environment(environment));
         return Nothing.AtAll;
      }

      private void ExecuteBlock(List<Stmt> statements, Environment blockEnvironment) {
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

      public Nothing VisitExpressionStmt(Stmt.Expression stmt) {
         Evaluate(stmt.Xpression);
         return Nothing.AtAll;
      }

      public Nothing VisitIfStmt(Stmt.If stmt) {
         if (IsTruthy(stmt.Condition)) {
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
