﻿using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class SnippetFileFinderTests
{
    [Fact]
    public Task Nested()
    {
        var directory = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Nested"));
        FileFinder finder = new(
            directory,
            DocumentConvention.SourceTransform,
            _ => true,
            _ => true,
            _ => true);
        var files = finder.FindFiles();
        return Verifier.Verify(files.snippetFiles);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Simple"));
        FileFinder finder = new(
            directory,
            DocumentConvention.SourceTransform,
            _ => true,
            _ => true,
            _ => true);
        var files = finder.FindFiles();
        return Verifier.Verify(files.snippetFiles);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
    {
        ConcurrentBag<string> directories = new();
        ConcurrentBag<string> snippetDirectories = new();
        ConcurrentBag<string> markdownDirectories = new();
        var directory = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/VerifyLambdasAreCalled"));
        FileFinder finder = new(
            directory,
            DocumentConvention.SourceTransform,
            directoryIncludes: path =>
            {
                directories.Add(path);
                return true;
            },
            markdownDirectoryIncludes: path =>
            {
                markdownDirectories.Add(path);
                return true;
            },
            snippetDirectoryIncludes: path =>
            {
                snippetDirectories.Add(path);
                return true;
            }
        );
        var files = finder.FindFiles();
        return Verifier.Verify(new
        {
            directories = directories.OrderBy(file => file),
            markdownDirectories = markdownDirectories.OrderBy(file => file),
            snippetDirectories = snippetDirectories.OrderBy(file => file),
        });
    }
}