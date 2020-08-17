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
        public List<string> Exclude { get; set; } = new List<string>();
        public List<string> TocExcludes { get; set; } = new List<string>();
        public List<string> UrlsAsSnippets { get; set; } = new List<string>();
        public List<string> DocumentExtensions { get; set; } = new List<string>();
        public bool? TreatMissingSnippetAsWarning { get; set; }
        public bool? TreatMissingIncludeAsWarning { get; set; }

        public override bool Execute()
        {
            var stopwatch = Stopwatch.StartNew();
            var root = GitRepoDirectoryFinder.FindForDirectory(ProjectDirectory);
            var (fileConfig, configFilePath) = ConfigReader.Read(root);

            var configResult = ConfigDefaults.Convert(
                fileConfig,
                new ConfigInput
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
                    DocumentExtensions = DocumentExtensions,
                    TreatMissingSnippetAsWarning = TreatMissingSnippetAsWarning,
                    TreatMissingIncludeAsWarning = TreatMissingIncludeAsWarning
                });

            var message = LogBuilder.BuildConfigLogMessage(root, configResult, configFilePath);
            Log.LogMessage(message);

            var processor = new DirectoryMarkdownProcessor(
                root,
                log: s => Log.LogMessage(s),
                readOnly: configResult.ReadOnly,
                directoryFilter: ExcludeToFilterBuilder.ExcludesToFilter(configResult.Exclude),
                writeHeader: configResult.WriteHeader,
                header: configResult.Header,
                urlPrefix: configResult.UrlPrefix,
                linkFormat: configResult.LinkFormat,
                convention: configResult.Convention,
                tocLevel: configResult.TocLevel,
                tocExcludes: configResult.TocExcludes,
                documentExtensions: configResult.DocumentExtensions,
                treatMissingSnippetAsWarning: configResult.TreatMissingSnippetAsWarning,
                treatMissingIncludeAsWarning: configResult.TreatMissingIncludeAsWarning,
                maxWidth: configResult.MaxWidth,
                validateContent: configResult.ValidateContent);

            var snippets = new List<Snippet>();

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
                    if (configResult.TreatMissingSnippetAsWarning)
                    {
                        Log.LogWarning($"MarkdownSnippets: Missing snippet: {missing.Key}", missing.File, missing.LineNumber, 0);
                    }
                    else
                    {
                        Log.LogFileError($"MarkdownSnippets: Missing snippet: {missing.Key}", missing.File, missing.LineNumber, 0);
                    }
                }

                return configResult.TreatMissingSnippetAsWarning;
            }
            catch (MissingIncludesException exception)
            {
                foreach (var missing in exception.Missing)
                {
                    if (configResult.TreatMissingIncludeAsWarning)
                    {
                        Log.LogWarning($"MarkdownSnippets: Missing include: {missing.Key}", missing.File, missing.LineNumber);
                    }
                    else
                    {
                        Log.LogFileError($"MarkdownSnippets: Missing include: {missing.Key}", missing.File, missing.LineNumber, 0);
                    }
                }

                return configResult.TreatMissingIncludeAsWarning;
            }
            catch (ContentValidationException exception)
            {
                foreach (var error in exception.Errors)
                {
                    //TODO: add column
                    Log.LogFileError($"MarkdownSnippets: Content validation: {error.Error}", error.File, error.Line, error.Column);
                }

                return configResult.TreatMissingIncludeAsWarning;
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