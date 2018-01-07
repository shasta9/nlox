using System.Collections.Generic;

namespace NLox {
   internal class Resolver : Expr.IExprVisitor<Nothing>, Stmt.IStmtVisitor<Nothing> {
      private enum ClassType {
         None,
         Class,
         Subclass
      }
      private enum FunctionType {
         None,
         Function,
         Initializer,
         Method
      }
      private readonly Interpreter interpreter;
      private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
      private ClassType currentClass = ClassType.None;
      private FunctionType currentFunction = FunctionType.None;


      public Resolver(Interpreter interpreter) {
         this.interpreter = interpreter;
      }

      public void Resolve(List<Stmt> statements) {
         foreach (var statement in statements) {
            Resolve(statement);
         }
      }

      private void Resolve(Stmt stmt) {
         stmt.Accept(this);
      }

      private void Resolve(Expr expr) {
         expr.Accept(this);
      }

      private void ResolveFunction(Stmt.Function function, FunctionType type) {
         FunctionType enclosingFunction = currentFunction;
         currentFunction = type;
         BeginScope();
         foreach (var param in function.Parameters) {
            Declare(param);
            Define(param);
         }
         Resolve(function.Body);
         EndScope();
         currentFunction = enclosingFunction;
      }

      private void ResolveLocal(Expr expr, Token name) {
         int depth = 0;
         foreach (var scope in scopes) {
            if (scope.ContainsKey(name.Lexeme)) {
               interpreter.Resolve(expr, depth);
               return;
            }
            depth++;
         }
         // not found, assume global
      }

      private void BeginScope() {
         scopes.Push(new Dictionary<string, bool>());
      }

      private void EndScope() {
         scopes.Pop();
      }

      private void Declare(Token name) {
         if (scopes.Count == 0) return;
         if (scopes.Peek().ContainsKey(name.Lexeme)) {
            nLox.Error(name, "Variable with this name is already declared in this scope.");
         }
         scopes.Peek()[name.Lexeme] = false;
      }

      private void Define(Token name) {
         if (scopes.Count == 0) return;
         scopes.Peek()[name.Lexeme] = true;
      }

      public Nothing VisitAssignExpr(Expr.Assign expr) {
         Resolve(expr.Value);
         ResolveLocal(expr, expr.Name);
         return Nothing.AtAll;
      }

      public Nothing VisitBinaryExpr(Expr.Binary expr) {
         Resolve(expr.Left);
         Resolve(expr.Right);
         return Nothing.AtAll;
      }

      public Nothing VisitCallExpr(Expr.Call expr) {
         Resolve(expr.Callee);
         foreach (var arg in expr.Arguments) {
            Resolve(arg);
         }
         return Nothing.AtAll;
      }

      public Nothing VisitGetExpr(Expr.Get expr) {
         Resolve(expr.Objekt);
         return Nothing.AtAll;
      }

      public Nothing VisitGroupingExpr(Expr.Grouping expr) {
         Resolve(expr.Xpression);
         return Nothing.AtAll;
      }

      public Nothing VisitLiteralExpr(Expr.Literal expr) {
         return Nothing.AtAll;
      }

      public Nothing VisitLogicalExpr(Expr.Logical expr) {
         Resolve(expr.Left);
         Resolve(expr.Right);
         return Nothing.AtAll;
      }

      public Nothing VisitSetExpr(Expr.Set expr) {
         Resolve(expr.Value);
         Resolve(expr.Objekt);
         return Nothing.AtAll;
      }

      public Nothing VisitSuperExpr(Expr.Super expr) {
         if (currentClass == ClassType.None) {
            nLox.Error(expr.Keyword, "Cannot use 'super' outside of a class.");
         }
         else if (currentClass != ClassType.Subclass) {
            nLox.Error(expr.Keyword, "Cannot use 'super' in a class with no superclass.");
         }
         ResolveLocal(expr, expr.Keyword);
         return Nothing.AtAll;
      }

      public Nothing VisitThisExpr(Expr.This expr) {
         if (currentClass == ClassType.None) {
            nLox.Error(expr.Keyword, "Cannot use 'this' outside of a class.");
            return Nothing.AtAll;
         }
         ResolveLocal(expr, expr.Keyword);
         return Nothing.AtAll;
      }

      public Nothing VisitUnaryExpr(Expr.Unary expr) {
         Resolve(expr.Right);
         return Nothing.AtAll;
      }

      public Nothing VisitVariableExpr(Expr.Variable expr) {
         if (scopes.Count != 0 && scopes.Peek().ContainsKey(expr.Name.Lexeme) && scopes.Peek()[expr.Name.Lexeme] == false) {
            nLox.Error(expr.Name, "Cannot read local variable in its own initializer.");
         }
         ResolveLocal(expr, expr.Name);
         return Nothing.AtAll;
      }

      public Nothing VisitBlockStmt(Stmt.Block stmt) {
         BeginScope();
         Resolve(stmt.Statements);
         EndScope();
         return Nothing.AtAll;
      }

      public Nothing VisitClassStmt(Stmt.Class stmt) {
         Declare(stmt.Name);
         Define(stmt.Name);
         ClassType enclosingClass = currentClass;
         currentClass = ClassType.Class;
         if (stmt.Superclass != null) {
            currentClass = ClassType.Subclass;
            Resolve(stmt.Superclass);
            BeginScope();
            scopes.Peek().Add("super", true);
         }
         BeginScope();
         scopes.Peek().Add("this", true);
         foreach (var method in stmt.Methods) {
            FunctionType declaration = FunctionType.Method;
            if (method.Name.Lexeme.Equals("init")) {
               declaration = FunctionType.Initializer;
            }
            ResolveFunction(method, declaration);
         }
         EndScope();
         if (stmt.Superclass != null) EndScope();
         currentClass = enclosingClass;
         return Nothing.AtAll;
      }

      public Nothing VisitExpressionStmt(Stmt.Expression stmt) {
         Resolve(stmt.Xpression);
         return Nothing.AtAll;
      }

      public Nothing VisitFunctionStmt(Stmt.Function stmt) {
         Declare(stmt.Name);
         Define(stmt.Name);
         ResolveFunction(stmt, FunctionType.Function);
         return Nothing.AtAll;
      }

      public Nothing VisitIfStmt(Stmt.If stmt) {
         Resolve(stmt.Condition);
         Resolve(stmt.ThenBranch);
         if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
         return Nothing.AtAll;
      }

      public Nothing VisitPrintStmt(Stmt.Print stmt) {
         Resolve(stmt.Xpression);
         return Nothing.AtAll;
      }

      public Nothing VisitReturnStmt(Stmt.Return stmt) {
         if (currentFunction == FunctionType.None) {
            nLox.Error(stmt.Keyword, "Cannot return from top level code.");
         }
         if (stmt.Value != null) {
            if (currentFunction == FunctionType.Initializer) {
               nLox.Error(stmt.Keyword, "Cannot return a value from an initializer.");
            }
            Resolve(stmt.Value);
         }
         return Nothing.AtAll;
      }

      public Nothing VisitVarStmt(Stmt.Var stmt) {
         Declare(stmt.Name);
         if (stmt.Initializer != null) {
            Resolve(stmt.Initializer);
         }
         Define(stmt.Name);
         return Nothing.AtAll;
      }

      public Nothing VisitWhileStmt(Stmt.While stmt) {
         Resolve(stmt.Condition);
         Resolve(stmt.Body);
         return Nothing.AtAll;
      }
   }
}
