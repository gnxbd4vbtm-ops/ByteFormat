using System.Collections.Generic;

namespace ByteFormat.Core
{
    public abstract class Node { }
    public sealed class ObjectNode : Node
    {
        public Dictionary<string, Node> Values { get; } = new();
        public bool Has(string key) => Values.ContainsKey(key);
    }
    public sealed class ArrayNode : Node
    {
        public List<Node> Items { get; } = new();
    }
    public sealed class ValueNode : Node
    {
        public string Raw { get; }
        public ValueNode(string raw) { Raw = raw; }
    }
}
