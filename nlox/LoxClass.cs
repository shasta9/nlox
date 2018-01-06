using System.Collections.Generic;

namespace NLox {
   internal class LoxClass : ICallable {
      private readonly LoxClass superclass;
      private readonly Dictionary<string, LoxFunction> methods;

      public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods) {
         Name = name;
         this.superclass = superclass;
         this.methods = methods;
      }

      public string Name { get; }

      public int Arity() {
         if (methods.TryGetValue("init", out LoxFunction initializer)) {
            return initializer.Arity();
         }
         return 0;
      }

      public object Call(Interpreter interpreter, List<object> arguments) {
         LoxInstance instance = new LoxInstance(this);
         if (methods.TryGetValue("init", out LoxFunction initializer)) {
            initializer.Bind(instance).Call(interpreter, arguments);
         }
         return instance;
      }

      public LoxFunction FindMethod(LoxInstance instance, string name) {
         if (methods.ContainsKey(name)) {
            return methods[name].Bind(instance);
         }
         if (superclass != null) {
            return superclass.FindMethod(instance, name);
         }
         return null;
      }

      public override string ToString() {
         return Name;
      }
   }
}