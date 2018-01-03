using System.Collections.Generic;

namespace NLox {
   internal class Resolver : Expr.IExprVisitor<Nothing>, Stmt.IStmtVisitor<Nothing> {
      private readonly Interpreter interpreter;
      private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

      public Resolver(Interpreter interpreter) {
         this.interpreter = interpreter;
      }

      private void Resolve(List<Stmt> statements) {
         foreach (var statement in statements) {
            Resolve(statement);
         }
      }

      private void Resolve(Stmt stmt) {
         stmt.Accept(this);
      }

      private void BeginScope() {
         scopes.Push(new Dictionary<string, bool>());
      }

      private void EndScope() {
         scopes.Pop();
      }

      private void Declare(Token name) {

      }

      private void Define(Token name) {

      }

      public Nothing VisitAssignExpr(Expr.Assign expr) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitBinaryExpr(Expr.Binary expr) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitCallExpr(Expr.Call expr) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitGroupingExpr(Expr.Grouping expr) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitLiteralExpr(Expr.Literal expr) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitLogicalExpr(Expr.Logical expr) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitUnaryExpr(Expr.Unary expr) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitVariableExpr(Expr.Variable expr) {
         throw new System.NotImplementedException();
      }


      public Nothing VisitBlockStmt(Stmt.Block stmt) {
         BeginScope();
         Resolve(stmt.Statements);
         EndScope();
         return Nothing.AtAll;
      }

      public Nothing VisitExpressionStmt(Stmt.Expression stmt) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitFunctionStmt(Stmt.Function stmt) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitIfStmt(Stmt.If stmt) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitPrintStmt(Stmt.Print stmt) {
         throw new System.NotImplementedException();
      }

      public Nothing VisitReturnStmt(Stmt.Return stmt) {
         throw new System.NotImplementedException();
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
         throw new System.NotImplementedException();
      }
   }
}
