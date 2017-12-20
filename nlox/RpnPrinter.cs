using System;
using System.Text;

namespace NLox {
   internal class RpnPrinter : IExprVisitor {
      StringBuilder sb = new StringBuilder();

      public void VisitBinaryExpr(Binary expr) {
         expr.Left.Accept(this);
         sb.Append(" ");
         expr.Right.Accept(this);
         sb.Append(" ");
         sb.Append($"{expr.Opr.Lexeme}");
      }

      public void VisitGroupingExpr(Grouping expr) {
         expr.Expression.Accept(this);
      }

      public void VisitLiteralExpr(Literal expr) {
         if (expr.Value == null) {
            sb.Append("nil");
            return;
         }
         sb.Append(expr.Value.ToString());
      }

      public void VisitUnaryExpr(Unary expr) {
         throw new NotImplementedException();
      }

      public override string ToString() {
         return sb.ToString();
      }
   }
}
