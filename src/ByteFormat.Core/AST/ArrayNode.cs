namespace ByteFormat.Core.AST;

public class ArrayNode : Node
{
    public List<Node> Items { get; } = new();
}
