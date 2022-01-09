using MarkdownSnippets;

[UsesVerify]
public class SnippetFileFinderTests
{
    [Fact]
    public Task Nested()
    {
        var directory = Path.Combine(AttributeReader.GetProjectDirectory(), "SnippetFileFinder/Nested");
        var finder = new FileFinder(directory, DocumentConvention.SourceTransform, _ => true, _ => true, _ => true);
        var files = finder.FindFiles();
        return Verify(files.snippetFiles);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.Combine(AttributeReader.GetProjectDirectory(), "SnippetFileFinder/Simple");
        var finder = new FileFinder(directory, DocumentConvention.SourceTransform, _ => true, _ => true, _ => true);
        var files = finder.FindFiles();
        return Verify(files.snippetFiles);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<string>();
        var snippetDirectories = new ConcurrentBag<string>();
        var markdownDirectories = new ConcurrentBag<string>();
        var directory = Path.Combine(AttributeReader.GetProjectDirectory(), "SnippetFileFinder/VerifyLambdasAreCalled");
        var finder = new FileFinder(
            directory,
            DocumentConvention.SourceTransform,
            directoryIncludes: path =>
            {
                directories.Add(path);
                return true;
            },
            markdownDirectoryIncludes: path =>
            {
                markdownDirectories.Add(path);
                return true;
            },
            snippetDirectoryIncludes: path =>
            {
                snippetDirectories.Add(path);
                return true;
            });
        var files = finder.FindFiles();
        return Verify(new
        {
            directories = directories.OrderBy(file => file),
            markdownDirectories = markdownDirectories.OrderBy(file => file),
            snippetDirectories = snippetDirectories.OrderBy(file => file),
        });
    }
}