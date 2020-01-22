using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

static class Downloader
{
    static string cache;

    static Downloader()
    {
        cache = Path.Combine(Path.GetTempPath(), "MarkdownSnippets");
        Directory.CreateDirectory(cache);
        foreach (var file in new DirectoryInfo(cache)
            .GetFiles()
            .OrderByDescending(x => x.LastWriteTime)
            .Skip(100))
        {
        }
    }

    static HttpClient httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static async Task<(bool success, string? path)> DownloadFile(string requestUri)
    {
        var tempPath = Path.Combine(cache, FileNameFromUrl.ConvertToFileName(requestUri));

        if (File.Exists(tempPath))
        {
            var fileTimestamp = Timestamp.GetTimestamp(tempPath);
            if (fileTimestamp.Expiry > DateTime.UtcNow)
            {
                return (true, tempPath);
            }
        }

        var request = new HttpRequestMessage(HttpMethod.Head, requestUri);

        Timestamp webTimeStamp;
        using (var headResponse = await httpClient.SendAsync(request))
        {
            if (headResponse.StatusCode != HttpStatusCode.OK)
            {
                return (false, null);
            }

            webTimeStamp = Timestamp.GetTimestamp(headResponse);

            if (File.Exists(tempPath))
            {
                var fileTimestamp = Timestamp.GetTimestamp(tempPath);
                if (fileTimestamp.LastModified == webTimeStamp.LastModified)
                {
                    return (true, tempPath);
                }

                File.Delete(tempPath);
            }
        }

        using (var response = await httpClient.GetAsync(requestUri))
        {
            using var httpStream = await response.Content.ReadAsStreamAsync();
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await httpStream.CopyToAsync(fileStream);
            }

            webTimeStamp = Timestamp.GetTimestamp(response);

            Timestamp.SetTimestamp(tempPath, webTimeStamp);
        }

        return (true, tempPath);
    }

    public static async Task<(bool success, string? content)> DownloadFileContent(string requestUri)
    {
        var (success, path) = await DownloadFile(requestUri);
        if (success)
        {
            return (true, File.ReadAllText(path));
        }
        return (false, null);
    }
}