using System;
using System.IO;
using ByteFormat.Core;

class Program
{
    static void Main()
    {
        const string path = "example.byte";
        EnsureFileExists(path);

        var bf = new ByteFile { Path = path };
        bf.LoadFile();

        CreateInt(bf);
        CreateString(bf);
        CreateBool(bf);
        CreateEnum(bf);
        CreateMultilineString(bf);
        CreateStringArray(bf);
        CreateNumberArray(bf);
        CreateObject(bf);
        CreateNestedObject(bf);
        CreateObjectArray(bf);

        PrintValues(bf);
    }

    static void EnsureFileExists(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, string.Empty);
        }
    }

    static void CreateInt(ByteFile bf)
    {
        bf.Save("demo.intValue", 2026);
    }

    static void CreateString(ByteFile bf)
    {
        bf.Save("demo.stringValue", "hello from byte format");
    }

    static void CreateBool(ByteFile bf)
    {
        bf.Save("demo.isActive", true);
    }

    static void CreateEnum(ByteFile bf)
    {
        bf.Save("demo.status", "enum:ACTIVE");
    }

    static void CreateMultilineString(ByteFile bf)
    {
        bf.Save("demo.notes", "This is a multiline string.\nIt spans multiple lines.\nByteFormat supports it.");
    }

    static void CreateStringArray(ByteFile bf)
    {
        bf.Save("demo.tags[0]", "coding");
        bf.Save("demo.tags[1]", "gameplay");
        bf.Save("demo.tags[2]", "learning");
    }

    static void CreateNumberArray(ByteFile bf)
    {
        bf.Save("demo.scores[0]", 99);
        bf.Save("demo.scores[1]", 87.5);
        bf.Save("demo.scores[2]", 100);
    }

    static void CreateObject(ByteFile bf)
    {
        bf.Save("demo.profile.username", "byte_master");
        bf.Save("demo.profile.age", 22);
        bf.Save("demo.profile.active", true);
    }

    static void CreateNestedObject(ByteFile bf)
    {
        bf.Save("demo.settings.theme", "enum:DARK");
        bf.Save("demo.settings.language", "en");
        bf.Save("demo.settings.notifications", false);
        bf.Save("demo.settings.privacy.share_profile", false);
        bf.Save("demo.settings.privacy.show_online_status", true);
    }

    static void CreateObjectArray(ByteFile bf)
    {
        bf.Save("demo.friends[0].name", "Alex");
        bf.Save("demo.friends[0].status", "enum:ONLINE");
        bf.Save("demo.friends[0].score", 88.2);

        bf.Save("demo.friends[1].name", "Mia");
        bf.Save("demo.friends[1].status", "enum:OFFLINE");
        bf.Save("demo.friends[1].score", 91.0);
    }

    static void PrintValues(ByteFile bf)
    {
        Console.WriteLine("--- ByteFormat test values ---");

        if (bf.TryLoad("demo.intValue", out int intValue))
            Console.WriteLine($"intValue: {intValue}");

        if (bf.TryLoad("demo.stringValue", out string stringValue))
            Console.WriteLine($"stringValue: {stringValue}");

        if (bf.TryLoad("demo.isActive", out bool isActive))
            Console.WriteLine($"isActive: {isActive}");

        if (bf.TryLoad("demo.status", out string status))
            Console.WriteLine($"status: {status}");

        if (bf.TryLoad("demo.notes", out string notes))
            Console.WriteLine($"notes:\n{notes}");

        if (bf.TryLoad("demo.tags[0]", out string tag0))
            Console.WriteLine($"tag0: {tag0}");
        if (bf.TryLoad("demo.tags[1]", out string tag1))
            Console.WriteLine($"tag1: {tag1}");
        if (bf.TryLoad("demo.tags[2]", out string tag2))
            Console.WriteLine($"tag2: {tag2}");

        if (bf.TryLoad("demo.scores[0]", out double score0))
            Console.WriteLine($"score0: {score0}");
        if (bf.TryLoad("demo.scores[1]", out double score1))
            Console.WriteLine($"score1: {score1}");
        if (bf.TryLoad("demo.scores[2]", out double score2))
            Console.WriteLine($"score2: {score2}");

        if (bf.TryLoad("demo.profile.username", out string profileName))
            Console.WriteLine($"profile.username: {profileName}");
        if (bf.TryLoad("demo.profile.age", out int profileAge))
            Console.WriteLine($"profile.age: {profileAge}");
        if (bf.TryLoad("demo.profile.active", out bool profileActive))
            Console.WriteLine($"profile.active: {profileActive}");

        if (bf.TryLoad("demo.settings.theme", out string theme))
            Console.WriteLine($"settings.theme: {theme}");
        if (bf.TryLoad("demo.settings.language", out string language))
            Console.WriteLine($"settings.language: {language}");
        if (bf.TryLoad("demo.settings.notifications", out bool notifications))
            Console.WriteLine($"settings.notifications: {notifications}");
        if (bf.TryLoad("demo.settings.privacy.share_profile", out bool shareProfile))
            Console.WriteLine($"settings.privacy.share_profile: {shareProfile}");
        if (bf.TryLoad("demo.settings.privacy.show_online_status", out bool showOnline))
            Console.WriteLine($"settings.privacy.show_online_status: {showOnline}");

        if (bf.TryLoad("demo.friends[0].name", out string friend0Name))
            Console.WriteLine($"friend0.name: {friend0Name}");
        if (bf.TryLoad("demo.friends[0].status", out string friend0Status))
            Console.WriteLine($"friend0.status: {friend0Status}");
        if (bf.TryLoad("demo.friends[0].score", out double friend0Score))
            Console.WriteLine($"friend0.score: {friend0Score}");

        if (bf.TryLoad("demo.friends[1].name", out string friend1Name))
            Console.WriteLine($"friend1.name: {friend1Name}");
        if (bf.TryLoad("demo.friends[1].status", out string friend1Status))
            Console.WriteLine($"friend1.status: {friend1Status}");
        if (bf.TryLoad("demo.friends[1].score", out double friend1Score))
            Console.WriteLine($"friend1.score: {friend1Score}");
    }
}
