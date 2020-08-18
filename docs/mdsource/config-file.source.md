# Config File

The [dotnet tool](/readme.md#installation) and the [MSBuild Task](msbuild.md) support a config file convention.

Add a file named `mdsnippets.json` at the target directory with the following content:

snippet: sampleConfig.json


## More Info

 * [ReadOnly: Mark resulting files as read only](/readme.md#mark-resulting-files-as-read-only)
 * [LinkFormat](/readme.md#linkformat).
 * [Convention](/readme.md#document-convention).
 * [TocLevel: Heading level](/docs/toc.md#heading-level).
 * [TocExcludes: Ignore headings](/docs/toc.md#ignore-headings).
 * [Exclude: Exclude directories from snippet discovery](/docs/snippet-exclusion.md).
 * [WriteHeader: Disable Header](/docs/header.md#disable-header).
 * [UrlPrefix](/readme.md#urlprefix).
 * [UrlsAsSnippets: Urls to files to be included as snippets](/readme.md#urlsassnippets).
 * [DocumentExtensions: Documents extensions to process. Defaults to `md`](/readme.md#documentextensions).
 * TreatMissingAsWarning: The default behavior for a missing snippet/include is to log an error (or throw an exception). To change that behavior to a warning set TreatMissingAsWarning to true.