public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        Downloader.httpClient = new(new FakeHttpHandler());
        // Isolate from the machine wide cache, which may hold real downloads
        Downloader.cache = Path.Combine(Path.GetTempPath(), "MarkdownSnippetsTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Downloader.cache);
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.IgnoreStackTrace();
        VerifierSettings.AddExtraSettings(serializer =>
        {
            var converters = serializer.Converters;
            converters.Add(new ProcessResultConverter());
            converters.Add(new SnippetConverter());
        });
        VerifierSettings.Inline(maxLines: 10, applyMaxLinesToExisting: true);
        VerifierSettings.AddScrubber(_ => _.Replace('\\', '/'));
    }
}
