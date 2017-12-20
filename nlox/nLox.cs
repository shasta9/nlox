using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NLox {
   class nLox {

      static bool hadError = false;

      static void Main(string[] args) {
         //TestAstPrinter();
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
   }
}
