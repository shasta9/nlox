using System;
using static NLox.TokenType;

namespace NLox {
   internal class Interpreter : Expr.IExprVisitor<object> {

      public void Interpret(Expr expression) {
         try {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
         }
         catch (RuntimeError error) {
            nLox.RuntimeError(error);
         }
      }

      private string Stringify(object obj) {
         if (obj == null) return "nil";
         return obj.ToString();
      }

      public object VisitBinaryExpr(Expr.Binary expr) {
         object left = Evaluate(expr.Left);
         object right = Evaluate(expr.Right);
         switch (expr.Opr.Type) {
            case BANG_EQUAL:
               return !IsEqual(left, right);
            case EQUAL_EQUAL:
               return IsEqual(left, right);
            case GREATER:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left > (double)right;
            case GREATER_EQUAL:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left >= (double)right;
            case LESS:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left < (double)right;
            case LESS_EQUAL:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left <= (double)right;
            case MINUS:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left - (double)right;
            case PLUS:
               if (left is double d1 && right is double d2) {
                  return d1 + d2;
               }
               if (left is string s1 && right is string s2) {
                  return s1 + s2;
               }
               throw new RuntimeError(expr.Opr, "Operands must be two numbers or two strings.");
            case SLASH:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left / (double)right;
            case STAR:
               CheckNumberOperands(expr.Opr, left, right);
               return (double)left * (double)right;
         }
         // unreachable
         return null;
      }

      private void CheckNumberOperand(Token opr, object operand) {
         if (operand is double) return;
         throw new RuntimeError(opr, "Operand must be a number.");
      }

      private void CheckNumberOperands(Token opr, object left, object right) {
         if (left is double && right is double) return;
         throw new RuntimeError(opr, "Operands must be numbers.");
      }

      public object VisitGroupingExpr(Expr.Grouping expr) {
         return Evaluate(expr.Expression);
      }

      public object VisitLiteralExpr(Expr.Literal expr) {
         return expr.Value;
      }

      public object VisitUnaryExpr(Expr.Unary expr) {
         object right = Evaluate(expr.Right);
         switch (expr.Opr.Type) {
            case BANG:
               return !IsTruthy(right);
            case MINUS:
               CheckNumberOperand(expr.Opr, right);
               return -(double)right;
         }
         // unreachable
         return null;
      }

      private object Evaluate(Expr expr) {
         return expr.Accept(this);
      }

      private bool IsTruthy(object obj) {
         if (obj == null) return false;
         if (obj is bool b) return b;
         return true;
      }

      private bool IsEqual(object a, object b) {
         // nil is only equal to nil
         if (a == null && b == null) return true;
         if (a == null) return false;
         return a.Equals(b);
      }
   }
}
