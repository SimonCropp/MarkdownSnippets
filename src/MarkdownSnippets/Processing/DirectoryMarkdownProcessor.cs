using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class DirectoryMarkdownProcessor
    {
        bool writeHeader;
        string? header;
        DirectoryFilter? directoryFilter;
        bool readOnly;
        int tocLevel;
        IEnumerable<string>? tocExcludes;
        Action<string> log;
        string targetDirectory;
        List<string> sourceMdFiles = new List<string>();
        List<Include> includes = new List<Include>();
        List<Snippet> snippets = new List<Snippet>();
        List<string> snippetSourceFiles = new List<string>();
        AppendSnippetGroupToMarkdown appendSnippetGroup;

        public DirectoryMarkdownProcessor(
            string targetDirectory,
            bool scanForMdFiles = true,
            bool scanForSnippets = true,
            Action<string>? log = null,
            AppendSnippetGroupToMarkdown? appendSnippetGroup = null,
            bool writeHeader = true,
            string? header = null,
            DirectoryFilter? directoryFilter = null,
            bool readOnly = false,
            LinkFormat linkFormat = LinkFormat.GitHub,
            int tocLevel = 2,
            IEnumerable<string>? tocExcludes = null)
        {
            this.writeHeader = writeHeader;
            this.header = header;
            this.directoryFilter = directoryFilter;
            this.readOnly = readOnly;
            this.tocLevel = tocLevel;
            this.tocExcludes = tocExcludes;

            this.appendSnippetGroup = appendSnippetGroup ?? new SnippetMarkdownHandling(targetDirectory, linkFormat).AppendGroup;

            this.log = log ?? (s => { Trace.WriteLine(s); });

            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            this.targetDirectory = Path.GetFullPath(targetDirectory);
            if (scanForMdFiles)
            {
                AddMdFilesFrom(targetDirectory);
            }

            if (scanForSnippets)
            {
                AddSnippetsFrom(targetDirectory);
            }
            AddIncludeFilesFrom(targetDirectory);
        }

        public void AddSnippets(List<Snippet> snippets)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            var files = snippets
                .Where(x => x.Path != null)
                .Select(x => x.Path!)
                .Distinct()
                .ToList();
            snippetSourceFiles.AddRange(files);
            log($"Added {files.Count} files for snippets");
            this.snippets.AddRange(snippets);
            log($"Added {snippets.Count} snippets");
        }

        public void AddSnippets(params Snippet[] snippets)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            AddSnippets(snippets.ToList());
        }

        public void AddSnippetsFrom(string directory)
        {
            directory = ExpandDirectory(directory);
            var finder = new SnippetFileFinder(directoryFilter);
            var files = finder.FindFiles(directory);
            snippetSourceFiles.AddRange(files);
            log($"Searching {files.Count} files for snippets");
            var read = FileSnippetExtractor.Read(files).ToList();
            snippets.AddRange(read);
            log($"Added {read.Count} snippets");
        }

        string ExpandDirectory(string directory)
        {
            Guard.AgainstNull(directory, nameof(directory));
            directory = Path.Combine(targetDirectory, directory);
            directory = Path.GetFullPath(directory);
            Guard.DirectoryExists(directory, nameof(directory));
            return directory;
        }

        public void AddMdFilesFrom(string directory)
        {
            directory = ExpandDirectory(directory);
            var finder = new MdFileFinder(directoryFilter);
            var files = finder.FindFiles(directory).ToList();
            sourceMdFiles.AddRange(files);
            log($"Added {files.Count} .source.md files");
        }

        public void AddIncludeFilesFrom(string directory)
        {
            directory = ExpandDirectory(directory);
            var finder = new IncludeFinder(directoryFilter);
            var toAdd = finder.ReadIncludes(directory).ToList();
            includes.AddRange(toAdd);
            log($"Added {toAdd.Count} .include files");
        }

        public void AddMdFiles(params string[] files)
        {
            Guard.AgainstNull(files, nameof(files));
            foreach (var file in files)
            {
                sourceMdFiles.Add(file);
            }
        }

        public void Run()
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            var processor = new MarkdownProcessor(
                snippets.ToDictionary(),
                includes,
                appendSnippetGroup,
                snippetSourceFiles,
                tocLevel,
                writeHeader,
                header,
                tocExcludes);
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

            var (lines, newLine) = ReadLines(sourceFile);

            var relativeSource = sourceFile
                .Substring(targetDirectory.Length)
                .Replace('\\', '/');
            var result = markdownProcessor.Apply(lines, newLine, relativeSource);
            var missing = result.MissingSnippets;
            if (missing.Any())
            {
                throw new MissingSnippetsException(missing);
            }

            WriteLines(target, lines);

            if (readOnly)
            {
                FileEx.MakeReadOnly(target);
            }
        }

        static void WriteLines(string target, List<Line> lines)
        {
            using var writer = File.CreateText(target);
            foreach (var line in lines)
            {
                writer.WriteLine(line.Current);
            }
        }

        static (List<Line> lines, string newLine) ReadLines(string sourceFile)
        {
            using var reader = File.OpenText(sourceFile);
            return LineReader.ReadAllLines(reader, sourceFile);
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