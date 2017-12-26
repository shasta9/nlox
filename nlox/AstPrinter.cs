using System.Text;

namespace NLox {
   internal class AstPrinter : Expr.IExprVisitor<string> {
      private readonly StringBuilder sb = new StringBuilder();



      public void VisitGroupingExpr(Expr.Grouping expr) {
         sb.Append("(");
         sb.Append("group");
         sb.Append(" ");
         expr.Expression.Accept(this);
         sb.Append(")");
      }

      public void VisitLiteralExpr(Expr.Literal expr) {
         if (expr.Value == null) {
            sb.Append("nil");
            return;
         }
         sb.Append(expr.Value.ToString());
      }

      public void VisitUnaryExpr(Expr.Unary expr) {
         sb.Append("(");
         sb.Append($"{expr.Opr.Lexeme} ");
         expr.Right.Accept(this);
         sb.Append(")");
      }

      public override string ToString() {
         return sb.ToString();
      }

      public T VisitBinaryExpr<T>(Expr.Binary expr) {
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
