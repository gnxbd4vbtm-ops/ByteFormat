namespace ByteFormat.Core.AST;

public class BlockNode : Node
{
    public string Key { get; set; } = string.Empty;
    public Node Value { get; set; } = null!;
}