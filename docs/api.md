<!--
This file was generate by MarkdownSnippets.
Source File: /docs/api.source.md
To change this file edit the source file and then re-run the generation using either the dotnet global tool (https://github.com/SimonCropp/MarkdownSnippets#markdownsnippetstool) or using the api (https://github.com/SimonCropp/MarkdownSnippets#running-as-a-unit-test).
-->
# Library Usage


## The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/MarkdownSnippets.svg)](https://www.nuget.org/packages/MarkdownSnippets/)

https://nuget.org/packages/MarkdownSnippets/

    PM> Install-Package MarkdownSnippets


## Reading snippets from files

<!-- snippet: ReadingFilesSimple -->
```cs
var files = Directory.EnumerateFiles(@"C:\path", "*.cs", SearchOption.AllDirectories);

var snippets = FileSnippetExtractor.Read(files);
```
<sup>[snippet source](/src/Tests/Snippets/Usage.cs#L8-L14)</sup>
<!-- endsnippet -->


## Reading snippets from a directory structure

<!-- snippet: ReadingDirectorySimple -->
```cs
// extract snippets from files
var snippetExtractor = new DirectorySnippetExtractor(
    // all directories except bin and obj
    directoryFilter: dirPath => !dirPath.EndsWith("bin") && !dirPath.EndsWith("obj"),
    // all js and cs files
    fileFilter: filePath => filePath.EndsWith(".js") || filePath.EndsWith(".cs"));
var snippets = snippetExtractor.ReadSnippets(@"C:\path");
```
<sup>[snippet source](/src/Tests/Snippets/Usage.cs#L38-L48)</sup>
<!-- endsnippet -->


## Full Usage

<!-- snippet: markdownProcessingSimple -->
```cs
// setup version convention and extract snippets from files
var snippetExtractor = new DirectorySnippetExtractor(
    directoryFilter: x => true,
    fileFilter: s => s.EndsWith(".js") || s.EndsWith(".cs"));
var snippets = snippetExtractor.ReadSnippets(@"C:\path");

// Merge with some markdown text
var markdownProcessor = new MarkdownProcessor(
    snippets: snippets,
    appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup);

using (var reader = File.OpenText(@"C:\path\inputMarkdownFile.md"))
using (var writer = File.CreateText(@"C:\path\outputMarkdownFile.md"))
{
    var result = markdownProcessor.Apply(reader, writer);
    // snippets that the markdown file expected but did not exist in the input snippets
    var missingSnippets = result.MissingSnippets;

    // snippets that the markdown file used
    var usedSnippets = result.UsedSnippets;
}
```
<sup>[snippet source](/src/Tests/Snippets/Usage.cs#L53-L77)</sup>
<!-- endsnippet -->


## Running as a unit test

For the git repository containing the unit test file:

<!-- snippet: RunForFilePath -->
```cs
DirectoryMarkdownProcessor.RunForFilePath();
```
<sup>[snippet source](/src/Tests/Snippets/Usage.cs#L19-L23)</sup>
<!-- endsnippet -->

For a specific directory:

<!-- snippet: DirectoryMarkdownProcessorRun -->
```cs
var processor = new DirectoryMarkdownProcessor("targetDirectory");
processor.Run();
```
<sup>[snippet source](/src/Tests/Snippets/Usage.cs#L28-L33)</sup>
<!-- endsnippet -->


## Ignored paths

To change conventions manipulate lists `MarkdownSnippets.Exclusions.ExcludedDirectorySuffixes` and `MarkdownSnippets.Exclusions.ExcludedFileExtensions`.
