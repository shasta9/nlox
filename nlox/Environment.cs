using System.Collections.Generic;

namespace NLox {
   internal class Environment {
      private readonly Dictionary<string, object> values = new Dictionary<string, object>();

      public void Define(string name, object value) {
         /* this syntax overwrites an existing name or adds a new one
           .Add() throws an exception if the name already exists */
         values[name]= value;
      }

     public object Get(Token name) {
        if (values.ContainsKey(name.Lexeme)) {
           return values[name.Lexeme];
        }
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
      }
   }
}
