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
