using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ByteFormat.Core
{
    public class ByteFile
    {
        public string? Path { get; set; }
        private ObjectNode? _root;

        public void LoadFile()
        {
            if (Path == null) throw new InvalidOperationException("Path is null");
            var txt = File.ReadAllText(Path);
            var p = new Parser(txt);
            _root = p.Parse();
        }

        public T? Load<T>(string path)
        {
            if (_root == null) LoadFile();
            if (_root == null) return default;
            if (TryGetNode(path, out var node))
            {
                if (node is ValueNode vn)
                {
                    return ConvertValue<T>(vn.Raw);
                }
                // arrays and objects cannot be converted to T directly; try to convert by string
                return default;
            }
            return default;
        }

        // Convenience: infer T from variable via ref so caller doesn't need explicit generic
        public bool Load<T>(ref T variable, string path)
        {
            var v = Load<T>(path);
            if (v == null) return false;
            variable = v;
            return true;
        }

        public bool TryLoad<T>(string path, out T? value)
        {
            value = default;
            if (_root == null) LoadFile();
            if (_root == null) return false;
            if (TryGetNode(path, out var node))
            {
                if (node is ValueNode vn)
                {
                    value = ConvertValue<T>(vn.Raw);
                    return true;
                }
            }
            return false;
        }

        public bool Save<T>(string path, T value)
        {
            if (_root == null) LoadFile();
            if (_root == null) return false;

            var raw = FormatValue(value);
            if (!TrySetNode(path, new ValueNode(raw))) return false;

            SaveFile();
            return true;
        }

        public bool Save<T>(ref T variable, string path)
        {
            return Save(path, variable);
        }

        public bool Remove(string path)
        {
            if (_root == null) LoadFile();
            if (_root == null) return false;

            if (!TryRemoveNode(path)) return false;

            SaveFile();
            return true;
        }

        public void Clear()
        {
            if (Path == null) throw new InvalidOperationException("Path is null");
            _root = new ObjectNode();
            SaveFile();
        }

        public void SaveFile()
        {
            if (Path == null) throw new InvalidOperationException("Path is null");
            if (_root == null) throw new InvalidOperationException("No data loaded");

            var text = SerializeRoot(_root);
            File.WriteAllText(Path, text);
        }

        private static string FormatValue<T>(T value)
        {
            if (value == null) return "\"\"";

            if (value is string s)
            {
                if (s.Contains('\n'))
                {
                    return "\"\"\"\n" + s.TrimEnd('\r', '\n') + "\n\"\"\"";
                }
                return "\"" + EscapeString(s) + "\"";
            }

            if (value is bool b) return b ? "true" : "false";
            if (value is float || value is double || value is decimal)
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            }
            if (value is int || value is long || value is short || value is byte || value is sbyte ||
                value is uint || value is ulong || value is ushort)
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            }

            return "\"" + EscapeString(value.ToString() ?? string.Empty) + "\"";
        }

        private static string EscapeString(string text)
        {
            return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string SerializeRoot(ObjectNode root)
        {
            var sb = new StringBuilder();
            foreach (var kv in root.Values)
            {
                WriteNode(sb, kv.Key, kv.Value, 0);
            }
            return sb.ToString();
        }

        private static void WriteNode(StringBuilder sb, string key, Node node, int indent)
        {
            var indentText = new string(' ', indent * 2);

            if (node is ValueNode vn)
            {
                sb.Append(indentText).Append(key).Append(" = ").AppendLine(FormatRawValue(vn.Raw));
                return;
            }

            if (node is ArrayNode arr)
            {
                if (arr.Items.Count == 0)
                {
                    sb.Append(indentText).AppendLine(key + " = []");
                    return;
                }

                sb.Append(indentText).AppendLine(key + " = [");
                for (int i = 0; i < arr.Items.Count; i++)
                {
                    var item = arr.Items[i];
                    if (item is ValueNode valueNode)
                    {
                        sb.Append(new string(' ', (indent + 1) * 2)).Append(FormatRawValue(valueNode.Raw));
                    }
                    else if (item is ObjectNode objectNode)
                    {
                        sb.Append(new string(' ', (indent + 1) * 2)).AppendLine("{");
                        WriteObjectBody(sb, objectNode, indent + 2);
                        sb.Append(new string(' ', (indent + 1) * 2)).Append("}");
                    }
                    else
                    {
                        sb.Append(new string(' ', (indent + 1) * 2)).AppendLine();
                    }

                    if (i < arr.Items.Count - 1)
                        sb.AppendLine(",");
                    else
                        sb.AppendLine();
                }
                sb.Append(indentText).AppendLine("]");
                return;
            }

            if (node is ObjectNode obj)
            {
                var allObjects = obj.Values.Count > 0 && obj.Values.Values.All(v => v is ObjectNode);
                if (allObjects)
                {
                    foreach (var inner in obj.Values)
                    {
                        sb.Append(indentText).Append(key).Append(' ').Append(inner.Key).AppendLine(" {");
                        WriteObjectBody(sb, (ObjectNode)inner.Value, indent + 1);
                        sb.Append(indentText).AppendLine("}");
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.Append(indentText).AppendLine(key + " {");
                    WriteObjectBody(sb, obj, indent + 1);
                    sb.Append(indentText).AppendLine("}");
                }
            }
        }

        private static void WriteObjectBody(StringBuilder sb, ObjectNode obj, int indent)
        {
            foreach (var kv in obj.Values)
            {
                WriteNode(sb, kv.Key, kv.Value, indent);
            }
        }

        private bool TrySetNode(string path, Node newNode)
        {
            if (_root == null) return false;

            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            Node cur = _root;
            for (var index = 0; index < parts.Length; index++)
            {
                var part = parts[index];
                var key = part;
                int? idx = null;
                if (part.Contains('['))
                {
                    var br = part.IndexOf('[');
                    var br2 = part.IndexOf(']', br + 1);
                    if (br2 > br)
                    {
                        var inside = part[(br + 1)..br2];
                        if (int.TryParse(inside, out var ii)) idx = ii;
                        key = part[..br];
                    }
                }

                if (cur is not ObjectNode on) return false;

                var isLast = index == parts.Length - 1;
                if (!on.Values.TryGetValue(key, out cur))
                {
                    if (isLast)
                    {
                        if (idx != null)
                        {
                            var arr = new ArrayNode();
                            while (arr.Items.Count <= idx.Value)
                                arr.Items.Add(new ValueNode("\"\""));
                            arr.Items[idx.Value] = newNode;
                            on.Values[key] = arr;
                            return true;
                        }

                        on.Values[key] = newNode;
                        return true;
                    }

                    if (idx != null)
                    {
                        var arr = new ArrayNode();
                        while (arr.Items.Count <= idx.Value)
                        {
                            arr.Items.Add(new ObjectNode());
                        }

                        on.Values[key] = arr;
                        cur = arr.Items[idx.Value];
                    }
                    else
                    {
                        var nextObj = new ObjectNode();
                        on.Values[key] = nextObj;
                        cur = nextObj;
                    }
                }
                else if (idx != null)
                {
                    if (cur is ArrayNode an)
                    {
                        while (an.Items.Count <= idx.Value)
                            an.Items.Add(new ObjectNode());
                        if (isLast)
                        {
                            an.Items[idx.Value] = newNode;
                            return true;
                        }

                        cur = an.Items[idx.Value];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (isLast)
                {
                    on.Values[key] = newNode;
                    return true;
                }
            }

            return false;
        }

        private bool TryRemoveNode(string path)
        {
            if (_root == null) return false;

            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return false;

            Node cur = _root;
            ObjectNode? parent = null;
            string? keyToRemove = null;
            int? removeIndex = null;

            for (var index = 0; index < parts.Length; index++)
            {
                var part = parts[index];
                var key = part;
                int? idx = null;
                if (part.Contains('['))
                {
                    var br = part.IndexOf('[');
                    var br2 = part.IndexOf(']', br + 1);
                    if (br2 > br)
                    {
                        var inside = part[(br + 1)..br2];
                        if (int.TryParse(inside, out var ii)) idx = ii;
                        key = part[..br];
                    }
                }

                if (cur is not ObjectNode on) return false;
                if (!on.Values.TryGetValue(key, out var next)) return false;

                var isLast = index == parts.Length - 1;
                if (isLast)
                {
                    parent = on;
                    keyToRemove = key;
                    removeIndex = idx;
                    break;
                }

                if (idx != null)
                {
                    if (next is ArrayNode an)
                    {
                        if (idx.Value < 0 || idx.Value >= an.Items.Count) return false;
                        cur = an.Items[idx.Value];
                    }
                    else return false;
                }
                else
                {
                    cur = next;
                }
            }

            if (parent == null || keyToRemove == null) return false;
            if (removeIndex == null)
            {
                return parent.Values.Remove(keyToRemove);
            }

            if (!parent.Values.TryGetValue(keyToRemove, out var node)) return false;
            if (node is not ArrayNode array) return false;
            if (removeIndex.Value < 0 || removeIndex.Value >= array.Items.Count) return false;
            array.Items.RemoveAt(removeIndex.Value);
            if (array.Items.Count == 0) parent.Values.Remove(keyToRemove);
            return true;
        }

        private static string FormatRawValue(string raw)
        {
            if (raw.StartsWith("enum:")) return raw;
            if (bool.TryParse(raw, out _)) return raw;
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out _)) return raw;
            if (raw.Contains("\n")) return "\"\"\"\n" + raw.TrimEnd('\r', '\n') + "\n\"\"\"";
            return "\"" + EscapeString(raw) + "\"";
        }

        private static T? ConvertValue<T>(string raw)
        {
            var t = typeof(T);
            if (t == typeof(string)) return (T?)(object)raw;
            if (t == typeof(int))
            {
                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)) return (T?)(object)i;
                if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)) return (T?)(object)(int)d;
            }
            if (t == typeof(double))
            {
                if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)) return (T?)(object)d;
            }
            if (t == typeof(bool))
            {
                if (bool.TryParse(raw, out var b)) return (T?)(object)b;
            }
            // enum:NAME -> return string
            if (raw.StartsWith("enum:") && t == typeof(string)) return (T?)(object)raw.Substring(5);
            return default;
        }

        private bool TryGetNode(string path, out Node? node)
        {
            node = null;
            if (_root == null) return false;
            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            Node cur = _root;
            foreach (var part in parts)
            {
                // support index like name[0]
                var p = part;
                int? idx = null;
                if (p.Contains('['))
                {
                    var br = p.IndexOf('[');
                    var br2 = p.IndexOf(']', br+1);
                    if (br2>br)
                    {
                        var inside = p[(br+1)..br2];
                        if (int.TryParse(inside, out var ii)) idx = ii;
                        p = p[..br];
                    }
                }
                if (cur is ObjectNode on)
                {
                    if (!on.Values.TryGetValue(p, out cur)) return false;
                }
                else return false;
                if (idx != null)
                {
                    if (cur is ArrayNode an)
                    {
                        if (idx.Value < 0 || idx.Value >= an.Items.Count) return false;
                        cur = an.Items[idx.Value];
                    }
                    else return false;
                }
            }
            node = cur; return true;
        }
    }
}
