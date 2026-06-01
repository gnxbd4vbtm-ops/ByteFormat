using System;

namespace ByteFormat.Core
{
    public class Parser
    {
        private readonly Lexer _lex;
        private Token _curr;
        public Parser(string text) { _lex = new Lexer(text); _curr = _lex.NextToken(); }
        private void Next() => _curr = _lex.NextToken();
        private bool Accept(TokenKind k) { if (_curr.Kind==k) { Next(); return true; } return false; }
        private void Expect(TokenKind k) { if (_curr.Kind!=k) throw new Exception($"Expected {k} but got {_curr.Kind}"); Next(); }

        public ObjectNode Parse()
        {
            var root = new ObjectNode();
            while (_curr.Kind != TokenKind.Eof)
            {
                if (_curr.Kind == TokenKind.Identifier)
                {
                    var first = _curr.Text; Next();
                    if (_curr.Kind == TokenKind.Identifier && _curr.Kind == TokenKind.Identifier)
                    {
                        // unreachable check; kept to mirror previous logic
                    }
                    if (_curr.Kind == TokenKind.LBrace)
                    {
                        Expect(TokenKind.LBrace);
                        var obj = ParseObjectBody();
                        root.Values[first] = obj;
                        continue;
                    }
                    // named block like "address office { ... }"
                    if (_curr.Kind == TokenKind.Identifier && PeekIsLBrace())
                    {
                        var name = _curr.Text; Next();
                        Expect(TokenKind.LBrace);
                        var obj = ParseObjectBody();
                        if (!root.Values.TryGetValue(first, out var existing) || existing is not ObjectNode addrRoot)
                        {
                            addrRoot = new ObjectNode();
                            root.Values[first] = addrRoot;
                        }
                        addrRoot.Values[name] = obj;
                        continue;
                    }
                }
                else { Next(); }
            }
            return root;
        }

        private bool PeekIsLBrace()
        {
            // Peek the next token to determine whether an identifier is followed by a '{'
            var t = _lex.PeekToken();
            return t.Kind == TokenKind.LBrace;
        }

        private ObjectNode ParseObjectBody()
        {
            var obj = new ObjectNode();
            while (_curr.Kind != TokenKind.RBrace && _curr.Kind != TokenKind.Eof)
            {
                if (_curr.Kind == TokenKind.Identifier)
                {
                    var key = _curr.Text; Next();
                    if (Accept(TokenKind.Equals))
                    {
                        var val = ParseValue();
                        obj.Values[key] = val;
                        continue;
                    }
                    if (_curr.Kind==TokenKind.Identifier && PeekIsLBrace())
                    {
                        var name = _curr.Text; Next();
                        Expect(TokenKind.LBrace);
                        var sub = ParseObjectBody();
                        if (!obj.Values.TryGetValue(key, out var maybe) || maybe is not ObjectNode root)
                        {
                            root = new ObjectNode(); obj.Values[key] = root;
                        }
                        root.Values[name] = sub;
                        continue;
                    }
                    if (Accept(TokenKind.LBrace))
                    {
                        var subobj = ParseObjectBody();
                        obj.Values[key] = subobj; continue;
                    }
                }
                else if (_curr.Kind==TokenKind.RBrace) break;
                else Next();
            }
            if (_curr.Kind==TokenKind.RBrace) Next();
            return obj;
        }

        private Node ParseValue()
        {
            if (_curr.Kind==TokenKind.String) { var s = _curr.Text; Next(); return new ValueNode(s); }
            if (_curr.Kind==TokenKind.MultilineString) { var s = _curr.Text; Next(); return new ValueNode(s); }
            if (_curr.Kind==TokenKind.Number) { var s = _curr.Text; Next(); return new ValueNode(s); }
            if (_curr.Kind==TokenKind.True) { Next(); return new ValueNode("true"); }
            if (_curr.Kind==TokenKind.False) { Next(); return new ValueNode("false"); }
            if (_curr.Kind==TokenKind.EnumMarker)
            {
                Next();
                if (Accept(TokenKind.Colon))
                {
                    if (_curr.Kind==TokenKind.Identifier) { var name=_curr.Text; Next(); return new ValueNode("enum:"+name); }
                }
                return new ValueNode("enum:");
            }
            if (Accept(TokenKind.LBracket))
            {
                var arr = new ArrayNode();
                while (_curr.Kind != TokenKind.RBracket && _curr.Kind!=TokenKind.Eof)
                {
                    if (_curr.Kind==TokenKind.LBrace)
                    {
                        Next(); var obj = ParseObjectBody(); arr.Items.Add(obj); if (Accept(TokenKind.Comma)) continue; else continue;
                    }
                    if (_curr.Kind==TokenKind.String) { arr.Items.Add(new ValueNode(_curr.Text)); Next(); }
                    else if (_curr.Kind==TokenKind.Number) { arr.Items.Add(new ValueNode(_curr.Text)); Next(); }
                    else if (_curr.Kind==TokenKind.True) { arr.Items.Add(new ValueNode("true")); Next(); }
                    else if (_curr.Kind==TokenKind.False) { arr.Items.Add(new ValueNode("false")); Next(); }
                    else Next();
                    Accept(TokenKind.Comma);
                }
                Expect(TokenKind.RBracket);
                return arr;
            }
            if (Accept(TokenKind.LBrace))
            {
                return ParseObjectBody();
            }
            var tok = _curr; Next(); return new ValueNode(tok.Text);
        }
    }
}
