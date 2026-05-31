namespace ByteFormat.Core.AST;

public class ValueNode : Node
{
    public object Value { get; }

    public ValueNode(object value)
    {
        Value = value;
    }
}
