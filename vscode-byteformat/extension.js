const vscode = require('vscode');

const completionItems = [
  new vscode.CompletionItem('true', vscode.CompletionItemKind.Keyword),
  new vscode.CompletionItem('false', vscode.CompletionItemKind.Keyword),
  new vscode.CompletionItem('key = value', vscode.CompletionItemKind.Snippet),
  new vscode.CompletionItem('key = [ ... ]', vscode.CompletionItemKind.Snippet),
  new vscode.CompletionItem('key = { ... }', vscode.CompletionItemKind.Snippet),
  new vscode.CompletionItem('multiline string', vscode.CompletionItemKind.Snippet)
];

completionItems[2].insertText = new vscode.SnippetString('${1:key} = ${2:value}');
completionItems[2].detail = 'ByteFormat assignment';
completionItems[2].documentation = 'Insert a ByteFormat key/value assignment.';

completionItems[3].insertText = new vscode.SnippetString('${1:key} = [\n\t$0\n]');
completionItems[3].detail = 'ByteFormat list';
completionItems[3].documentation = 'Insert a ByteFormat list structure.';

completionItems[4].insertText = new vscode.SnippetString('${1:key} = {\n\t$0\n}');
completionItems[4].detail = 'ByteFormat object';
completionItems[4].documentation = 'Insert a ByteFormat nested object.';

completionItems[5].insertText = new vscode.SnippetString('${1:key} = """\n$0\n"""');
completionItems[5].detail = 'ByteFormat multi-line string';
completionItems[5].documentation = 'Insert a ByteFormat multi-line string block.';

function activate(context) {
  const provider = vscode.languages.registerCompletionItemProvider(
    'byte',
    {
      provideCompletionItems(document, position) {
        const linePrefix = document.lineAt(position).text.substr(0, position.character);
        if (/\b(enum:)?[A-Za-z0-9_]*$/.test(linePrefix)) {
          return completionItems;
        }
        return undefined;
      }
    },
    '"', '{', '[', '='
  );

  context.subscriptions.push(provider);
}

function deactivate() {}

module.exports = {
  activate,
  deactivate
};
