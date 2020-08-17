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
        DirectoryFilter? directoryFilter;
        bool readOnly;
        int tocLevel;
        int maxWidth;
        IEnumerable<string>? tocExcludes;
        List<string> documentExtensions;
        Action<string> log;
        string targetDirectory;
        List<string> sourceMdFiles = new List<string>();
        List<Include> includes = new List<Include>();
        List<Snippet> snippets = new List<Snippet>();
        public IReadOnlyList<Snippet> Snippets => snippets;
        List<string> snippetSourceFiles = new List<string>();
        AppendSnippetGroupToMarkdown appendSnippetGroup;
        bool treatMissingAsWarning;

        public DirectoryMarkdownProcessor(
            string targetDirectory,
            DocumentConvention convention = DocumentConvention.SourceTransform,
            bool scanForMdFiles = true,
            bool scanForSnippets = true,
            bool scanForIncludes = true,
            Action<string>? log = null,
            bool? writeHeader = null,
            string? header = null,
            DirectoryFilter? directoryFilter = null,
            bool? readOnly = null,
            LinkFormat linkFormat = LinkFormat.GitHub,
            int tocLevel = 2,
            IEnumerable<string>? tocExcludes = null,
            IEnumerable<string>? documentExtensions = null,
            bool treatMissingAsWarning = false,
            int maxWidth = int.MaxValue,
            string? urlPrefix = null,
            bool validateContent = false) :
            this(
                targetDirectory,
                new SnippetMarkdownHandling(targetDirectory, linkFormat, urlPrefix).AppendGroup,
                convention,
                scanForMdFiles,
                scanForSnippets,
                scanForIncludes,
                log,
                writeHeader,
                header,
                directoryFilter,
                readOnly,
                tocLevel,
                tocExcludes,
                documentExtensions,
                treatMissingAsWarning,
                maxWidth,
                validateContent)
        {
        }

        public DirectoryMarkdownProcessor(
            string targetDirectory,
            AppendSnippetGroupToMarkdown appendSnippetGroup,
            DocumentConvention convention = DocumentConvention.SourceTransform,
            bool scanForMdFiles = true,
            bool scanForSnippets = true,
            bool scanForIncludes = true,
            Action<string>? log = null,
            bool? writeHeader = null,
            string? header = null,
            DirectoryFilter? directoryFilter = null,
            bool? readOnly = null,
            int tocLevel = 2,
            IEnumerable<string>? tocExcludes = null,
            IEnumerable<string>? documentExtensions = null,
            bool treatMissingAsWarning = false,
            int maxWidth = int.MaxValue,
            bool validateContent = false)
        {
            this.appendSnippetGroup = appendSnippetGroup;
            this.convention = convention;
            this.writeHeader = writeHeader.GetValueOrDefault(convention == DocumentConvention.SourceTransform);
            this.readOnly = readOnly.GetValueOrDefault(convention == DocumentConvention.SourceTransform);
            this.validateContent = validateContent;
            this.header = header;
            this.directoryFilter = directoryFilter;
            this.tocLevel = tocLevel;
            this.tocExcludes = tocExcludes;
            this.documentExtensions = MdFileFinder.BuildDefaultExtensions(documentExtensions);
            this.maxWidth = maxWidth;
            this.treatMissingAsWarning = treatMissingAsWarning;

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

            if (scanForIncludes)
            {
                AddIncludeFilesFrom(targetDirectory);
            }
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
            var read = FileSnippetExtractor.Read(files, maxWidth).ToList();
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
            var finder = new MdFileFinder(convention, directoryFilter, documentExtensions);
            var files = finder.FindFiles(directory).ToList();
            sourceMdFiles.AddRange(files);
            log($"Added {files.Count} markdown files");
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
            Guard.AgainstNull(Snippets, nameof(snippets));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            var processor = new MarkdownProcessor(
                convention,
                Snippets.ToDictionary(),
                includes,
                appendSnippetGroup,
                snippetSourceFiles,
                tocLevel,
                writeHeader,
                targetDirectory,
                validateContent,
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
            return Lines.ReadAllLines(reader, sourceFile);
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
            targetFile = targetFile.Replace(".source.", ".");
            return targetFile;
        }
    }
}