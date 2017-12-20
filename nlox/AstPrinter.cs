using System.Text;

namespace NLox {
   internal class AstPrinter : IExprVisitor {
      StringBuilder sb = new StringBuilder();

      public void VisitBinaryExpr(Binary expr) {
         sb.Append("(");
         sb.Append($"{expr.Opr.Lexeme}");
         sb.Append(" ");
         expr.Left.Accept(this);
         sb.Append(" ");
         expr.Right.Accept(this);
         sb.Append(")");
      }

      public void VisitGroupingExpr(Grouping expr) {
         sb.Append("(");
         sb.Append("group");
         sb.Append(" ");
         expr.Expression.Accept(this);
         sb.Append(")");
      }

      public void VisitLiteralExpr(Literal expr) {
         if (expr.Value == null) {
            sb.Append("nil");
            return;
         }
         sb.Append(expr.Value.ToString());
      }

      public void VisitUnaryExpr(Unary expr) {
         sb.Append("(");
         sb.Append($"{expr.Opr.Lexeme} ");
         expr.Right.Accept(this);
         sb.Append(")");
      }

      public override string ToString() {
         return sb.ToString();
      }
   }
}
