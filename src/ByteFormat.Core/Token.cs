using System;

namespace ByteFormat.Core
{
    internal enum TokenKind { Identifier, String, MultilineString, Number, True, False, EnumMarker, Colon, Equals, LBrace, RBrace, LBracket, RBracket, Comma, Eof }

    internal record Token(TokenKind Kind, string Text);
}
