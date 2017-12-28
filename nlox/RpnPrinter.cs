﻿using System;
using System.Text;

namespace NLox {
   internal class RpnPrinter : Expr.IExprVisitor<string> {
      public string VisitAssignExpr(Expr.Assign expr) {
         throw new NotImplementedException();
      }

      public string VisitBinaryExpr(Expr.Binary expr) {
         var sb = new StringBuilder();
         sb.Append(expr.Left.Accept(this));
         sb.Append(" ");
         sb.Append(expr.Right.Accept(this));
         sb.Append(" ");
         sb.Append($"{expr.Opr.Lexeme}");
         return sb.ToString();
      }

      public string VisitGroupingExpr(Expr.Grouping expr) {
         return expr.Expression.Accept(this);
      }

      public string VisitLiteralExpr(Expr.Literal expr) {
         var sb = new StringBuilder();
         if (expr.Value == null) {
            sb.Append("nil");
            return sb.ToString();
         }
         sb.Append(expr.Value);
         return sb.ToString();
      }

      public string VisitUnaryExpr(Expr.Unary expr) {
         var sb = new StringBuilder();
         sb.Append(expr.Accept(this));
         sb.Append(" !");
         return sb.ToString();
      }

      public string VisitVariableExpr(Expr.Variable expr) {
         throw new NotImplementedException();
      }
   }
}
