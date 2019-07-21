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

        public override bool Execute()
        {
            var stopwatch = Stopwatch.StartNew();
            var root = GitRepoDirectoryFinder.FindForDirectory(ProjectDirectory);
            var config = ConfigReader.Read(root);

            var (readOnly, writeHeader) = ConfigDefaults.Convert(config, ReadOnly, WriteHeader);

            var processor = new DirectoryMarkdownProcessor(
                root,
                log: s => Log.LogMessage(s),
                readOnly: readOnly,
                writeHeader: writeHeader);
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