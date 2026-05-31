using ByteFormat.Core.Lexer;

namespace ByteFormat.Core.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _pos;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public void Parse()
    {
        // Placeholder
    }
}
