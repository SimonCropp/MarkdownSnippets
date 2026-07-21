static class Downloader
{
    static string cache = Path.Combine(Path.GetTempPath(), "MarkdownSnippets");

    static Downloader()
    {
        Directory.CreateDirectory(cache);
        foreach (var file in new DirectoryInfo(cache)
                     .GetFiles()
                     .OrderByDescending(_ => _.LastWriteTime)
                     .Skip(100))
        {
            try
            {
                file.Delete();
            }
            catch (IOException)
            {
                // The cache is shared by every process on the machine, so another
                // process may be reading this file. Pruning is best effort: leave it
                // for a later run rather than failing the download that triggered it.
            }
        }
    }

    static HttpClient httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static async Task<(bool success, string? path)> DownloadFile(string uri)
    {
        var file = Path.Combine(cache, FileNameFromUrl.ConvertToFileName(uri));

        if (File.Exists(file))
        {
            var fileTimestamp = Timestamp.GetTimestamp(file);
            if (fileTimestamp.Expiry > DateTime.UtcNow)
            {
                return (true, file);
            }
        }

        Timestamp webTimeStamp;
        using (var request = new HttpRequestMessage(HttpMethod.Head, uri))
        {
            using var headResponse = await httpClient.SendAsync(request);
            if (headResponse.StatusCode != HttpStatusCode.OK)
            {
                return (false, null);
            }

            webTimeStamp = Timestamp.GetTimestamp(headResponse);

            if (File.Exists(file))
            {
                var fileTimestamp = Timestamp.GetTimestamp(file);
                if (fileTimestamp.LastModified == webTimeStamp.LastModified)
                {
                    return (true, file);
                }
            }
        }

        // Download to a process-unique temp file and then swap it into place. The cache
        // is shared by every process on the machine, so writing the target directly
        // would hold it exclusively for the duration of the request, and deleting it up
        // front leaves a long window where a concurrent reader sees nothing at all.
        var temp = Path.Combine(cache, $"{Guid.NewGuid():N}.tmp");
        try
        {
            using (var response = await httpClient.GetAsync(uri))
            {
                using var httpStream = await response.Content.ReadAsStreamAsync();
                using (var fileStream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await httpStream.CopyToAsync(fileStream);
                }

                webTimeStamp = Timestamp.GetTimestamp(response);
            }

            Timestamp.SetTimestamp(temp, webTimeStamp);
            await SwapIntoPlace(temp, file);
        }
        finally
        {
            Cleanup(temp);
        }

        return (true, file);
    }

    static async Task SwapIntoPlace(string temp, string target)
    {
        const int maxAttempts = 5;
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                File.Delete(target);
                File.Move(temp, target);
                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                // Another process is reading the same cached url, or swapped its own
                // copy in between the delete and the move. This window is orders of
                // magnitude shorter than the request, so a brief back off clears it.
                await Task.Delay(50 * attempt);
            }
        }
    }

    static void Cleanup(string temp)
    {
        try
        {
            File.Delete(temp);
        }
        catch (IOException)
        {
            // Best effort. A leftover temp file is pruned with the rest of the cache.
        }
    }

    public static async Task<(bool success, string? content)> DownloadContent(string uri)
    {
        var (success, path) = await DownloadFile(uri);
        if (success)
        {
            return (true, await File.ReadAllTextAsync(path!));
        }

        return (false, null);
    }
}