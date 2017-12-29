using System;
using System.Collections.Generic;
using static NLox.TokenType;

namespace NLox {
   internal class Parser {

      private readonly List<Token> tokens;
      private int current = 0;

      public Parser(List<Token> tokens) {
         this.tokens = tokens;
      }

      public List<Stmt> Parse() {
         var statements = new List<Stmt>();
         while (!IsAtEnd()) {
            statements.Add(Declaration());
         }
         return statements;
      }

      private Stmt Declaration() {
         try {
            if (Match(FUN)) return Function("function");
            if (Match(VAR)) return VarDeclaration();
            return Statement();
         }
         catch (ParseError error) {
            Synchronize();
            return null;
         }
      }

      private Stmt Statement() {
         if (Match(FOR)) return ForStatement();
         if (Match(IF)) return IfStatement();
         if (Match(PRINT)) return PrintStatement();
         if (Match(RETURN)) return ReturnStatement();
         if (Match(WHILE)) return WhileStatement();
         if (Match(LEFT_BRACE)) return new Stmt.Block(Block());
         return ExpressionStatement();
      }

      private Stmt ForStatement() {
         Consume(LEFT_PAREN, "Expect '(' after 'for'.");
         Stmt initializer;
         if (Match(SEMICOLON)) {
            initializer = null;
         }
         else if (Match(VAR)) {
            initializer = VarDeclaration();
         }
         else {
            initializer = ExpressionStatement();
         }
         Expr condition = null;
         if (!Check(SEMICOLON)) {
            condition = Expression();
         }
         Consume(SEMICOLON, "Expect ';' after loop condition.");
         Expr increment = null;
         if (!Check(RIGHT_PAREN)) {
            increment = Expression();
         }
         Consume(RIGHT_PAREN, "Expect ')' after for clauses.");
         Stmt body = Statement();
         if (increment != null) {
            body = new Stmt.Block(
               new List<Stmt>(
                  new[] { body, new Stmt.Expression(increment) }));
         }
         if (condition == null) condition = new Expr.Literal(true);
         body = new Stmt.While(condition, body);
         if (initializer != null) {
            body = new Stmt.Block(
               new List<Stmt>(
                  new[] { initializer, body }));
         }
         return body;
      }

      private Stmt IfStatement() {
         Consume(LEFT_PAREN, "Expect '(' after 'if'.");
         Expr condition = Expression();
         Consume(RIGHT_PAREN, "Expect ')' after if condition.");
         Stmt thenBranch = Statement();
         Stmt elseBranch = null;
         if (Match(ELSE)) {
            elseBranch = Statement();
         }
         return new Stmt.If(condition, thenBranch, elseBranch);
      }

      private Stmt PrintStatement() {
         Expr value = Expression();
         Consume(SEMICOLON, "Expect ';' after value.");
         return new Stmt.Print(value);
      }

      private Stmt ReturnStatement() {
         Token keyword = Previous();
         Expr value = null;
         if (!Check(SEMICOLON)) {
            value = Expression();
         }
         Consume(SEMICOLON, "Expect ';' after return value.");
         return new Stmt.Return(keyword, value);
      }

      private Stmt WhileStatement() {
         Consume(LEFT_PAREN, "Expect '(' after 'while'.");
         Expr condition = Expression();
         Consume(RIGHT_PAREN, "Expect ')' after while condition.");
         Stmt body = Statement();
         return new Stmt.While(condition, body);
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

      private Stmt.Function Function(string kind) {
         Token name = Consume(IDENTIFIER, $"Expect {kind} name.");
         Consume(LEFT_PAREN, $"Expect '(' after {kind} name.");
         List<Token> parameters = new List<Token>();
         if (!Check(RIGHT_PAREN)) {
            do {
               if (parameters.Count >= 8) {
                  Error(Peek(), "Cannot have more than 8 parameters.");
               }
               parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
            } while (Match(COMMA));
         }
         Consume(RIGHT_PAREN, "Expect ')' after parameters.");
         Consume(LEFT_BRACE, $"Expect '{{' before {kind} body.");
         List<Stmt> body = Block();
         return new Stmt.Function(name, parameters, body);
      }

      private List<Stmt> Block() {
         List<Stmt> statements = new List<Stmt>();
         while (!Check(RIGHT_BRACE) && !IsAtEnd()) {
            statements.Add(Declaration());
         }
         Consume(RIGHT_BRACE, "Expect '}' after block.");
         return statements;
      }

      private Expr Expression() {
         return Assignment();
      }

      private Expr Assignment() {
         Expr expr = Or();

         if (Match(EQUAL)) {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Expr.Variable v) {
               Token name = v.Name;
               return new Expr.Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
         }

         return expr;
      }

      private Expr Or() {
         Expr expr = And();
         while (Match(OR)) {
            Token opr = Previous();
            Expr right = And();
            expr = new Expr.Logical(expr, opr, right);
         }
         return expr;
      }

      private Expr And() {
         Expr expr = Equality();
         while (Match(AND)) {
            Token opr = Previous();
            Expr right = Equality();
            expr = new Expr.Logical(expr, opr, right);
         }
         return expr;
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
         return Call();
      }

      private Expr Call() {
         Expr expr = Primary();
         while (true) {
            if (Match(LEFT_PAREN)) {
               expr = FinishCall(expr);
            }
            else {
               break;
            }
         }
         return expr;
      }

      private Expr FinishCall(Expr callee) {
         List<Expr> arguments = new List<Expr>();
         if (!Check(RIGHT_PAREN)) {
            do {
               if (arguments.Count >= 8) {
                  Error(Peek(), "Cannot have more than 8 arguments.");
               }
               arguments.Add(Expression());
            } while (Match(COMMA));
         }
         Token paren = Consume(RIGHT_PAREN, "Expect ')' after function arguments.");
         return new Expr.Call(callee, paren, arguments);
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
}
