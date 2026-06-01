using System;
using System.IO;
using ByteFormat.Core;

class Program
{
    static void Main()
    {
        const string path = "roila.byte";
        EnsureFileExists(path);

        var roila = new ByteFile { Path = path };
        roila.LoadFile();
        roila.Clear();
        MakeRoila(roila);
        roila.PrintAll();
    }

    static void MakeRoila(ByteFile roila)
    {
        roila.Save("roila.age", 18);
        roila.Save("roila.name", "Roila");

        roila.Save("roila.hobbys[0]", "being gay");
        roila.Save("roila.hobbys[1]", "being stupid");
        roila.Save("roila.hobbys[2]", "sucking kamo");

        roila.Save("roila.address.city", "idk");
        roila.Save("roila.address.country", "Spain");

        var bio = "pvping since big 21\nLT1 on Mace\nLT2 On Sword\n\n*\"Why oh why does he bleed us dry? He gets away with murder, gets away with murder!\"*";
        roila.Save("roila.bio", bio);
        roila.Save("roila.gender", "Female");
        roila.Save("roila.iq", -17);
    }

    static void EnsureFileExists(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, string.Empty);
        }
    }
}