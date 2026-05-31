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

            switch (c)
            {
                case '[': tokens.Add(new Token(TokenType.LeftBracket)); break;
                case ']': tokens.Add(new Token(TokenType.RightBracket)); break;
                case '{': tokens.Add(new Token(TokenType.LeftBrace)); break;
                case '}': tokens.Add(new Token(TokenType.RightBrace)); break;
                case '(': tokens.Add(new Token(TokenType.LeftParen)); break;
                case ')': tokens.Add(new Token(TokenType.RightParen)); break;
                case ':': tokens.Add(new Token(TokenType.Colon)); break;

                case '"':
                    tokens.Add(ReadString());
                    break;

                default:
                    if (char.IsWhiteSpace(c))
                    {
                        _pos++;
                        continue;
                    }
                    else if (char.IsDigit(c))
                    {
                        tokens.Add(ReadNumber());
                        continue;
                    }
                    else if (char.IsLetter(c))
                    {
                        tokens.Add(ReadIdentifier());
                        continue;
                    }
                    else
                    {
                        throw new Exception($"Unexpected character: {c}");
                    }
                    break;
            }

            _pos++;
        }

        tokens.Add(new Token(TokenType.EOF));
        return tokens;
    }

    private Token ReadString()
    {
        _pos++; // skip "
        var sb = new StringBuilder();

        while (_pos < _input.Length && _input[_pos] != '"')
        {
            sb.Append(_input[_pos++]);
        }

        _pos++; // skip closing "
        return new Token(TokenType.String, sb.ToString());
    }

    private Token ReadNumber()
    {
        var sb = new StringBuilder();

        while (_pos < _input.Length && char.IsDigit(_input[_pos]))
        {
            sb.Append(_input[_pos++]);
        }

        return new Token(TokenType.Number, sb.ToString());
    }

    private Token ReadIdentifier()
    {
        var sb = new StringBuilder();

        while (_pos < _input.Length && char.IsLetter(_input[_pos]))
        {
            sb.Append(_input[_pos++]);
        }

        var value = sb.ToString();

        if (value == "true" || value == "false")
            return new Token(TokenType.Boolean, value);

        return new Token(TokenType.Identifier, value);
    }
}
