namespace ByteFormat.Core.Lexer;

public enum TokenType
{
    LBracket,
    RBracket,
    LBrace,
    RBrace,
    LParen,
    RParen,

    Colon,

    Identifier,
    String,
    Number,
    Boolean,

    EOF
}