using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NLox {
   class nLox {

      static bool hadError = false;

      static void Main(string[] args) {
         //TestAstPrinter();
         //TestRpnPrinter();
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
         // if (hadError) System.exit(65);
      }

      private static void RunPrompt() {
         for (; ; ) {
            Console.Write("> ");
            Run(Console.ReadLine());
            hadError = false;
         }
      }

      private static void Run(string source) {
         Scanner scanner = new Scanner(source);
         List<Token> tokens = scanner.ScanTokens();

         // For now, just print the tokens.
         foreach (Token token in tokens) {
            Console.WriteLine(token);
         }
      }

      internal static void Error(int line, string message) {
         Report(line, "", message);
      }

      internal static void Report(int line, string where, string message) {
         Console.WriteLine($"Line [{line}] Error {where}: {message}");
         hadError = true;
      }

      private static void TestAstPrinter() {
         // -123 * (45.67) => (* (- 123) (group 45.67))
         Expr expression =
            new Binary(
               new Unary(
                   new Token(TokenType.MINUS, "-", null, 1),
                   new Literal(123)
               ),
               new Token(TokenType.STAR, "*", null, 1),
               new Grouping(
                   new Literal(45.67)
               )
            );

         var printer = new AstPrinter();
         expression.Accept(printer);
         Console.WriteLine(printer.ToString());
         Console.ReadLine();
      }

      private static void TestRpnPrinter() {
         // (1 + 2) * (4 - 3) => 1 2 + 4 3 - *
         Expr expression =
            new Binary(
               new Grouping(
                  new Binary(
                     new Literal(1),
                     new Token(TokenType.PLUS, "+", null, 1),
                     new Literal(2)
                  )
               ),
               new Token(TokenType.STAR, "*", null, 1),
               new Grouping(
                  new Binary(
                     new Literal(4),
                     new Token(TokenType.MINUS, "-", null, 1),
                     new Literal(3)
                  )
               )
            );

         var printer = new RpnPrinter();
         expression.Accept(printer);
         Console.WriteLine(printer.ToString());
         Console.ReadLine();
      }
   }
}
