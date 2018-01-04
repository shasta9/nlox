using System.Collections.Generic;

namespace NLox {
   internal class Resolver : Expr.IExprVisitor<Nothing>, Stmt.IStmtVisitor<Nothing> {
      private enum FunctionType {
         None,
         Function
      }
      private readonly Interpreter interpreter;
      private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
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
         scopes.Peek()[name.Lexeme]= false;
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
         throw new System.NotImplementedException();
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
            nLox.Error(stmt.Keyword,"Cannot return from top level code.");
         }
         if (stmt.Value != null) Resolve(stmt.Value);
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
