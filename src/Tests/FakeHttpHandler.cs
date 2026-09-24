

// Serves web snippet and include requests from the local repo, so tests are stable
// and do not depend on the network. Urls of the form
// https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/{branch}/{path}
// map to {repoRoot}/{path}. Anything else is a 404.
class FakeHttpHandler : HttpMessageHandler
{
    const string prefix = "https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/";
    static string repoRoot = GitRepoDirectoryFinder.FindForFilePath();
    static DateTimeOffset lastModified = new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Cancel cancel)
    {
        var path = FindPath(request.RequestUri!.AbsoluteUri);
        if (path == null)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        var content = request.Method == HttpMethod.Head
            ? new ByteArrayContent([])
            : new ByteArrayContent(File.ReadAllBytes(path));
        content.Headers.LastModified = lastModified;
        return Task.FromResult(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            });
    }

    static string? FindPath(string url)
    {
        if (!url.StartsWith(prefix))
        {
            return null;
        }

        var branchAndPath = url.Substring(prefix.Length);
        var slash = branchAndPath.IndexOf('/');
        if (slash < 0)
        {
            return null;
        }

        var path = Path.Combine(repoRoot, branchAndPath.Substring(slash + 1));
        if (File.Exists(path))
        {
            return path;
        }

        return null;
    }
}
