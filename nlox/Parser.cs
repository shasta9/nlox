using System;
using System.Collections.Generic;

namespace NLox {
   internal class Parser {

      private readonly List<Token> tokens;
      private int current = 0;

      public Parser(List<Token> tokens) {
         this.tokens = tokens;
      }

      internal Expr Parse() {
         try {
            return Expression();
         }
         catch (ParseError error) {
            return null;
         }
      }

      private Expr Expression() {
         return Equality();
      }

      private Expr Equality() {
         Expr expr = Comparison();
         while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
            Token opr = Previous();
            Expr right = Comparison();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Comparison() {
         Expr expr = Addition();
         while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
            Token opr = Previous();
            Expr right = Addition();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Addition() {
         Expr expr = Multiplication();
         while (Match(TokenType.MINUS, TokenType.PLUS)) {
            Token opr = Previous();
            Expr right = Multiplication();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Multiplication() {
         Expr expr = Unary();
         while (Match(TokenType.SLASH, TokenType.STAR)) {
            Token opr = Previous();
            Expr right = Unary();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Unary() {
         if (Match(TokenType.BANG, TokenType.MINUS)) {
            Token opr = Previous();
            Expr right = Unary();
            return new Expr.Unary(opr, right);
         }
         return Primary();
      }

      private Expr Primary() {
         if (Match(TokenType.FALSE)) return new Expr.Literal(false);
         if (Match(TokenType.TRUE)) return new Expr.Literal(true);
         if (Match(TokenType.NIL)) return new Expr.Literal(null);
         if (Match(TokenType.NUMBER, TokenType.STRING)) {
            return new Expr.Literal(Previous().Literal);
         }
         if (Match(TokenType.LEFT_PAREN)) {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
         }
         throw Error(Peek(), "Expect expression.");
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

      private Token Consume(TokenType type, string message) {
         if (Check(type)) return Advance();
         throw Error(Peek(), message);
      }

      private void Synchronize() {
         Advance();
         while (!IsAtEnd()) {
            if (Previous().Type == TokenType.SEMICOLON) return;
            switch (Peek().Type) {
               case TokenType.CLASS:
               case TokenType.FUN:
               case TokenType.VAR:
               case TokenType.FOR:
               case TokenType.IF:
               case TokenType.WHILE:
               case TokenType.PRINT:
               case TokenType.RETURN:
                  return;
            }
            Advance();
         }
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

      private ParseError Error(Token token, String message) {
         nLox.Error(token, message);
         return new ParseError();
      }
   } 
   
   public class ParseError : Exception { }

}
