namespace ByteFormat.Core.AST;

public class BlockNode : Node
{
    public string Key { get; set; } = "";
    public Node Value { get; set; }
}
