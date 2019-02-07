using System;
using System.IO;
using System.Linq;
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
            file.Delete();
        }
    }

    static HttpClient httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static async Task<string> DownloadFile(string requestUri)
    {
        var path = Path.Combine(cache, FileNameFromUrl.ConvertToFileName(requestUri));

        if (File.Exists(path))
        {
            var fileTimestamp = Timestamp.GetTimestamp(path);
            if (fileTimestamp.Expiry > DateTime.UtcNow)
            {
                return File.ReadAllText(path);
            }
        }

        var requestMessage = new HttpRequestMessage(HttpMethod.Head, requestUri);

        Timestamp webTimeStamp;
        using (var headResponse = await httpClient.SendAsync(requestMessage).ConfigureAwait(false))
        {
            webTimeStamp = Timestamp.GetTimestamp(headResponse);

            if (File.Exists(path))
            {
                var fileTimestamp = Timestamp.GetTimestamp(path);
                if (fileTimestamp.LastModified == webTimeStamp.LastModified)
                {
                    return File.ReadAllText(path);
                }

                File.Delete(path);
            }
        }

        using (var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false))
        using (var httpStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }

            webTimeStamp = Timestamp.GetTimestamp(response);

            Timestamp.SetTimestamp(path, webTimeStamp);
        }

        return File.ReadAllText(path);
    }
}