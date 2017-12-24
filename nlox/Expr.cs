namespace NLox {


   internal abstract class Expr {
      
      public interface IExprVisitor {
         void VisitBinaryExpr(Binary expr);
         void VisitGroupingExpr(Grouping expr);
         void VisitLiteralExpr(Literal expr);
         void VisitUnaryExpr(Unary expr);
      }

      public abstract void Accept(Expr.IExprVisitor visitor);

      public class Binary : Expr {
         public Expr Left { get; }
         public Token Opr { get; }
         public Expr Right { get; }
         public Binary(Expr left, Token opr, Expr right) {
            Left = left;
            Opr = opr;
            Right = right;
         }

         public override void Accept(IExprVisitor visitor) {
            visitor.VisitBinaryExpr(this);
         }
      }

      public class Grouping : Expr {
         public Expr Expression { get; }
         public Grouping(Expr expression) {
            Expression = expression;
         }

         public override void Accept(IExprVisitor visitor) {
            visitor.VisitGroupingExpr(this);
         }
      }

      public class Literal : Expr {
         public object Value { get; }
         public Literal(object value) {
            Value = value;
         }

         public override void Accept(IExprVisitor visitor) {
            visitor.VisitLiteralExpr(this);
         }
      }

      public class Unary : Expr {
         public Token Opr { get; }
         public Expr Right { get; }

         public Unary(Token opr, Expr right) {
            Opr = opr;
            Right = right;
         }

         public override void Accept(IExprVisitor visitor) {
            visitor.VisitUnaryExpr(this);
         }
      }
   }
}
