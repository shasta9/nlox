using System.Collections.Generic;

namespace NLox {
   internal interface ICallable {
      int Arity();
      object Call(Interpreter interpreter, List<object> arguments);
   }
}
