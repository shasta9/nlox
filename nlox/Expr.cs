namespace Nlox {
   internal abstract class Expr {
   }

   internal class Binary : Expr {
      public Expr left;
      public Token op;
      public Expr right;
      public Binary(Expr left,Token op,Expr right) {
         Left = left;
         Op = op;
         Right = right;
      }
   }

   internal class Grouping : Expr {
      public Expr expression;
      public Grouping(Expr expression) {
         Expression = expression;
      }
   }

   internal class Literal : Expr {
      public Object value;
      public Literal(object value) {
         Value = value;
      }
   }

   internal class Unary : Expr {
      public Token op;
      public Expr right;
      public Unary(Token op,Expr right) {
         Op = op;
         Right = right;
      }
   }
}
