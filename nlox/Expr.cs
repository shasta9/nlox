namespace NLox {


   internal abstract class Expr {

      public interface IExprVisitor {
         T VisitBinaryExpr<T>(Binary expr);
         T VisitGroupingExpr<T>(Grouping expr);
         T VisitLiteralExpr<T>(Literal expr);
         T VisitUnaryExpr<T>(Unary expr);
      }

      public abstract T Accept<T>(IExprVisitor visitor);

      public class Binary : Expr {
         public Expr Left { get; }
         public Token Opr { get; }
         public Expr Right { get; }
         public Binary(Expr left, Token opr, Expr right) {
            Left = left;
            Opr = opr;
            Right = right;
         }

         public override T Accept<T>(IExprVisitor visitor) {
            return visitor.VisitBinaryExpr<T>(this);
         }
      }

      public class Grouping : Expr {
         public Expr Expression { get; }
         public Grouping(Expr expression) {
            Expression = expression;
         }

         public override T Accept<T>(IExprVisitor visitor) {
            return visitor.VisitGroupingExpr<T>(this);
         }
      }

      public class Literal : Expr {
         public object Value { get; }
         public Literal(object value) {
            Value = value;
         }

         public override T Accept<T>(IExprVisitor visitor) {
            return visitor.VisitLiteralExpr<T>(this);
         }
      }

      public class Unary : Expr {
         public Token Opr { get; }
         public Expr Right { get; }

         public Unary(Token opr, Expr right) {
            Opr = opr;
            Right = right;
         }

         public override T Accept<T>(IExprVisitor visitor) {
            return visitor.VisitUnaryExpr<T>(this);
         }
      }
   }
}
