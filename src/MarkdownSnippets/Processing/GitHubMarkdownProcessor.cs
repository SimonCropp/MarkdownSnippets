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

        public static void RunForFilePath([CallerFilePath] string sourceFilePath = "",bool writeHeader=true)
        {
            Guard.FileExists(sourceFilePath, nameof(sourceFilePath));
            var root = GitRepoDirectoryFinder.FindForFilePath(sourceFilePath);
            Run(root,writeHeader);
        }

        public static void Run(string targetDirectory, bool writeHeader=true)
        {
            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            targetDirectory = Path.GetFullPath(targetDirectory);
            var finder = new FileFinder();
            var findFiles = finder.FindFiles(targetDirectory);
            Run(targetDirectory, findFiles,writeHeader);
        }

        public static void Run(string targetDirectory, List<string> snippetSourceFiles, bool writeHeader=true)
        {
            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            targetDirectory = Path.GetFullPath(targetDirectory);
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            log($"Searching {snippetSourceFiles.Count} files for snippets");
            var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();
            log($"Found {snippets.Count} snippets");
            Run(targetDirectory, snippets, snippetSourceFiles,writeHeader);
        }

        public static void Run(string targetDirectory, List<Snippet> snippets, List<string> snippetSourceFiles, bool writeHeader=true)
        {
            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            var mdFinder = new FileFinder(path => true, IsSourceMd);
            var sourceMdFiles = mdFinder.FindFiles(targetDirectory);
            log($"Found {sourceMdFiles.Count} .source.md files");
            var handling = new GitHubSnippetMarkdownHandling(targetDirectory);
            var processor = new MarkdownProcessor(snippets, handling.AppendGroup, snippetSourceFiles);
            foreach (var sourceFile in sourceMdFiles)
            {
                ProcessFile(sourceFile, processor, targetDirectory,writeHeader);
            }
        }

        static void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor, string rootDirectory, bool writeHeader)
        {
            log($"Processing {sourceFile}");
            var target = GetTargetFile(sourceFile, rootDirectory);
            using (var reader = File.OpenText(sourceFile))
            using (var writer = File.CreateText(target))
            {
                if (writeHeader)
                {
                    HeaderWriter.WriteHeader(sourceFile, rootDirectory, writer);
                }

                var result = markdownProcessor.Apply(reader, writer);
                var missing = result.MissingSnippets;
                if (missing.Any())
                {
                    throw new MissingSnippetsException(missing);
                }
            }
        }

        static string GetTargetFile(string sourceFile, string rootDirectory)
        {
            var relativePath = FileEx.GetRelativePath(sourceFile, rootDirectory);

            var filtered = relativePath.Split(Path.DirectorySeparatorChar)
                .Where(x=>!string.Equals(x, "source", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            var sourceTrimmed = Path.Combine(filtered);
            var targetFile = Path.Combine(rootDirectory, sourceTrimmed);
            // remove ".md" from ".source.md" then change ".source" to ".md"
            targetFile = Path.ChangeExtension(Path.ChangeExtension(targetFile, null), ".md");
            return targetFile;
        }

        static bool IsSourceMd(string path)
        {
            return path.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase);
        }
    }
}