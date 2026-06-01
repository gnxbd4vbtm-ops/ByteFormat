# ByteFormat

Layout: `src/ByteFormat.Core` (library) and `samples/ByteFormat.Sample` (sample console app).

Usage:

1. Build and run the sample (it references the library by ProjectReference):

```bash
cd /workspaces/ByteFormat/samples/ByteFormat.Sample
dotnet run
```

2. API:
- Create a `ByteFile`, set `Path` to the `.byte` file, then call `Load<T>("path.to.field")` or use the convenience `Load(ref variable, "path.to.field")`.

Examples in `samples/ByteFormat.Sample/Program.cs`.

To build, pack, and copy the release artifacts into `ByteFormatPackage/`, run:

```bash
./build_package.sh
```
