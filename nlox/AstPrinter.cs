using System.Text;

namespace NLox {
   internal class AstPrinter : Expr.IExprVisitor<string> {
      private readonly StringBuilder sb = new StringBuilder();

      public string Print(Expr expr) {
         return expr.Accept(this);
      }

      public string VisitAssignExpr(Expr.Assign expr) {
         throw new System.NotImplementedException();
      }

      public string VisitBinaryExpr(Expr.Binary expr) {
         return Parenthesise(expr.Opr.Lexeme, expr.Left, expr.Right);
      }

      public string VisitGroupingExpr(Expr.Grouping expr) {
         return Parenthesise("group", expr.Expression);
      }

      public string VisitLiteralExpr(Expr.Literal expr) {
         if (expr.Value == null) return "nil";
         return expr.Value.ToString();
      }

      public string VisitLogicalExpr(Expr.Logical expr) {
         throw new System.NotImplementedException();
      }

      public string VisitUnaryExpr(Expr.Unary expr) {
         return Parenthesise(expr.Opr.Lexeme, expr.Right);
      }

      public string VisitVariableExpr(Expr.Variable expr) {
         throw new System.NotImplementedException();
      }

      private string Parenthesise(string name, params Expr[] exprs) {
         var builder = new StringBuilder();
         builder.Append("(").Append(name);
         foreach (var expr in exprs) {
            builder.Append(" ");
            builder.Append(expr.Accept<string>(this));
         }
         builder.Append(")");
         return builder.ToString();
      }
   }
}
