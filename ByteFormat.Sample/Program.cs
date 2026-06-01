using System;
using ByteFormat.Core;

class Program
{
    static void Main()
    {
        var bf = new ByteFile();
        bf.Path = "example.byte";
        bf.LoadFile();

        int age = 0; bf.Load(ref age, "user.age");
        string name = ""; bf.Load(ref name, "user.username");
        string bio = ""; bf.Load(ref bio, "user.bio");

        Console.WriteLine($"Name: {name}");
        Console.WriteLine($"Age: {age}");
        Console.WriteLine("Bio:\n" + bio);
    }
}
