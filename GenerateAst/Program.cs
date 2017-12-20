using System.IO;

namespace GenerateAst {
   class Program {
      static void Main(string[] args) {
         DefineAst(
            "d:\\dev\\software\\nlox\\nlox\\",
            "Expr",
            new[] { "Binary   : Expr left, Token opr, Expr right",
                    "Grouping : Expr expression",
                    "Literal  : object value",
                    "Unary    : Token opr, Expr right" });
      }

      private static void DefineAst(string path, string baseName, string[] types) {
         using (StreamWriter writer = new StreamWriter($"{path}{baseName}.cs")) {
            writer.WriteLine("namespace NLox {");
            DefineVisitorInterface(writer, baseName, types);
            DefineAbstractClass(writer, baseName, types);
            foreach (var type in types) {
               var className = type.Split(':')[0].Trim();
               var fields = type.Split(':')[1].Trim();
               DefineSubclass(writer, baseName, className, fields);
            }
            writer.WriteLine($"}}");
         }
      }

      private static void DefineVisitorInterface(StreamWriter writer, string baseName, string[] types) {
         writer.WriteLine();
         writer.WriteLine($"   internal interface I{baseName}Visitor {{");
         foreach (var type in types) {
            var className = type.Split(':')[0].Trim();
            writer.WriteLine($"      void Visit{className}{baseName}({className} {baseName.ToLower()});");
         }
         writer.WriteLine($"   }}");
      }

      private static void DefineAbstractClass(StreamWriter writer, string baseName, string[] types) {
         writer.WriteLine();
         writer.WriteLine($"   internal abstract class {baseName} {{");
         writer.WriteLine($"      public abstract void Accept(I{baseName}Visitor visitor);");
         writer.WriteLine($"   }}");
      }

      private static void DefineSubclass(StreamWriter writer, string baseName, string className, string fieldList) {
         writer.WriteLine();
         writer.WriteLine($"   internal class {className} : {baseName} {{");
         // properties
         var fields = fieldList.Split(',');
         foreach (var field in fields) {
            var trimmed = field.Trim();
            var type = trimmed.Split(' ')[0].Trim();
            var name = trimmed.Split(' ')[1].Trim();
            writer.WriteLine($"      public {type} {Capitalise(name)} {{ get; }}");
         }
         // ctor
         writer.WriteLine($"      public {className} ({fieldList}) {{");
         // store parameters in fields
         foreach (var field in fields) {
            var trimmed = field.Trim();
            var name = trimmed.Split(' ')[1];
            writer.WriteLine($"         {Capitalise(name)} = {name};");
         }
         writer.WriteLine($"      }}");
         // visitor interface
         writer.WriteLine();
         writer.WriteLine($"      public override void Accept(I{baseName}Visitor visitor) {{");
         writer.WriteLine($"         visitor.Visit{className}{baseName}(this);");
         writer.WriteLine($"      }}");
         writer.WriteLine($"   }}");
      }

      private static string Capitalise(string source) {
         return source.Substring(0, 1).ToUpper() + source.Substring(1);
      }
   }
}
