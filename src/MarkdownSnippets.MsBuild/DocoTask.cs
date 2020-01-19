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
        public bool? WriteHeader { get; set; }
        public string? Header { get; set; }
        public string? UrlPrefix { get; set; }
        public int? TocLevel { get; set; }
        public int? MaxWidth { get; set; }
        public LinkFormat? LinkFormat { get; set; }
        public List<string> Exclude { get; set; } = new List<string>();
        public List<string> TocExcludes { get; set; } = new List<string>();
        public List<string> UrlsAsSnippets { get; set; } = new List<string>();
        public bool? TreatMissingSnippetAsWarning { get; set; }

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
                    WriteHeader = WriteHeader,
                    Header = Header,
                    UrlPrefix = UrlPrefix,
                    LinkFormat = LinkFormat,
                    Exclude = Exclude,
                    TocExcludes = TocExcludes,
                    TocLevel = TocLevel,
                    MaxWidth = MaxWidth,
                    UrlsAsSnippets = UrlsAsSnippets,
                    TreatMissingSnippetAsWarning = TreatMissingSnippetAsWarning
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
                tocLevel: configResult.TocLevel,
                tocExcludes: configResult.TocExcludes,
                treatMissingSnippetAsWarning: configResult.TreatMissingSnippetAsWarning,
                maxWidth: configResult.MaxWidth);

            var snippets = new List<Snippet>();
            snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets).GetAwaiter().GetResult();
            processor.AddSnippets(snippets);

            try
            {
                var snippetsInError = processor.Snippets.Where(x => x.IsInError).ToList();
                if (snippetsInError.Any())
                {
                    foreach (var snippet in snippetsInError)
                    {
                        Log.LogFileError($"Snippet error: {snippet.Error}. Key: {snippet.Key}", snippet.Path, snippet.StartLine);
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
                        Log.LogWarning($"MarkdownSnippet: Missing: {missing.Key}", missing.File, missing.LineNumber);
                    }
                    else
                    {
                        Log.LogFileError($"MarkdownSnippet: Missing: {missing.Key}", missing.File, missing.LineNumber);
                    }
                }

                return configResult.TreatMissingSnippetAsWarning;
            }
            catch (MarkdownProcessingException exception)
            {
                Log.LogFileError($"MarkdownSnippets: {exception.Message}", exception.File, exception.LineNumber);
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