# ByteFormat VS Code Extension

Provides syntax highlighting and a custom file icon for ByteFormat `.byte` files.

What's included
- Syntax grammar tuned to use JSON-like scopes so strings, numbers, and keys get familiar colors in most themes.
- A lightweight file icon for `.byte` files.

Seti icons

This extension can optionally bundle the Seti icon set (the icons used by the built-in `vs-seti` theme). To include those icons in this extension package you must download them separately due to upstream licensing and distribution preferences. See `scripts/fetch-seti-icons.sh` to fetch and copy the Seti icons into `icons/seti/`.

Credits
- Seti icons: The extension can ship the Seti icons if you fetch them using the script. Attribution and license text are placed in `icons/SETI_LICENSE.txt` when you run the fetch script.

Publishing

The extension version has been bumped to `3.0.0`.

If you'd like, I can run a script to fetch the Seti icons and add them into `icons/seti/` for inclusion in the published package — note that running the script requires network access and will clone the upstream repository.
