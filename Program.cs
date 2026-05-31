using ByteFormat.Core;

var text = File.ReadAllText("examples/sample.byte");

var result = ByteFormatEngine.Parse(text);

foreach (var obj in result.Items)
{
    var o = obj as ByteFormat.Core.AST.ObjectNode;

    var name = ((ByteFormat.Core.AST.ValueNode)o.Properties["name"]).Value;
    var age = ((ByteFormat.Core.AST.ValueNode)o.Properties["age"]).Value;
    var active = ((ByteFormat.Core.AST.ValueNode)o.Properties["active"]).Value;

    Console.WriteLine($"{name} | {age} | {active}");
}