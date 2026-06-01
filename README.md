# ByteFormat

ByteFormat is a lightweight C# library for working with a custom file format designed for simplicity and readability.
It was created as an alternative to formats like JSON, with a focus on easy access and understanding, nested structures, and straightforward saving/loading.

A VS Code extension is also planned to add syntax highlighting and file type support.
---
## Features
- Simple key-based storage system
- Nested data support (e.g. `user.stats.level`)
- Array-like indexing (`list[0]`)
- Supports strings, numbers, and multi-line values
- Easy to use API
---
## Project Structure

ByteFormat/
├── src/
│   └── ByteFormat.Core/        # Core library
├── samples/
│   └── ByteFormat.Sample/      # Example console app
├── ByteFormatPackage/          # NuGet output (.nupkg)

---
## Installation

just run 

`dotnet add package ByteFormat.Core` 

in the root of your dotnet project
after that just refrene the package 
```cs
using ByteFormat.Core;
```

### From Source
Clone the repository and reference the project:
```bash
git clone https://github.com/gnxbd4vbtm-ops/ByteFormat.git

# Then add a project reference in your .csproj:

<ProjectReference Include="src/ByteFormat.Core/ByteFormat.Core.csproj" />
```
⸻

Usage

1. Create and load a ByteFile
```cs
using ByteFormat.Core;
const string path = "path-to-bytefile";
var file = new ByteFile
{
    Path = path
};
file.LoadFile();
```
⸻

2. Saving data
```cs
#Numbers

file.Save("user1.age", 20);
file.Save("user2.age", 16);

#Strings

file.Save("user1.name", "Max");

#Lists

file.Save("user1.hobbys[0]", "playing games");
file.Save("user1.hobbys[1]", "watching YouTube");
file.Save("user1.hobbys[2]", "hanging out with friends");

#Multi-line values

file.Save("user.bio", "this is my bio");
```
⸻

Building the Sample
````bash
cd ByteFormat/samples/ByteFormat.Sample
dotnet run
```

⸻

Building the NuGet Package

A build script is provided:
```bash
./build_package.sh
```
Output:

ByteFormat/ByteFormatPackage/ByteFormat.Core.<version>.nupkg

⸻

Notes

* This library is designed for simple structured storage, not full JSON replacement.
* Keys are string-based and support hierarchical naming.
* Array syntax uses bracket indexing (items[0]).
* This shouldnt be used in deplyoment since its still getting developed

