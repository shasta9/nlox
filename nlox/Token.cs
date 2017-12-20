﻿namespace NLox {
    internal class Token {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        internal Token(TokenType type, string lexeme, object literal, int line) {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString() {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
