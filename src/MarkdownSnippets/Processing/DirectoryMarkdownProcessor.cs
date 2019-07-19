using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MarkdownSnippets
{
    public class DirectoryMarkdownProcessor
    {
        bool writeHeader;
        DirectoryFilter directoryFilter;
        bool readOnly;
        Action<string> log;
        string targetDirectory;
        List<string> sourceMdFiles = new List<string>();
        List<Snippet> snippets = new List<Snippet>();
        List<string> snippetSourceFiles = new List<string>();
        AppendSnippetGroupToMarkdown appendSnippetGroup;

        public DirectoryMarkdownProcessor(
            string targetDirectory,
            bool scanForMdFiles = true,
            bool scanForSnippets = true,
            Action<string> log = null,
            AppendSnippetGroupToMarkdown appendSnippetGroup = null,
            bool writeHeader = true,
            DirectoryFilter directoryFilter = null,
            bool readOnly = true)
        {
            this.writeHeader = writeHeader;
            this.directoryFilter = directoryFilter;
            this.readOnly = readOnly;
            if (appendSnippetGroup == null)
            {
                this.appendSnippetGroup = new SnippetMarkdownHandling(targetDirectory).AppendGroup;
            }
            else
            {
                this.appendSnippetGroup = appendSnippetGroup;
            }

            if (log == null)
            {
                this.log = s => { Trace.WriteLine(s); };
            }
            else
            {
                this.log = log;
            }

            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            this.targetDirectory = Path.GetFullPath(targetDirectory);
            if (scanForMdFiles)
            {
                IncludeMdFilesFrom(targetDirectory);
            }

            if (scanForSnippets)
            {
                IncludeSnippetsFrom(targetDirectory);
            }
        }

        public void IncludeSnippets(List<Snippet> snippets)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            var files = snippets
                .Select(x => x.Path)
                .Where(x => x != null)
                .Distinct()
                .ToList();
            snippetSourceFiles.AddRange(files);
            log($"Added {files.Count} files for snippets");
            this.snippets.AddRange(snippets);
            log($"Added {snippets.Count} snippets");
        }

        public void IncludeSnippets(params Snippet[] snippets)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            IncludeSnippets(snippets.ToList());
        }

        public void IncludeSnippetsFrom(string directory)
        {
            Guard.AgainstNull(directory, nameof(directory));
            directory = Path.Combine(targetDirectory, directory);
            directory = Path.GetFullPath(directory);
            Guard.DirectoryExists(directory, nameof(directory));
            var finder = new SnippetFileFinder(directoryFilter);
            var files = finder.FindFiles(directory);
            snippetSourceFiles.AddRange(files);
            log($"Searching {files.Count} files for snippets");
            var read = FileSnippetExtractor.Read(files).ToList();
            snippets.AddRange(read);
            log($"Added {read.Count} snippets");
        }

        public void IncludeMdFilesFrom(string directory)
        {
            Guard.DirectoryExists(directory, nameof(directory));
            var mdFinder = new MdFileFinder(directoryFilter);
            var files = mdFinder.FindFiles(directory).ToList();
            sourceMdFiles.AddRange(files);
            log($"Added {files.Count} .source.md files");
        }

        public void IncludeMdFiles(params string[] files)
        {
            Guard.AgainstNull(files, nameof(files));
            foreach (var file in files)
            {
                sourceMdFiles.Add(file);
            }
        }

        public static void RunForFilePath([CallerFilePath] string sourceFilePath = "")
        {
            Guard.FileExists(sourceFilePath, nameof(sourceFilePath));
            var root = GitRepoDirectoryFinder.FindForFilePath(sourceFilePath);
            new DirectoryMarkdownProcessor(root).Run();
        }

        public void Run()
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            var processor = new MarkdownProcessor(snippets, appendSnippetGroup, snippetSourceFiles);
            foreach (var sourceFile in sourceMdFiles)
            {
                ProcessFile(sourceFile, processor);
            }
        }

        void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor)
        {
            log($"Processing {sourceFile}");
            var target = GetTargetFile(sourceFile, targetDirectory);
            FileEx.ClearReadOnly(target);
            using (var reader = File.OpenText(sourceFile))
            using (var writer = File.CreateText(target))
            {
                if (writeHeader)
                {
                    HeaderWriter.WriteHeader(sourceFile, targetDirectory, writer);
                }

                var result = markdownProcessor.Apply(reader, writer,sourceFile);
                var missing = result.MissingSnippets;
                if (missing.Any())
                {
                    throw new MissingSnippetsException(missing);
                }
            }

            if (readOnly)
            {
                FileEx.MakeReadOnly(target);
            }
        }

        static string GetTargetFile(string sourceFile, string rootDirectory)
        {
            var relativePath = FileEx.GetRelativePath(sourceFile, rootDirectory);

            var filtered = relativePath.Split(Path.DirectorySeparatorChar)
                .Where(x => !string.Equals(x, "mdsource", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            var sourceTrimmed = Path.Combine(filtered);
            var targetFile = Path.Combine(rootDirectory, sourceTrimmed);
            // remove ".md" from ".source.md" then change ".source" to ".md"
            targetFile = Path.ChangeExtension(Path.ChangeExtension(targetFile, null), ".md");
            return targetFile;
        }
    }
}