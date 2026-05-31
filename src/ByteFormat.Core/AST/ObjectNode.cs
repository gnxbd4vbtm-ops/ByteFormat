namespace ByteFormat.Core.AST;

public class ObjectNode : Node
{
    public Dictionary<string, Node> Properties { get; } = new();
}