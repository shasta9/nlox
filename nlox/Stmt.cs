namespace NLox {

   internal abstract class Stmt {

      public interface IStmtVisitor<T> {
         T VisitExpressionStmt(Expression stmt);
         T VisitPrintStmt(Print stmt);
      }

      public abstract T Accept<T>(IStmtVisitor<T> visitor);

      public class Expression : Stmt {
         public Expr Xpression { get; }
         public Expression (Expr xpression) {
            Xpression = xpression;
         }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitExpressionStmt(this);
         }
      }

      public class Print : Stmt {
         public Expr Xpression { get; }
         public Print (Expr xpression) {
            Xpression = xpression;
         }

         public override T Accept<T>(IStmtVisitor<T> visitor) {
            return visitor.VisitPrintStmt(this);
         }
      }
   }
}
