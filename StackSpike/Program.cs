using System;
using System.Collections.Generic;

namespace StackSpike {
   class Program {
      static void Main(string[] args) {
         
         var stack = new Stack<int>();
         
         stack.Push(1);
         stack.Push(2);
         stack.Push(3);

         Console.WriteLine($"Peek={stack.Peek()}");

         Console.WriteLine("Stack enumeration:");
         foreach (var i in stack) {
            Console.WriteLine(i);
         }
         
         Console.ReadLine();
      }
   }
}
