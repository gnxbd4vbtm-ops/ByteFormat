using ByteFormat.Core.AST;
using ByteFormat.Core.Lexer;
using ByteFormat.Core.Parser;

namespace ByteFormat.Core;

public static class ByteFormatEngine
{
    public static ArrayNode Parse(string input)
    {
        var lexer = new Lexer.Lexer(input);
        var tokens = lexer.Tokenize();
        var parser = new Parser.Parser(tokens);
        return parser.Parse();
    }
}public static class ByteFormatFile
{
    public static dynamic Load(string filePath)
    {
        var content = File.ReadAllText(filePath);
        var arrayNode = ByteFormatEngine.Parse(content);
        
        // If the file contains objects, we map the first one to a dynamic object for easy access
        if (arrayNode.Items.Count > 0 && arrayNode.Items[0] is ObjectNode obj)
        {
            return ConvertToObject(obj);
        }

        return arrayNode;
    }

    public static void SaveToByte(string filePath, string key, object value)
    {
        // Simple implementation that creates a single-block object inside an array
        var content = $"[(\n    ({key}: {FormatValue(value)})\n)]";
        File.WriteAllText(filePath, content);
    }

    private static dynamic ConvertToObject(ObjectNode node)
    {
        var expando = new System.Dynamic.ExpandoObject();
        var dict = (IDictionary<string, object>)expando!;

        foreach (var prop in node.Properties)
        {
            dict[prop.Key] = prop.Value switch
            {
                ValueNode v => v.Value,
                ObjectNode o => ConvertToObject(o),
                _ => prop.Value
            };
        }

        return expando;
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            string s => $"\"{s}\"",
            bool b => b.ToString().ToLower(),
            _ => value.ToString() ?? ""
        };
    }
}
