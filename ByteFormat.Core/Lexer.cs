using System;

namespace ByteFormat.Core
{
    internal class Lexer
    {
        private readonly string _s;
        private int _i;
        public Lexer(string s) { _s = s; _i = 0; }
        private char Curr => _i < _s.Length ? _s[_i] : '\0';
        private void Next() => _i++;
        public Token NextToken()
        {
            while (char.IsWhiteSpace(Curr)) Next();
            if (Curr == '\0') return new Token(TokenKind.Eof, string.Empty);
            if (Curr == '#') { while (Curr != '\n' && Curr != '\0') Next(); return NextToken(); }
            if (Curr == '=') { Next(); return new Token(TokenKind.Equals, "="); }
            if (Curr == '{') { Next(); return new Token(TokenKind.LBrace, "{"); }
            if (Curr == '}') { Next(); return new Token(TokenKind.RBrace, "}"); }
            if (Curr == '[') { Next(); return new Token(TokenKind.LBracket, "["); }
            if (Curr == ']') { Next(); return new Token(TokenKind.RBracket, "]"); }
            if (Curr == ',') { Next(); return new Token(TokenKind.Comma, ","); }
            if (Curr == ':') { Next(); return new Token(TokenKind.Colon, ":"); }
            if (Curr == '"')
            {
                // check for triple-quote
                if (_i + 2 < _s.Length && _s[_i + 1] == '"' && _s[_i + 2] == '"')
                {
                    _i += 3;
                    var start = _i;
                    while (!(_i + 2 < _s.Length && _s[_i] == '"' && _s[_i + 1] == '"' && _s[_i + 2] == '"') && Curr != '\0') Next();
                    var text = _s[start.._i];
                    _i += 3;
                    return new Token(TokenKind.MultilineString, text);
                }
                Next();
                var sb = new System.Text.StringBuilder();
                while (Curr != '"' && Curr != '\0')
                {
                    if (Curr == '\\') { Next(); if (Curr == 'n') sb.Append('\n'); else sb.Append(Curr); Next(); continue; }
                    sb.Append(Curr); Next();
                }
                if (Curr == '"') Next();
                return new Token(TokenKind.String, sb.ToString());
            }
            if (char.IsLetter(Curr) || Curr == '_' )
            {
                var start = _i;
                while (char.IsLetterOrDigit(Curr) || Curr == '_' || Curr=='-') Next();
                var id = _s[start.._i];
                if (id == "true") return new Token(TokenKind.True, id);
                if (id == "false") return new Token(TokenKind.False, id);
                if (id == "enum") return new Token(TokenKind.EnumMarker, id);
                return new Token(TokenKind.Identifier, id);
            }
            if (char.IsDigit(Curr) || (Curr=='-' && char.IsDigit(Peek())))
            {
                var start = _i;
                if (Curr=='-') Next();
                while (char.IsDigit(Curr)) Next();
                if (Curr == '.') { Next(); while (char.IsDigit(Curr)) Next(); }
                var num = _s[start.._i];
                return new Token(TokenKind.Number, num);
            }
            // unknown char: skip
            Next();
            return NextToken();
        }

        private char Peek() => _i + 1 < _s.Length ? _s[_i + 1] : '\0';

        // Peek the next token without advancing the lexer state
        public Token PeekToken()
        {
            var saved = _i;
            var tok = NextToken();
            _i = saved;
            return tok;
        }
    }
}
