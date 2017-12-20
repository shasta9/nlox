namespace NLox {

   internal interface IExprVisitor {
      void VisitBinaryExpr(Binary expr);
      void VisitGroupingExpr(Grouping expr);
      void VisitLiteralExpr(Literal expr);
      void VisitUnaryExpr(Unary expr);
   }

   internal abstract class Expr {
      public abstract void Accept(IExprVisitor visitor);
   }

   internal class Binary : Expr {
      public Expr Left { get; }
      public Token Opr { get; }
      public Expr Right { get; }
      public Binary (Expr left, Token opr, Expr right) {
         Left = left;
         Opr = opr;
         Right = right;
      }

      public override void Accept(IExprVisitor visitor) {
         visitor.VisitBinaryExpr(this);
      }
   }

   internal class Grouping : Expr {
      public Expr Expression { get; }
      public Grouping (Expr expression) {
         Expression = expression;
      }

      public override void Accept(IExprVisitor visitor) {
         visitor.VisitGroupingExpr(this);
      }
   }

   internal class Literal : Expr {
      public object Value { get; }
      public Literal (object value) {
         Value = value;
      }

      public override void Accept(IExprVisitor visitor) {
         visitor.VisitLiteralExpr(this);
      }
   }

   internal class Unary : Expr {
      public Token Opr { get; }
      public Expr Right { get; }
      public Unary (Token opr, Expr right) {
         Opr = opr;
         Right = right;
      }

      public override void Accept(IExprVisitor visitor) {
         visitor.VisitUnaryExpr(this);
      }
   }
}
