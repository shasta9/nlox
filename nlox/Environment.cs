﻿using System.Collections.Generic;

namespace NLox {
   internal class Environment {
      private readonly Dictionary<string, object> values = new Dictionary<string, object>();

      public Environment() {
         Enclosing = null;
      }

      public Environment(Environment enclosing) {
         Enclosing = enclosing;
      }

      public Environment Enclosing { get; }

      public void Define(string name, object value) {
         /* this syntax overwrites an existing name or adds a new one
           .Add() throws an exception if the name already exists */
         values[name] = value;
      }

      public object GetAt(int distance, string name) {
         return Ancestor(distance).values[name];
      }

      private Environment Ancestor(int distance) {
         Environment environment = this;
         for (int i = 0; i < distance; i++) {
            environment = environment.Enclosing;
         }
         return environment;
      }

      public object Get(Token name) {
         if (values.ContainsKey(name.Lexeme)) {
            return values[name.Lexeme];
         }
         if (Enclosing != null) {
            return Enclosing.Get(name);
         }
         throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
      }

      public void AssignAt(int distance, Token name, object value) {
         Ancestor(distance).values[name.Lexeme] = value;
      }

      public void Assign(Token name, object value) {
         if (values.ContainsKey(name.Lexeme)) {
            values[name.Lexeme] = value;
            return;
         }
         if (Enclosing != null) {
            Enclosing.Assign(name, value);
            return;
         }
         throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
      }
   }
}
