using System;
using System.Collections.Generic;

namespace NLox {
   internal class Parser {

      private class ParseError : Exception { }

      private readonly List<Token> tokens;
      private int current = 0;

      public Parser(List<Token> tokens) {
         this.tokens = tokens;
      }

      private Expr Expression() {
         return Equality();
      }

      private Expr Equality() {
         Expr expr = Comparison();
         while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
            Token opr = Previous();
            Expr right = Comparison();
            expr = new Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Comparison() {
         Expr expr = Addition();
         while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
            Token opr = Previous();
            Expr right = Addition();
            expr = new Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Addition() {
         Expr expr = Multiplication();
         while (Match(TokenType.MINUS, TokenType.PLUS)) {
            Token opr = Previous();
            Expr right = Multiplication();
            expr = new Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Multiplication() {
         Expr expr = Unary();
         while (Match(TokenType.SLASH, TokenType.STAR)) {
            Token opr = Previous();
            Expr right = Unary();
            expr = new Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Unary() {
         if (Match(TokenType.BANG, TokenType.MINUS)) {
            Token opr = Previous();
            Expr right = Unary();
            return new Unary(opr, right);
         }
         return Primary();
      }

      private Expr Primary() {
         if (Match(TokenType.FALSE)) return new Literal(false);
         if (Match(TokenType.TRUE)) return new Literal(true);
         if (Match(TokenType.NIL)) return new Literal(null);
         if (Match(TokenType.NUMBER, TokenType.STRING)) {
            return new Literal(Previous().Literal);
         }
         if (Match(TokenType.LEFT_PAREN)) {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
         }
         return null;
      }

      private Token Consume(TokenType type, string message) {
         if (Check(type)) return Advance();
         throw Error(Peek(), message);
      }

      private ParseError Error(Token token, String message) {
         nLox.Error(token, message);
         return new ParseError();
      }

      private static void Error(Token token, String message) {
         if (token.type == TokenType.EOF) {
            report(token.line, " at end", message);
         }
         else {
            report(token.line, " at '" + token.lexeme + "'", message);
         }
      }

      private bool Match(params TokenType[] types) {
         foreach (var type in types) {
            if (Check(type)) {
               Advance();
               return true;
            }
         }
         return false;
      }

      private bool Check(TokenType tokenType) {
         if (IsAtEnd()) return false;
         return Peek().Type == tokenType;
      }

      private Token Advance() {
         if (!IsAtEnd()) current++;
         return Previous();
      }

      private bool IsAtEnd() {
         return Peek().Type == TokenType.EOF;
      }

      private Token Peek() {
         return tokens[current];
      }

      private Token Previous() {
         return tokens[current - 1];
      }
   }
}
