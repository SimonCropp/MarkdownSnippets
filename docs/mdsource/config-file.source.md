# Config File

The [dotnet tool](/readme.md#installation) and the [MSBuild Task](msbuild.md) support a config file convention.

Add a file named `mdsnippets.json` at the target directory with the following content:


## For [InPlaceOverwrite](https://github.com/SimonCropp/MarkdownSnippets#inplaceoverwrite)

snippet: InPlaceOverwrite.json


## For [SourceTransform](https://github.com/SimonCropp/MarkdownSnippets#sourcetransform)

snippet: SourceTransform.json


## All Settings

snippet: allConfig.json


## JSON Schema

Editor help is available by adding the `$schema` field to the `mdsnippets.json` file.

```json
{
  "$schema": "https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/schema.json"
}
```

In the screenshot, [JetBrains Rider](https://jetbrains.com/rider), is able to offer code completion support.

![IDE schema code completion](/docs/code-completion.png)

The schema also includes `enum` values for constrained value types.

![IDE schema code completion](/docs/code-completion-values.png)


## More Info

 * [ReadOnly: Mark resulting files as read only](/readme.md#mark-resulting-files-as-read-only)
 * [LinkFormat](/readme.md#linkformat).
 * [Convention](/readme.md#document-convention).
 * [TocLevel: Heading level](/docs/toc.md#heading-level).
 * [TocExcludes: Ignore headings](/docs/toc.md#ignore-headings).
 * [Exclude: Exclude directories from discovery](/docs/exclusion.md).
 * [WriteHeader: Disable Header](/docs/header.md#disable-header).
 * [UrlPrefix](/readme.md#urlprefix).
 * [UrlsAsSnippets: Urls to files to be included as snippets](/readme.md#urlsassnippets).
 * TreatMissingAsWarning: The default behavior for a missing snippet/include is to log an error (or throw an exception). To change that behavior to a warning set TreatMissingAsWarning to true.
 * ValidateContent: Runs a set of technical-writing style checks over the content of processed markdown. When enabled, each line is validated and any problem is reported as an error. Defaults to false. The checks flag: exclamation marks (`! `); a list of discouraged words (such as `you`, `your`, `we`, `our`, `please`, `just`, `simply`, `easy`, and archaic terms like `aforementioned` or `henceforth`); and wordy phrases that have a simpler equivalent (for example `in order to` &rarr; `to`, `prior to` &rarr; `before`, `due to the fact that` &rarr; `because`). Lines that start with `>` (block quotes) are skipped.