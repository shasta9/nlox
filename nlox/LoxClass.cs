using System.Collections.Generic;

namespace NLox {
   internal class LoxClass : ICallable {
      public LoxClass(string name) {
         Name = name;
      }

      public string Name { get; }
      public int Arity => 0;

      public override string ToString() {
         return Name;
      }

      public object Call(Interpreter interpreter, List<object> arguments) {
         LoxInstance instance = new LoxInstance(this);
         return instance;
      }
   }
}