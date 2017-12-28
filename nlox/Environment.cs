using System.Collections.Generic;

namespace NLox {
   internal class Environment {
      private readonly Environment enclosing;
      private readonly Dictionary<string, object> values = new Dictionary<string, object>();

      public Environment() {
         enclosing = null;
      }

      public Environment(Environment enclosing) {
         this.enclosing = enclosing;
      }
      
      public void Define(string name, object value) {
         /* this syntax overwrites an existing name or adds a new one
           .Add() throws an exception if the name already exists */
         values[name] = value;
      }

      public object Get(Token name) {
         if (values.ContainsKey(name.Lexeme)) {
            return values[name.Lexeme];
         }
         if (enclosing != null) {
            return enclosing.Get(name);
         }
         throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
      }

      public void Assign(Token name, object value) {
         if (values.ContainsKey(name.Lexeme)) {
            values[name.Lexeme] = value;
            return;
         }
         if (enclosing != null) {
            enclosing.Assign(name,value);
            return;
         }
         throw new RuntimeError(name,$"Undefined variable '{name.Lexeme}'.");
      }
   }
}
