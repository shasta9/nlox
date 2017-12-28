using System;
using System.CodeDom;
using System.Collections.Generic;
using static NLox.TokenType;

namespace NLox {
   internal class Parser {

      private readonly List<Token> tokens;
      private int current = 0;

      public Parser(List<Token> tokens) {
         this.tokens = tokens;
      }

      internal List<Stmt> Parse() {
         var statements = new List<Stmt>();
         while (!IsAtEnd()) {
            statements.Add(Declaration());
         }
         return statements;
      }

      private Expr Expression() {
         return Equality();
      }

      private Stmt Declaration() {
         try {
            if (Match(VAR)) return VarDeclaration();
            return Statement();
         }
         catch (ParseError error) {
            Synchronize();
            return null;
         }
      }

      private Stmt Statement() {
         if (Match(PRINT)) return PrintStatement();
         return ExpressionStatement();
      }

      private Stmt PrintStatement() {
         Expr value = Expression();
         Consume(SEMICOLON, "Expect ';' after value.");
         return new Stmt.Print(value);
      }

      private Stmt VarDeclaration() {
         Token name = Consume(IDENTIFIER, "Expect variable name.");
         Expr initializer = null;
         if (Match(EQUAL)) {
            initializer = Expression();
         }
         Consume(SEMICOLON, "Expect ';' after variable declaration.");
         return new Stmt.Var(name, initializer);
      }

      private Stmt ExpressionStatement() {
         Expr value = Expression();
         Consume(SEMICOLON, "Expect ';' after expression.");
         return new Stmt.Expression(value);
      }

      private Expr Equality() {
         Expr expr = Comparison();
         while (Match(BANG_EQUAL, EQUAL_EQUAL)) {
            Token opr = Previous();
            Expr right = Comparison();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Comparison() {
         Expr expr = Addition();
         while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL)) {
            Token opr = Previous();
            Expr right = Addition();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Addition() {
         Expr expr = Multiplication();
         while (Match(MINUS, PLUS)) {
            Token opr = Previous();
            Expr right = Multiplication();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Multiplication() {
         Expr expr = Unary();
         while (Match(SLASH, STAR)) {
            Token opr = Previous();
            Expr right = Unary();
            expr = new Expr.Binary(expr, opr, right);
         }
         return expr;
      }

      private Expr Unary() {
         if (Match(BANG, MINUS)) {
            Token opr = Previous();
            Expr right = Unary();
            return new Expr.Unary(opr, right);
         }
         return Primary();
      }

      private Expr Primary() {
         if (Match(FALSE)) return new Expr.Literal(false);
         if (Match(TRUE)) return new Expr.Literal(true);
         if (Match(NIL)) return new Expr.Literal(null);
         if (Match(NUMBER, STRING)) {
            return new Expr.Literal(Previous().Literal);
         }
         if (Match(IDENTIFIER)) {
            return new Expr.Variable(Previous());
         }
         if (Match(LEFT_PAREN)) {
            Expr expr = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after expression.");
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
            if (Previous().Type == SEMICOLON) return;
            switch (Peek().Type) {
               case CLASS:
               case FUN:
               case VAR:
               case FOR:
               case IF:
               case WHILE:
               case PRINT:
               case RETURN:
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
         return Peek().Type == EOF;
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
