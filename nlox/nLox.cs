using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NLox {
   internal class nLox {

      private static readonly Interpreter interpreter = new Interpreter();
      private static bool hadError = false;
      private static bool hadRuntimeError = false;

      private static void Main(string[] args) {
         if (args.Length > 1) {
            Console.WriteLine("Usage: nlox [script]");
         }
         else if (args.Length == 1) {
            RunFile(args[0]);
         }
         else {
            RunPrompt();
         }
      }

      private static void RunFile(string path) {
         var bytes = File.ReadAllBytes(path);
         string source = Encoding.Default.GetString(bytes);
         Run(source);
         // Indicate an error in the exit code.
         if (hadError) Environment.Exit(65);
         if (hadRuntimeError) Environment.Exit(70);
      }

      private static void RunPrompt() {
         for (;;) {
            Console.Write("> ");
            Run(Console.ReadLine());
            hadError = false;
         }
      }

      private static void Run(string source) {
         Scanner scanner = new Scanner(source);
         List<Token> tokens = scanner.ScanTokens();
         Parser parser = new Parser(tokens);
         Expr expression = parser.Parse();
         if (hadError) return;
         interpreter.Interpret(expression);
      }

      public static void Error(int line, string message) {
         Report(line, "", message);
      }

      public static void Error(Token token, String message) {
         if (token.Type == TokenType.EOF) {
            Report(token.Line, " at end", message);
         }
         else {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
         }
      }

      public static void RuntimeError(RuntimeError error) {
         Console.WriteLine(error.Message + $"\n[line {error.Token.Line}]");
         hadRuntimeError = true;
      }

      public static void Report(int line, string where, string message) {
         Console.WriteLine($"Line [{line}] Error {where}: {message}");
         hadError = true;
      }

      private static void TestAstPrinter() {
         // -123 * (45.67) => (* (- 123) (group 45.67))
         Expr expression =
            new Expr.Binary(
               new Expr.Unary(
                   new Token(TokenType.MINUS, "-", null, 1),
                   new Expr.Literal(123)
               ),
               new Token(TokenType.STAR, "*", null, 1),
               new Expr.Grouping(
                   new Expr.Literal(45.67)
               )
            );

         var printer = new AstPrinter();
         Console.WriteLine(expression.Accept(printer));
         Console.ReadLine();
      }

      private static void TestRpnPrinter() {
         // (1 + 2) * (4 - 3) => 1 2 + 4 3 - *
         Expr expression =
            new Expr.Binary(
               new Expr.Grouping(
                  new Expr.Binary(
                     new Expr.Literal(1),
                     new Token(TokenType.PLUS, "+", null, 1),
                     new Expr.Literal(2)
                  )
               ),
               new Token(TokenType.STAR, "*", null, 1),
               new Expr.Grouping(
                  new Expr.Binary(
                     new Expr.Literal(4),
                     new Token(TokenType.MINUS, "-", null, 1),
                     new Expr.Literal(3)
                  )
               )
            );

         var printer = new RpnPrinter();
         Console.WriteLine(expression.Accept(printer));
         Console.ReadLine();
      }
   }
}
