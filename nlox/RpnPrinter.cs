using System;
using System.Text;

namespace NLox {
   internal class RpnPrinter : Expr.IExprVisitor {
      StringBuilder sb = new StringBuilder();

      public void VisitBinaryExpr(Expr.Binary expr) {
         expr.Left.Accept(this);
         sb.Append(" ");
         expr.Right.Accept(this);
         sb.Append(" ");
         sb.Append($"{expr.Opr.Lexeme}");
      }

      public void VisitGroupingExpr(Expr.Grouping expr) {
         expr.Expression.Accept(this);
      }

      public void VisitLiteralExpr(Expr.Literal expr) {
         if (expr.Value == null) {
            sb.Append("nil");
            return;
         }
         sb.Append(expr.Value.ToString());
      }

      public void VisitUnaryExpr(Expr.Unary expr) {
         throw new NotImplementedException();
      }

      public override string ToString() {
         return sb.ToString();
      }
   }
}
