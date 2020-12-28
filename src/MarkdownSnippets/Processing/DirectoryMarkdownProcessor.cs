using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class DirectoryMarkdownProcessor
    {
        DocumentConvention convention;
        bool writeHeader;
        bool validateContent;
        string? header;
        ShouldIncludeDirectory shouldIncludeDirectory;
        bool readOnly;
        int tocLevel;
        int maxWidth;
        IEnumerable<string>? tocExcludes;
        List<string> documentExtensions;
        Action<string> log;
        string targetDirectory;
        List<string> mdFiles = new();
        List<Include> includes = new();
        List<Snippet> snippets = new();
        public IReadOnlyList<Snippet> Snippets => snippets;
        List<string> snippetSourceFiles = new();
        AppendSnippetsToMarkdown appendSnippets;
        bool treatMissingAsWarning;
        string newLine;
        List<string> allFiles;

        public DirectoryMarkdownProcessor(
            string targetDirectory,
            ShouldIncludeDirectory shouldIncludeDirectory,
            DocumentConvention convention = DocumentConvention.SourceTransform,
            bool scanForMdFiles = true,
            bool scanForSnippets = true,
            bool scanForIncludes = true,
            Action<string>? log = null,
            bool? writeHeader = null,
            string? header = null,
            bool? readOnly = null,
            LinkFormat linkFormat = LinkFormat.GitHub,
            int tocLevel = 2,
            IEnumerable<string>? tocExcludes = null,
            IEnumerable<string>? documentExtensions = null,
            bool treatMissingAsWarning = false,
            int maxWidth = int.MaxValue,
            string? urlPrefix = null,
            bool validateContent = false,
            string? newLine = null,
            bool hashSnippetAnchors = false) :
            this(
                targetDirectory,
                new SnippetMarkdownHandling(targetDirectory, linkFormat, hashSnippetAnchors, urlPrefix).Append,
                shouldIncludeDirectory,
                convention,
                scanForMdFiles: scanForMdFiles,
                scanForSnippets: scanForSnippets,
                scanForIncludes: scanForIncludes,
                log: log,
                writeHeader: writeHeader,
                header: header,
                readOnly: readOnly,
                tocLevel: tocLevel,
                tocExcludes: tocExcludes,
                documentExtensions: documentExtensions,
                treatMissingAsWarning: treatMissingAsWarning,
                maxWidth: maxWidth,
                validateContent: validateContent,
                newLine: newLine)
        {
        }

        public DirectoryMarkdownProcessor(
            string targetDirectory,
            AppendSnippetsToMarkdown appendSnippets,
            ShouldIncludeDirectory shouldIncludeDirectory,
            DocumentConvention convention = DocumentConvention.SourceTransform,
            bool scanForMdFiles = true,
            bool scanForSnippets = true,
            bool scanForIncludes = true,
            Action<string>? log = null,
            bool? writeHeader = null,
            string? header = null,
            bool? readOnly = null,
            int tocLevel = 2,
            IEnumerable<string>? tocExcludes = null,
            IEnumerable<string>? documentExtensions = null,
            bool treatMissingAsWarning = false,
            int maxWidth = int.MaxValue,
            bool validateContent = false,
            string? newLine = null)
        {
            this.appendSnippets = appendSnippets;
            this.convention = convention;
            this.writeHeader = writeHeader.GetValueOrDefault(convention == DocumentConvention.SourceTransform);
            this.readOnly = readOnly.GetValueOrDefault(false);
            this.validateContent = validateContent;
            this.header = header;
            this.shouldIncludeDirectory = shouldIncludeDirectory;
            this.tocLevel = tocLevel;
            this.tocExcludes = tocExcludes;
            this.documentExtensions = MdFileFinder.BuildDefaultExtensions(documentExtensions);
            this.maxWidth = maxWidth;
            this.treatMissingAsWarning = treatMissingAsWarning;

            this.log = log ?? (s => { Trace.WriteLine(s); });

            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            this.targetDirectory = Path.GetFullPath(targetDirectory);

            FileFinder fileFinder = new(targetDirectory, convention,shouldIncludeDirectory);

            allFiles = fileFinder.FindFiles();
            if (scanForMdFiles)
            {
                AddMdFilesFrom(targetDirectory);
            }

            this.newLine = FindNewLine(newLine);
            if (scanForSnippets)
            {
                AddSnippetsFrom(targetDirectory);
            }

            if (scanForIncludes)
            {
                AddIncludeFilesFrom(targetDirectory);
            }
        }

        string FindNewLine(string? newLine)
        {
            if (newLine != null)
            {
                return newLine;
            }

            foreach (var mdFile in mdFiles.OrderBy(x => x.Length))
            {
                using var reader = File.OpenText(mdFile);
                if (reader.TryFindNewline(out newLine))
                {
                    return newLine!;
                }
            }

            var otherMdFiles = Directory.EnumerateFiles(targetDirectory, "*.md", SearchOption.AllDirectories)
                .Where(x => !mdFiles.Contains(x))
                .OrderBy(x => x.Length);
            foreach (var mdFile in otherMdFiles)
            {
                using var reader = File.OpenText(mdFile);
                if (reader.TryFindNewline(out newLine))
                {
                    return newLine!;
                }
            }

            throw new SnippetException("Could not derive new-line string from markdown files.");
        }

        public void AddSnippets(List<Snippet> snippets)
        {
            var stopwatch = Stopwatch.StartNew();
            Guard.AgainstNull(snippets, nameof(snippets));
            var files = snippets
                .Where(x => x.Path != null)
                .Select(x => x.Path!)
                .Distinct()
                .ToList();
            snippetSourceFiles.AddRange(files);
            log($"Added {files.Count} files for snippets");
            this.snippets.AddRange(snippets);
            log($"Added {snippets.Count} snippets ({stopwatch.ElapsedMilliseconds}ms)");
        }

        public void AddSnippets(params Snippet[] snippets)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            AddSnippets(snippets.ToList());
        }

        public void AddSnippetsFrom(string directory)
        {
            var stopwatch = Stopwatch.StartNew();
            directory = ExpandDirectory(directory);
            SnippetFileFinder finder = new(shouldIncludeDirectory);
            var files = finder.FindFiles(directory);
            snippetSourceFiles.AddRange(files);
            log($"Found {files.Count} files for snippets ({stopwatch.ElapsedMilliseconds}ms)");
            stopwatch.Restart();
            var read = FileSnippetExtractor.Read(files, maxWidth, newLine).ToList();
            snippets.AddRange(read);
            log($"Added {read.Count} snippets ({stopwatch.ElapsedMilliseconds}ms)");
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
            var stopwatch = Stopwatch.StartNew();
            directory = ExpandDirectory(directory);
            MdFileFinder finder = new(convention, shouldIncludeDirectory, documentExtensions);
            var files = finder.FindFiles(directory).ToList();
            mdFiles.AddRange(files);
            log($"Added {files.Count} markdown files ({stopwatch.ElapsedMilliseconds}ms)");
        }

        public void AddIncludeFilesFrom(string directory)
        {
            var stopwatch = Stopwatch.StartNew();
            directory = ExpandDirectory(directory);
            IncludeFinder finder = new(shouldIncludeDirectory);
            var toAdd = finder.ReadIncludes(directory).ToList();
            includes.AddRange(toAdd);
            log($"Added {toAdd.Count} .include files ({stopwatch.ElapsedMilliseconds}ms)");
        }

        public void AddMdFiles(params string[] files)
        {
            Guard.AgainstNull(files, nameof(files));
            foreach (var file in files)
            {
                mdFiles.Add(file);
            }
        }

        public void Run()
        {
            Guard.AgainstNull(Snippets, nameof(snippets));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            var stopwatch = Stopwatch.StartNew();
            MarkdownProcessor processor = new(
                convention,
                Snippets.ToDictionary(),
                includes,
                appendSnippets,
                snippetSourceFiles,
                tocLevel,
                writeHeader,
                targetDirectory,
                validateContent,
                header: header,
                tocExcludes: tocExcludes,
                newLine: newLine);
            foreach (var sourceFile in mdFiles)
            {
                ProcessFile(sourceFile, processor);
            }

            log($"MarkdownProcessor Finished. {stopwatch.ElapsedMilliseconds}ms");
        }

        void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor)
        {
            string targetFile;
            if (convention == DocumentConvention.SourceTransform)
            {
                targetFile = TargetFileForSourceTransform(sourceFile, targetDirectory);
            }
            else
            {
                targetFile = sourceFile;
            }

            var lines = ReadLines(sourceFile);

            FileEx.ClearReadOnly(targetFile);

            var relativeSource = sourceFile
                .Substring(targetDirectory.Length)
                .Replace('\\', '/');
            var result = markdownProcessor.Apply(lines, newLine, relativeSource);

            var missingSnippets = result.MissingSnippets;
            if (missingSnippets.Any())
            {
                // If the config value is set to treat missing snippets as warnings, then don't throw
                if (treatMissingAsWarning)
                {
                    foreach (var missing in missingSnippets)
                    {
                        log($"WARN: The source file:{missing.File} includes a key {missing.Key}, however the snippet is missing. Make sure that the snippet is defined.");
                    }
                }
                else
                {
                    throw new MissingSnippetsException(missingSnippets);
                }
            }

            var missingIncludes = result.MissingIncludes;
            if (missingIncludes.Any())
            {
                // If the config value is set to treat missing include as warnings, then don't throw
                if (treatMissingAsWarning)
                {
                    foreach (var missing in missingIncludes)
                    {
                        log($"WARN: The source file:{missing.File} includes a key {missing.Key}, however the include is missing. Make sure that the include is defined.");
                    }
                }
                else
                {
                    throw new MissingIncludesException(missingIncludes);
                }
            }

            var errors = result.ValidationErrors;
            if (errors.Any())
            {
                throw new ContentValidationException(errors);
            }

            WriteLines(targetFile, lines);

            if (readOnly)
            {
                FileEx.MakeReadOnly(targetFile);
            }
        }

        void WriteLines(string target, List<Line> lines)
        {
            using var writer = File.CreateText(target);
            writer.NewLine = newLine;
            foreach (var line in lines)
            {
                writer.WriteLine(line.Current);
            }
        }

        static List<Line> ReadLines(string sourceFile)
        {
            using var reader = File.OpenText(sourceFile);
            return Lines.ReadAllLines(reader, sourceFile).ToList();
        }

        static string TargetFileForSourceTransform(string sourceFile, string rootDirectory)
        {
            var relativePath = FileEx.GetRelativePath(sourceFile, rootDirectory);

            var filtered = relativePath.Split(Path.DirectorySeparatorChar)
                .Where(x => !string.Equals(x, "mdsource", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            var sourceTrimmed = Path.Combine(filtered);
            var targetFile = Path.Combine(rootDirectory, sourceTrimmed);
            // remove ".md" from ".source.md" then change ".source" to ".md"
            targetFile = targetFile.Replace(".source.", ".");
            return targetFile;
        }
    }
}