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
        bool readOnly;
        int tocLevel;
        int maxWidth;
        IEnumerable<string>? tocExcludes;
        Action<string> log;
        string targetDirectory;
        List<string> mdFiles = new();
        List<Include> includes = new();
        List<Snippet> snippets = new();
        public IReadOnlyList<Snippet> Snippets => snippets;
        List<string> snippetSourceFiles = new();
        AppendSnippetsToMarkdown appendSnippets;
        bool treatMissingAsWarning;
        string? newLine;
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
            this.tocLevel = tocLevel;
            this.tocExcludes = tocExcludes;
            this.newLine = newLine!;
            this.maxWidth = maxWidth;
            this.treatMissingAsWarning = treatMissingAsWarning;

            this.log = log ?? (s => { Trace.WriteLine(s); });

            Guard.DirectoryExists(targetDirectory, nameof(targetDirectory));
            this.targetDirectory = Path.GetFullPath(targetDirectory);

            FileFinder fileFinder = new(targetDirectory, convention,shouldIncludeDirectory);

            var (snippetFiles, mdFiles, includeFiles, allFiles) = fileFinder.FindFiles();
            this.allFiles = allFiles;
            if (scanForMdFiles)
            {
                this.mdFiles.AddRange(mdFiles);
            }

            if (scanForSnippets)
            {
                InitNewLine();
                snippetSourceFiles.AddRange(snippetFiles);
                this.log($"Found {snippetFiles.Count} files for snippets");
                var stopwatch = Stopwatch.StartNew();
                var read = FileSnippetExtractor.Read(snippetFiles, this.maxWidth, this.newLine!).ToList();
                snippets.AddRange(read);
                this.log($"Added {read.Count} snippets ({stopwatch.ElapsedMilliseconds}ms)");
            }

            if (scanForIncludes)
            {
                foreach (var file in includeFiles)
                {
                    var key = Path.GetFileName(file).Replace(".include.md", "");
                    if (includes.Any(x=>x.Key == key))
                    {
                        throw new($"Duplicate include: {key}");
                    }

                    includes.Add(Include.Build(key, File.ReadAllLines(file), file));
                }
            }
        }

        void InitNewLine()
        {
            if (newLine != null)
            {
                return;
            }

            foreach (var mdFile in mdFiles.OrderBy(x => x.Length))
            {
                using var reader = File.OpenText(mdFile);
                if (reader.TryFindNewline(out newLine))
                {
                    return;
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

            InitNewLine();

            var stopwatch = Stopwatch.StartNew();
            MarkdownProcessor processor = new(
                convention,
                Snippets.ToDictionary(),
                includes,
                appendSnippets,
                snippetSourceFiles,
                allFiles,
                tocLevel,
                writeHeader,
                targetDirectory,
                validateContent,
                header: header,
                tocExcludes: tocExcludes,
                newLine: newLine!);
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
            var result = markdownProcessor.Apply(lines, newLine!, relativeSource);

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