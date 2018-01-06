using System.Collections.Generic;

namespace NLox {
   internal class LoxFunction : ICallable {
      private readonly Stmt.Function declaration;
      private readonly Environment closure;
      private readonly bool isInitializer;

      public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer) {
         this.declaration = declaration;
         this.closure = closure;
         this.isInitializer = isInitializer;
      }

      public int Arity() {
         return declaration.Parameters.Count;
      }

      public LoxFunction Bind(LoxInstance instance) {
         Environment environment = new Environment(closure);
         environment.Define("this", instance);
         return new LoxFunction(declaration, environment, isInitializer);
      }

      public object Call(Interpreter interpreter, List<object> arguments) {
         Environment environment = new Environment(closure);
         for (int i = 0; i < declaration.Parameters.Count; i++) {
            environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
         }

         try {
            interpreter.ExecuteBlock(declaration.Body, environment);
         }
         catch (Return returnValue) {
            return returnValue.Value;
         }
         if (isInitializer) return closure.GetAt(0, "this");
         return null;
      }

      public override string ToString() {
         return $"<fn {declaration.Name.Lexeme}>";
      }
   }
}