#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
project_path="$script_dir/ByteFormat.Core/ByteFormat.Core.csproj"
output_dir="$script_dir/ByteFormatPackage"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: dotnet is not installed or not on PATH." >&2
  exit 1
fi

mkdir -p "$output_dir"

echo "Packing ByteFormat.Core from '$project_path' into '$output_dir'..."

echo "Removing existing packages from '$output_dir'..."

rm -rf "$output_dir"/*

echo "Existing packages removed."

dotnet pack "$project_path" -c Release -o "$output_dir"

echo "Package build complete. Packages written to '$output_dir'."
