using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MarkdownSnippets
{
    public class DocoTask :
        Task,
        ICancelableTask
    {
        [Required] public string ProjectDirectory { get; set; } = null!;
        public bool? ReadOnly { get; set; }
        public bool? ValidateContent { get; set; }
        public bool? WriteHeader { get; set; }
        public string? Header { get; set; }
        public string? UrlPrefix { get; set; }
        public int? TocLevel { get; set; }
        public int? MaxWidth { get; set; }
        public LinkFormat? LinkFormat { get; set; }
        public DocumentConvention? Convention { get; set; }
        public List<string> Exclude { get; set; } = new();
        public List<string> TocExcludes { get; set; } = new();
        public List<string> UrlsAsSnippets { get; set; } = new();
        public bool? TreatMissingAsWarning { get; set; }
        public bool? HashSnippetAnchors { get; set; }

        public override bool Execute()
        {
            var stopwatch = Stopwatch.StartNew();
            var root = GitRepoDirectoryFinder.FindForDirectory(ProjectDirectory);
            var (fileConfig, configFilePath) = ConfigReader.Read(root);

            var configResult = ConfigDefaults.Convert(
                fileConfig,
                new()
                {
                    ReadOnly = ReadOnly,
                    ValidateContent = ValidateContent,
                    WriteHeader = WriteHeader,
                    Header = Header,
                    UrlPrefix = UrlPrefix,
                    LinkFormat = LinkFormat,
                    Convention = Convention,
                    Exclude = Exclude,
                    TocExcludes = TocExcludes,
                    TocLevel = TocLevel,
                    MaxWidth = MaxWidth,
                    UrlsAsSnippets = UrlsAsSnippets,
                    TreatMissingAsWarning = TreatMissingAsWarning,
                    HashSnippetAnchors = HashSnippetAnchors,
                });

            var message = LogBuilder.BuildConfigLogMessage(root, configResult, configFilePath);
            Log.LogMessage(message);

            DirectoryMarkdownProcessor processor = new(
                root,
                shouldIncludeDirectory: ExcludeToFilterBuilder.ExcludesToFilter(configResult.Exclude),
                convention: configResult.Convention,
                log: s => Log.LogMessage(s),
                writeHeader: configResult.WriteHeader,
                header: configResult.Header,
                readOnly: configResult.ReadOnly,
                linkFormat: configResult.LinkFormat,
                tocLevel: configResult.TocLevel,
                tocExcludes: configResult.TocExcludes,
                treatMissingAsWarning: configResult.TreatMissingAsWarning,
                maxWidth: configResult.MaxWidth,
                urlPrefix: configResult.UrlPrefix,
                validateContent: configResult.ValidateContent,
                hashSnippetAnchors: configResult.HashSnippetAnchors);

            List<Snippet> snippets = new();

            try
            {
                snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets).GetAwaiter().GetResult();
                processor.AddSnippets(snippets);
                var snippetsInError = processor.Snippets.Where(x => x.IsInError).ToList();
                if (snippetsInError.Any())
                {
                    foreach (var snippet in snippetsInError)
                    {
                        Log.LogFileError($"Snippet error: {snippet.Error}. Key: {snippet.Key}", snippet.Path, snippet.StartLine, 0);
                    }

                    return false;
                }

                processor.Run();
                return true;
            }
            catch (MissingSnippetsException exception)
            {
                foreach (var missing in exception.Missing)
                {
                    if (configResult.TreatMissingAsWarning)
                    {
                        Log.LogWarning($"MarkdownSnippets: Missing snippet: {missing.Key}", missing.File, missing.LineNumber, 0);
                    }
                    else
                    {
                        Log.LogFileError($"MarkdownSnippets: Missing snippet: {missing.Key}", missing.File, missing.LineNumber, 0);
                    }
                }

                return configResult.TreatMissingAsWarning;
            }
            catch (MissingIncludesException exception)
            {
                foreach (var missing in exception.Missing)
                {
                    if (configResult.TreatMissingAsWarning)
                    {
                        Log.LogWarning($"MarkdownSnippets: Missing include: {missing.Key}", missing.File, missing.LineNumber);
                    }
                    else
                    {
                        Log.LogFileError($"MarkdownSnippets: Missing include: {missing.Key}", missing.File, missing.LineNumber, 0);
                    }
                }

                return configResult.TreatMissingAsWarning;
            }
            catch (ContentValidationException exception)
            {
                foreach (var error in exception.Errors)
                {
                    //TODO: add column
                    Log.LogFileError($"MarkdownSnippets: Content validation: {error.Error}", error.File, error.Line, error.Column);
                }

                return configResult.TreatMissingAsWarning;
            }
            catch (MarkdownProcessingException exception)
            {
                Log.LogFileError($"MarkdownSnippets: {exception.Message}", exception.File, exception.LineNumber, 0);
                return false;
            }
            catch (SnippetException exception)
            {
                Log.LogError($"MarkdownSnippets: {exception}");
                return false;
            }
            finally
            {
                Log.LogMessageFromText($"Finished MarkdownSnippets {stopwatch.ElapsedMilliseconds}ms", MessageImportance.Normal);
            }
        }

        public void Cancel()
        {
        }
    }
}