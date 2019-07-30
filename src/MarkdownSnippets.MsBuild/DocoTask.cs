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
        [Required]
        public string ProjectDirectory { get; set; }
        public bool? ReadOnly { get; set; }
        public bool? WriteHeader { get; set; }
        public int? TocLevel { get; set; }
        public LinkFormat? LinkFormat { get; set; }
        public List<string> Exclude { get; set; } = new List<string>();
        public List<string> TocExcludes { get; set; } = new List<string>();
        public List<string> UrlsAsSnippets { get; set; } = new List<string>();

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
                    LinkFormat = LinkFormat,
                    Exclude = Exclude,
                    TocExcludes = TocExcludes,
                    TocLevel = TocLevel,
                    UrlsAsSnippets = UrlsAsSnippets
                });

            var message = LogBuilder.BuildConfigLogMessage(root, configResult, configFilePath);
            Log.LogMessage(message);

            var processor = new DirectoryMarkdownProcessor(
                root,
                log: s => Log.LogMessage(s),
                readOnly: configResult.ReadOnly,
                directoryFilter: ExcludeToFilterBuilder.ExcludesToFilter(configResult.Exclude),
                writeHeader: configResult.WriteHeader,
                linkFormat: configResult.LinkFormat,
                tocLevel: configResult.TocLevel,
                tocExcludes: configResult.TocExcludes);

            var snippets = new List<Snippet>();
            snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets).GetAwaiter().GetResult();
            processor.IncludeSnippets(snippets);

            try
            {
                processor.Run();
                Log.LogMessageFromText($"Finished MarkdownSnippets {stopwatch.ElapsedMilliseconds}ms", MessageImportance.Normal);
            }
            catch (MissingSnippetsException exception)
            {
                var first = exception.Missing.First();
                Log.LogFileError($"MarkdownSnippets: {exception.Message}", first.File, first.LineNumber);
                return false;
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