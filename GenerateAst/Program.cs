using System.IO;

namespace GenerateAst {
   class Program {
      static void Main(string[] args) {
         var outputPath = @"D:\dev\software\nlox\nlox\";
         DefineAst(
            outputPath,
            "Expr",
            new[] { "Assign   : Token name, Expr value",
                    "Binary   : Expr left, Token opr, Expr right",
                    "Call     : Expr callee, Token paren, List<Expr> arguments",
                    "Get      : Expr objekt, Token name",
                    "Grouping : Expr xpression",
                    "Literal  : object value",
                    "Logical  : Expr left, Token opr, Expr right",
                    "Unary    : Token opr, Expr right",
                    "Variable : Token name"
            });
         DefineAst(
            outputPath,
            "Stmt",
            new[] { "Block      : List<Stmt> statements",
                    "Class      : Token name, List<Stmt.Function> methods",
                    "Expression : Expr xpression",
                    "Function   : Token name, List<Token> parameters, List<Stmt> body",
                    "If         : Expr condition, Stmt thenBranch, Stmt elseBranch",
                    "Print      : Expr xpression",
                    "Return     : Token keyword, Expr value",
                    "Var        : Token name, Expr initializer",
                    "While      : Expr condition, Stmt body"
            });
      }

      private static void DefineAst(string path, string baseName, string[] types) {
         using (StreamWriter writer = new StreamWriter($"{path}{baseName}.cs")) {
            NameSpace(writer);
            AbstractClass(writer, baseName, types);
            writer.WriteLine($"}}");
         }
      }

      private static void NameSpace(StreamWriter writer) {
         writer.WriteLine("using System.Collections.Generic;");
         writer.WriteLine();
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
