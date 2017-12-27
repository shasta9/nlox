namespace NLox {

   internal abstract class Expr {

      public interface IExprVisitor<T> {
         T VisitBinaryExpr(Binary expr);
         T VisitGroupingExpr(Grouping expr);
         T VisitLiteralExpr(Literal expr);
         T VisitUnaryExpr(Unary expr);
         T VisitVariableExpr(Variable expr);
      }

      public abstract T Accept<T>(IExprVisitor<T> visitor);

      public class Binary : Expr {
         public Expr Left { get; }
         public Token Opr { get; }
         public Expr Right { get; }
         public Binary (Expr left, Token opr, Expr right) {
            Left = left;
            Opr = opr;
            Right = right;
         }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitBinaryExpr(this);
         }
      }

      public class Grouping : Expr {
         public Expr Expression { get; }
         public Grouping (Expr expression) {
            Expression = expression;
         }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitGroupingExpr(this);
         }
      }

      public class Literal : Expr {
         public object Value { get; }
         public Literal (object value) {
            Value = value;
         }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitLiteralExpr(this);
         }
      }

      public class Unary : Expr {
         public Token Opr { get; }
         public Expr Right { get; }
         public Unary (Token opr, Expr right) {
            Opr = opr;
            Right = right;
         }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitUnaryExpr(this);
         }
      }

      public class Variable : Expr {
         public Token Name { get; }
         public Variable (Token name) {
            Name = name;
         }

         public override T Accept<T>(IExprVisitor<T> visitor) {
            return visitor.VisitVariableExpr(this);
         }
      }
   }
}
