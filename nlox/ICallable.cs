using System.Collections.Generic;

namespace NLox {
   internal interface ICallable {
      int Arity { get; }
      object Call(Interpreter interpreter, List<object> arguments);
   }
}
