using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateAst {
   class Program {
      static void Main(string[] args) {
         DefineAst("Expr",
            new List<string>(
               new[] { "Binary   : Expr left, Token op, Expr right",
                       "Grouping : Expr expression",
                       "Literal  : object value",
                       "Unary    : Token op, Expr right"
               }
            )
         );
      }

      private static void DefineAst(string baseName, List<string> types) {
         var path = @"d:\dev\software\nlox\nlox\" + baseName + ".cs";
         using (StreamWriter writer = new StreamWriter(path)) {

            writer.WriteLine("namespace Nlox {");
            writer.WriteLine("   internal abstract class " + baseName + " {");
            writer.WriteLine("   }");

            foreach (var type in types) {
               var className = type.Split(':')[0].Trim();
               var fields = type.Split(':')[1].Trim();
               DefineType(writer, baseName, className, fields);
            }
            writer.WriteLine("}");
         }
      }

      private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList) {
         writer.WriteLine();
         writer.WriteLine("   internal class " + className + " : " + baseName + " {");
         // fields
         var fields = fieldList.Split(',');
         foreach (var field in fields) {
            writer.WriteLine("      public " + Capitalise(field) + ";");
         }
         // ctor
         writer.WriteLine("      public " + className + "(" + fieldList + ") {");
         // store parameters in fields
         foreach (var field in fields) {
            var name = field.Split(' ')[1];
            writer.WriteLine("         " + Capitalise(name) + " = " + name + ";");
         }
         writer.WriteLine("      }");
         writer.WriteLine("   }");
      }

      private static string Capitalise(string source) {
         return source.Substring(0, 1).ToUpper() + source.Substring(1).ToLower();
      }
   }
}
