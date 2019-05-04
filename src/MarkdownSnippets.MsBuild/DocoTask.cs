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
            processor.Run();
            return true;
        }

        public void Cancel()
        {
        }
    }
}