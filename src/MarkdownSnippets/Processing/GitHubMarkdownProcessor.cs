using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MarkdownSnippets
{
    public static class GitHubMarkdownProcessor
    {
        static Action<string> log;

        static GitHubMarkdownProcessor()
        {
            log = s => { };
        }

        public static Action<string> Log
        {
            get => log;
            set
            {
                Guard.AgainstNull(value, nameof(value));
                log = value;
            }
        }

        public static void RunForFilePath([CallerFilePath] string sourceFilePath = "")
        {
            Guard.FileExists(sourceFilePath, nameof(sourceFilePath));
            var root = GitRepoDirectoryFinder.FindForFilePath(sourceFilePath);
            Run(root);
        }

        public static void Run(string targetDirectory)
        {
            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            targetDirectory = Path.GetFullPath(targetDirectory);
            var finder = new FileFinder();
            var findFiles = finder.FindFiles(targetDirectory);
            Run(targetDirectory, findFiles);
        }

        public static void Run(string targetDirectory, List<string> snippetSourceFiles)
        {
            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            targetDirectory = Path.GetFullPath(targetDirectory);
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            log($"Searching {snippetSourceFiles.Count} files for snippets");
            var mdFinder = new FileFinder(path => true, IsSourceMd);
            var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();
            log($"Found {snippets.Count} snippets");
            var handling = new GitHubSnippetMarkdownHandling(targetDirectory);
            var processor = new MarkdownProcessor(snippets, handling.AppendGroup, snippetSourceFiles);
            var sourceMdFiles = mdFinder.FindFiles(targetDirectory);
            log($"Found {sourceMdFiles.Count} .source.md files");
            foreach (var sourceFile in sourceMdFiles)
            {
                ProcessFile(sourceFile, processor, targetDirectory);
            }
        }

        static void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor, string targetDirectory)
        {
            log($"Processing {sourceFile}");
            // remove ".md" from ".source.md" then change ".source" to ".md"
            var target = Path.ChangeExtension(Path.ChangeExtension(sourceFile, null), ".md");
            using (var reader = File.OpenText(sourceFile))
            using (var writer = File.CreateText(target))
            {
                writer.WriteLine(@"<!--");
                writer.WriteLine(@"This file was generate by MarkdownSnippets.");
                writer.WriteLine($@"Source File: {sourceFile.ReplaceCaseless(targetDirectory, "")
                                                            .Replace('\\', '/')}");
                writer.WriteLine(@"To change this file edit the source file and then re-run the generation using either the dotnet global tool (https://github.com/SimonCropp/MarkdownSnippets#githubmarkdownsnippets) or using the api (https://github.com/SimonCropp/MarkdownSnippets#running-as-a-unit-test).");
                writer.WriteLine(@"-->");

                var processResult = markdownProcessor.Apply(reader, writer);
                var missing = processResult.MissingSnippets;
                if (missing.Any())
                {
                    throw new MissingSnippetsException(missing);
                }
            }
        }

        static bool IsSourceMd(string path)
        {
            return path.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase);
        }
    }
}