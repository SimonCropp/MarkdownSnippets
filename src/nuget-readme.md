# MarkdownSnippets

A [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) or [MsBuild Task](https://github.com/SimonCropp/MarkdownSnippets/blob/main/docs/msbuild.md) that extract snippets from code files and merges them into markdown documents.

See https://github.com/SimonCropp/MarkdownSnippets for full documentation.

## Behavior

 * Recursively scan the target directory for code files containing snippets.
 * Recursively scan the target directory for markdown (`.md` or `mdx`) files.
 * Merge the snippets into those markdown files.

## Installation

Ensure [dotnet CLI is installed](https://docs.microsoft.com/en-us/dotnet/core/tools/).

Install [MarkdownSnippets.Tool](https://nuget.org/packages/MarkdownSnippets.Tool/)

```
dotnet tool install -g MarkdownSnippets.Tool
```

## Usage

```
mdsnippets C:\Code\TargetDirectory
```

If no directory is passed the current directory will be used, but only if it exists with a git repository directory tree. If not an error is returned.

## Defining Snippets

Any code wrapped in a convention based comment will be picked up. The comment needs to start with `begin-snippet:` which is followed by the key. The snippet is then terminated by `end-snippet`.

```
// begin-snippet: MySnippetName
My Snippet Code
// end-snippet
```

Named [C# regions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-region) will also be picked up, with the name of the region used as the key.

```
#region MySnippetName
My Snippet Code
#endregion
```

## Using Snippets in Markdown

The raw snippet key can be used in any markdown document by subsequent surrounding it with `snippet:`:

```
<!-- snippet: MySnippetName -->
```
