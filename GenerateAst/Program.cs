using System.IO;

namespace GenerateAst {
   class Program {
      static void Main(string[] args) {
         var outputPath = @"C:\Users\shast\OneDrive\Documents\Visual Studio 2017\Projects\nlox\nlox\";
         DefineAst(
            outputPath,
            "Expr",
            new[] { "Binary   : Expr left, Token opr, Expr right",
                    "Grouping : Expr expression",
                    "Literal  : object value",
                    "Unary    : Token opr, Expr right",
                    "Variable : Token name" });
         DefineAst(
            outputPath,
            "Stmt",
            new[] { "Expression : Expr xpression",
                    "Print      : Expr xpression",
                    "Var        : Token name, Expr initializer" });
      }

      private static void DefineAst(string path, string baseName, string[] types) {
         using (StreamWriter writer = new StreamWriter($"{path}{baseName}.cs")) {
            NameSpace(writer);
            AbstractClass(writer, baseName, types);
            writer.WriteLine($"}}");
         }
      }

      private static void NameSpace(StreamWriter writer) {
         writer.WriteLine("namespace NLox {");
      }

      private static void AbstractClass(StreamWriter writer, string baseName, string[] types) {
         writer.WriteLine();
         writer.WriteLine($"   internal abstract class {baseName} {{");
         VisitorInterface(writer, baseName, types);
         writer.WriteLine();
         writer.WriteLine($"      public abstract T Accept<T>(I{baseName}Visitor<T> visitor);");
         foreach (var type in types) {
            var className = type.Split(':')[0].Trim();
            var fields = type.Split(':')[1].Trim();
            DefineSubclass(writer, baseName, className, fields);
         }
         writer.WriteLine("   }");
      }

      private static void VisitorInterface(StreamWriter writer, string baseName, string[] types) {
         writer.WriteLine();
         writer.WriteLine($"      public interface I{baseName}Visitor<T> {{");
         foreach (var type in types) {
            var className = type.Split(':')[0].Trim();
            writer.WriteLine($"         T Visit{className}{baseName}({className} {baseName.ToLower()});");
         }
         writer.WriteLine("      }");
      }

      private static void DefineSubclass(StreamWriter writer, string baseName, string className, string fieldList) {
         writer.WriteLine();
         writer.WriteLine($"      public class {className} : {baseName} {{");
         // properties
         var fields = fieldList.Split(',');
         foreach (var field in fields) {
            var trimmed = field.Trim();
            var type = trimmed.Split(' ')[0].Trim();
            var name = trimmed.Split(' ')[1].Trim();
            writer.WriteLine($"         public {type} {Capitalise(name)} {{ get; }}");
         }
         // ctor
         writer.WriteLine($"         public {className} ({fieldList}) {{");
         // store parameters in fields
         foreach (var field in fields) {
            var trimmed = field.Trim();
            var name = trimmed.Split(' ')[1];
            writer.WriteLine($"            {Capitalise(name)} = {name};");
         }
         writer.WriteLine("         }");
         // visitor interface
         writer.WriteLine();
         writer.WriteLine($"         public override T Accept<T>(I{baseName}Visitor<T> visitor) {{");
         writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
         writer.WriteLine("         }");
         writer.WriteLine("      }");
      }

      private static string Capitalise(string source) {
         return source.Substring(0, 1).ToUpper() + source.Substring(1);
      }
   }
}
