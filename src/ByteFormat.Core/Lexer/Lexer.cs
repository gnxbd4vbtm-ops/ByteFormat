using System.Text;

namespace ByteFormat.Core.Lexer;

public class Lexer
{
    private readonly string _input;
    private int _pos;

    public Lexer(string input)
    {
        _input = input;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_pos < _input.Length)
        {
            char c = _input[_pos];

            if (char.IsWhiteSpace(c))
            {
                _pos++;
                continue;
            }

            switch (c)
            {
                case '[': tokens.Add(new Token(TokenType.LBracket)); _pos++; break;
                case ']': tokens.Add(new Token(TokenType.RBracket)); _pos++; break;
                case '{': tokens.Add(new Token(TokenType.LBrace)); _pos++; break;
                case '}': tokens.Add(new Token(TokenType.RBrace)); _pos++; break;
                case '(': tokens.Add(new Token(TokenType.LParen)); _pos++; break;
                case ')': tokens.Add(new Token(TokenType.RParen)); _pos++; break;
                case ':': tokens.Add(new Token(TokenType.Colon)); _pos++; break;

                case '"':
                    tokens.Add(ReadString());
                    break;

                default:
                    if (char.IsDigit(c))
                        tokens.Add(ReadNumber());
                    else if (char.IsLetter(c))
                        tokens.Add(ReadIdentifier());
                    else
                        throw new Exception($"Unexpected char: {c}");
                    break;
            }
        }

        tokens.Add(new Token(TokenType.EOF));
        return tokens;
    }

    private Token ReadString()
    {
        _pos++; // skip "
        var sb = new StringBuilder();

        while (_pos < _input.Length && _input[_pos] != '"')
            sb.Append(_input[_pos++]);

        _pos++; // skip closing "
        return new Token(TokenType.String, sb.ToString());
    }

    private Token ReadNumber()
    {
        var sb = new StringBuilder();

        while (_pos < _input.Length && char.IsDigit(_input[_pos]))
            sb.Append(_input[_pos++]);

        return new Token(TokenType.Number, sb.ToString());
    }

    private Token ReadIdentifier()
    {
        var sb = new StringBuilder();

        while (_pos < _input.Length && char.IsLetter(_input[_pos]))
            sb.Append(_input[_pos++]);

        var value = sb.ToString();

        if (value is "true" or "false")
            return new Token(TokenType.Boolean, value);

        return new Token(TokenType.Identifier, value);
    }
}