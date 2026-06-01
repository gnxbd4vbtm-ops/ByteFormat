#!/usr/bin/env sh
set -eu

echo "Cleaning projects..."
dotnet clean ./src/ByteFormat.Core/ByteFormat.Core.csproj
dotnet clean ./samples/ByteFormat.Sample/ByteFormat.Sample.csproj

echo "Building library and sample in Release..."
dotnet build ./src/ByteFormat.Core/ByteFormat.Core.csproj -c Release
dotnet build ./samples/ByteFormat.Sample/ByteFormat.Sample.csproj -c Release

PACKAGE_DIR="ByteFormatPackage"
PUBLISH_DIR="$PACKAGE_DIR/ByteFormat.Sample"

echo "Preparing package directory: $PACKAGE_DIR"
rm -rf "$PACKAGE_DIR"
mkdir -p "$PUBLISH_DIR"

echo "Packing library..."
dotnet pack ./src/ByteFormat.Core/ByteFormat.Core.csproj -c Release -o "$PACKAGE_DIR"

echo "Publishing sample app..."
dotnet publish ./samples/ByteFormat.Sample/ByteFormat.Sample.csproj -c Release -o "$PUBLISH_DIR"

if [ -f "./samples/ByteFormat.Sample/example.byte" ]; then
  echo "Copying example .byte file into package..."
  cp "./samples/ByteFormat.Sample/example.byte" "$PACKAGE_DIR/"
fi

echo "Package creation complete. Output available in $PACKAGE_DIR"
