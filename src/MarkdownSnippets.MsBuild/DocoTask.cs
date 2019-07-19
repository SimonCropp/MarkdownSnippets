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

        public bool ReadOnly { get; set; }

        public override bool Execute()
        {
            var stopwatch = Stopwatch.StartNew();
            var root = GitRepoDirectoryFinder.FindForDirectory(ProjectDirectory);
            var processor = new DirectoryMarkdownProcessor(
                root,
                log: s => Log.LogMessage(s),
                readOnly: ReadOnly);
            try
            {
                processor.Run();
                Log.LogMessageFromText($"Finished MarkdownSnippets {stopwatch.ElapsedMilliseconds}ms", MessageImportance.Normal);
            }
            catch (MissingSnippetsException exception)
            {
                var first = exception.Missing.First();
                Log.LogFileError($"MarkdownSnippets: {exception.Message}", first.File, first.Line);
                return false;
            }
            catch (MarkdownProcessingException exception)
            {
                Log.LogFileError($"MarkdownSnippets: {exception.Message}", exception.File, exception.Line);
                return false;
            }

            return true;
        }

        public void Cancel()
        {
        }
    }
}