# Library Usage


## The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/MarkdownSnippets.svg)](https://www.nuget.org/packages/MarkdownSnippets/)

https://nuget.org/packages/MarkdownSnippets/

    PM> Install-Package MarkdownSnippets


## Reading snippets from files

snippet: ReadingFilesSimple


## Reading snippets from a directory structure

snippet: ReadingDirectorySimple


## Full Usage

snippet: markdownProcessingSimple


## Running as a unit test

For the git repository containing the unit test file:

snippet: RunForFilePath

For a specific directory:

snippet: DirectoryMarkdownProcessorRun


## Ignored paths

To change conventions manipulate lists `MarkdownSnippets.Exclusions.ExcludedDirectorySuffixes` and `MarkdownSnippets.Exclusions.ExcludedFileExtensions`.