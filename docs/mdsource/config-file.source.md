# Config File

The [dotnet tool](/MarkdownSnippets#installation) and the [MSBuild Task](msbuild.md) support a config file convention.

Add a file named `mdsnippets.json` at the target directory with the following content:

snippet: sampleConfig.json


## More Info

 * [ReadOnly: Mark resulting files as read only](/#mark-resulting-files-as-read-only)
 * [LinkFormat](/#linkformat)
 * [TocLevel: Heading level](/#heading-level)
 * [TocExcludes: Ignore headings](/#ignore-headings)
 * [Exclude: Exclude directories from snippet discovery](/#exclude-directories-from-snippet-discovery)
 * [WriteHeader: Disable Header](/#disable-header)
 * [UrlPrefix](/#urlprefix)
 * [UrlsAsSnippets: Urls to files to be included as snippets](/#urlsassnippets)
 * TreatMissingSnippetAsWarning: The default behavior for a missing snippet is to log an error (or throw an exception). To change that behavior to a warning set TreatMissingSnippetAsWarning to true.
 * TreatMissingIncludeAsWarning: The default behavior for a missing Include is to log an error (or throw an exception). To change that behavior to a warning set TreatMissingIncludeAsWarning to true.