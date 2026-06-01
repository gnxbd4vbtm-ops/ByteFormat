#!/usr/bin/env sh
set -eu

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

SAMPLE_PROJECT="./samples/ByteFormat.Sample/ByteFormat.Sample.csproj"
if [ ! -f "$SAMPLE_PROJECT" ]; then
  SAMPLE_PROJECT="./ByteFormat.Sample/ByteFormat.Sample.csproj"
fi

EXAMPLE_FILE="./samples/ByteFormat.Sample/example.byte"
if [ ! -f "$EXAMPLE_FILE" ]; then
  EXAMPLE_FILE="./ByteFormat.Sample/example.byte"
fi

echo "Cleaning projects..."
dotnet clean ./src/ByteFormat.Core/ByteFormat.Core.csproj
dotnet clean "$SAMPLE_PROJECT"

echo "Building library and sample in Release..."
dotnet build ./src/ByteFormat.Core/ByteFormat.Core.csproj -c Release
dotnet build "$SAMPLE_PROJECT" -c Release

PACKAGE_DIR="ByteFormatPackage"
PUBLISH_DIR="$PACKAGE_DIR/ByteFormat.Sample"

echo "Preparing package directory: $PACKAGE_DIR"
rm -rf "$PACKAGE_DIR"
mkdir -p "$PUBLISH_DIR"

echo "Packing library..."
dotnet pack ./src/ByteFormat.Core/ByteFormat.Core.csproj -c Release -o "$PACKAGE_DIR"

echo "Publishing sample app..."
dotnet publish "$SAMPLE_PROJECT" -c Release -o "$PUBLISH_DIR"

if [ -f "$EXAMPLE_FILE" ]; then
  echo "Copying example .byte file into package..."
  cp "$EXAMPLE_FILE" "$PACKAGE_DIR/"
fi

echo "Package creation complete. Output available in $PACKAGE_DIR"
