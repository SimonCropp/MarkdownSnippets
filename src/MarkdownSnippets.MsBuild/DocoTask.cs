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

        public override bool Execute()
        {
            var root = GitRepoDirectoryFinder.FindForDirectory(ProjectDirectory);
            var processor = new DirectoryMarkdownProcessor(root, log: s => Log.LogMessage(s));
            try
            {
                processor.Run();
            }
            catch (MissingSnippetsException exception)
            {
                Log.LogError($"MarkdownSnippets: {exception.Message}");
                return false;
            }
            catch (MarkdownProcessingException exception)
            {
                Log.LogError($"MarkdownSnippets: {exception.Message}");
                return false;
            }
            
            return true;
        }

        public void Cancel()
        {
        }
    }
}