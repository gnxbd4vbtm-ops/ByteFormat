using ByteFormat.Core.Lexer;
using ByteFormat.Core.AST;

namespace ByteFormat.Core.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _pos;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public ArrayNode Parse()
    {
        Expect(TokenType.LBracket);

        var array = new ArrayNode();

        while (!Match(TokenType.RBracket))
        {
            array.Items.Add(ParseObject());
        }

        return array;
    }

    private ObjectNode ParseObject()
    {
        Expect(TokenType.LBrace);

        var obj = new ObjectNode();

        while (!Match(TokenType.RBrace))
        {
            var block = ParseBlock();
            obj.Properties[block.Key] = block.Value;
        }

        return obj;
    }

    private BlockNode ParseBlock()
    {
        Expect(TokenType.LParen);

        var key = Expect(TokenType.Identifier).Value;

        Expect(TokenType.Colon);

        var value = ParseValue();

        Expect(TokenType.RParen);

        return new BlockNode
        {
            Key = key,
            Value = value
        };
    }

    private Node ParseValue()
    {
        var token = Peek();

        return token.Type switch
        {
            TokenType.String => new ValueNode(Consume().Value),
            TokenType.Number => new ValueNode(int.Parse(Consume().Value)),
            TokenType.Boolean => new ValueNode(bool.Parse(Consume().Value)),

            TokenType.LBrace => ParseObject(),
            TokenType.LBracket => ParseArray(),

            _ => throw new Exception($"Unexpected value: {token.Type}")
        };
    }

    private ArrayNode ParseArray()
    {
        Expect(TokenType.LBracket);

        var arr = new ArrayNode();

        while (!Match(TokenType.RBracket))
        {
            arr.Items.Add(ParseValue());
        }

        return arr;
    }

    // helpers
    private Token Expect(TokenType type)
    {
        var t = _tokens[_pos];

        if (t.Type != type)
            throw new Exception($"Expected {type}, got {t.Type}");

        _pos++;
        return t;
    }

    private bool Match(TokenType type)
    {
        if (_tokens[_pos].Type == type)
        {
            _pos++;
            return true;
        }
        return false;
    }

    private Token Peek() => _tokens[_pos];
    private Token Consume() => _tokens[_pos++];
}