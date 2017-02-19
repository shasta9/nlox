using System;
using System.Collections.Generic;

namespace lox {
    internal class Scanner {
        private string source;
        private List<Token> tokens = new List<Token>();

        internal Scanner(String source) {
            this.source = source;
        }

        internal List<Token> ScanTokens() {
            throw new NotImplementedException();
        }
    }
}


