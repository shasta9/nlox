using System;
using System.Collections.Generic;

namespace NLox {
   internal class Clock : ICallable {
      public int Arity() {
         return 0;
      }

      public object Call(Interpreter interpreter, List<object> arguments) {
         return (double)(DateTime.Now.Ticks / 10000);
      }
   }
}