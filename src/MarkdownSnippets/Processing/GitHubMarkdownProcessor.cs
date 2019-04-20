using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MarkdownSnippets
{
    public class GitHubMarkdownProcessor
    {
        public bool WriteHeader { set; get; } = true;
        Action<string> log = s => { };
        string targetDirectory;
        List<string> sourceMdFiles = new List<string>();

        public GitHubMarkdownProcessor(string targetDirectory, bool autoScanTargetForMdFiles = true)
        {
            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            this.targetDirectory = Path.GetFullPath(targetDirectory);
            if (autoScanTargetForMdFiles)
            {
                IncludeMdFilesFrom(targetDirectory);
            }
        }

        public void IncludeMdFilesFrom(string directory)
        {
            Guard.DirectoryExists(directory, nameof(directory));
            var mdFinder = new FileFinder(path => true, IsSourceMd);
            sourceMdFiles.AddRange(mdFinder.FindFiles(directory));
            log($"Found {sourceMdFiles.Count} .source.md files");
        }

        public void IncludeMdFiles(params string[] files)
        {
            Guard.AgainstNull(files, nameof(files));
            foreach (var file in files)
            {
                sourceMdFiles.Add(file);
                
            }
        }

        public Action<string> Log
        {
            get => log;
            set
            {
                Guard.AgainstNull(value, nameof(value));
                log = value;
            }
        }

        public static GitHubMarkdownProcessor BuildForForFilePath([CallerFilePath] string sourceFilePath = "")
        {
            Guard.FileExists(sourceFilePath, nameof(sourceFilePath));
            var root = GitRepoDirectoryFinder.FindForFilePath(sourceFilePath);
            return new GitHubMarkdownProcessor(root);
        }

        public void Run()
        {
            var finder = new FileFinder();
            var findFiles = finder.FindFiles(targetDirectory);
            Run(findFiles);
        }

        public void Run(List<string> snippetSourceFiles)
        {
            targetDirectory = Path.GetFullPath(targetDirectory);
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            log($"Searching {snippetSourceFiles.Count} files for snippets");
            var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();
            log($"Found {snippets.Count} snippets");
            Run(snippets, snippetSourceFiles);
        }

        public void Run(List<Snippet> snippets, List<string> snippetSourceFiles)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            var handling = new SnippetMarkdownHandling(targetDirectory);
            var processor = new MarkdownProcessor(snippets, handling.AppendGroup, snippetSourceFiles);
            foreach (var sourceFile in sourceMdFiles)
            {
                ProcessFile(sourceFile, processor);
            }
        }

        void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor)
        {
            log($"Processing {sourceFile}");
            var target = GetTargetFile(sourceFile, targetDirectory);
            using (var reader = File.OpenText(sourceFile))
            using (var writer = File.CreateText(target))
            {
                if (WriteHeader)
                {
                    HeaderWriter.WriteHeader(sourceFile, targetDirectory, writer);
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
                .Where(x => !string.Equals(x, "source", StringComparison.OrdinalIgnoreCase))
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