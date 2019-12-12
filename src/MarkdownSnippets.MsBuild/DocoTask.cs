using System.Collections.Generic;
using System.Diagnostics;
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
        public int? TocLevel { get; set; }
        public LinkFormat? LinkFormat { get; set; }
        public List<string> Exclude { get; set; } = new List<string>();
        public List<string> TocExcludes { get; set; } = new List<string>();
        public List<string> UrlsAsSnippets { get; set; } = new List<string>();
        public bool? TreatMissingSnippetsAsErrors { get; set; }

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
                    LinkFormat = LinkFormat,
                    Exclude = Exclude,
                    TocExcludes = TocExcludes,
                    TocLevel = TocLevel,
                    UrlsAsSnippets = UrlsAsSnippets,
                    TreatMissingSnippetsAsErrors = TreatMissingSnippetsAsErrors
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
                linkFormat: configResult.LinkFormat,
                tocLevel: configResult.TocLevel,
                tocExcludes: configResult.TocExcludes,
                treatMissingSnippetsAsErrors: configResult.TreatMissingSnippetsAsErrors);

            var snippets = new List<Snippet>();
            snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets).GetAwaiter().GetResult();
            processor.AddSnippets(snippets);

            try
            {
                processor.Run();
                Log.LogMessageFromText($"Finished MarkdownSnippets {stopwatch.ElapsedMilliseconds}ms", MessageImportance.Normal);
            }
            catch (MissingSnippetsException exception)
            {
                
                foreach (var missing in exception.Missing)
                {
                    if (configResult.TreatMissingSnippetsAsErrors)
                    {
                        Log.LogFileError($"MarkdownSnippet: Missing: {missing.Key}", missing.File, missing.LineNumber);

                    }
                    else
                    {
                        Log.LogWarning($"MarkdownSnippet: Missing: {missing.Key}", missing.File, missing.LineNumber);
                    }
                }

                return !configResult.TreatMissingSnippetsAsErrors;
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

            return true;
        }

        public void Cancel()
        {
        }
    }
}