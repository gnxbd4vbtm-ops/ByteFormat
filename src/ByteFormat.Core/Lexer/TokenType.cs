namespace ByteFormat.Core.Lexer;

public enum TokenType
{
    LeftBracket,   // [
    RightBracket,  // ]
    LeftBrace,     // {
    RightBrace,    // }
    LeftParen,     // (
    RightParen,    // )

    Identifier,
    Number,
    String,
    Boolean,

    Colon,
    EOF
}
