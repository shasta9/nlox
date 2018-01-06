using System.CodeDom;
using System.Collections.Generic;

namespace NLox {
   internal class LoxInstance {
      private readonly LoxClass klass;
      private readonly Dictionary<string,object> fields = new Dictionary<string, object>();

      public LoxInstance(LoxClass klass) {
         this.klass = klass;
      }

      public object Get(Token name) {
         if (fields.ContainsKey(name.Lexeme)) {
            return fields[name.Lexeme];
         }
         throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
      }

      public void Set(Token name, object value) {
         fields[name.Lexeme] = value;
      }

      public override string ToString() {
         return $"{klass.Name} instance";
      }
   }
}