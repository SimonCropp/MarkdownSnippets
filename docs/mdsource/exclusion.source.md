# Exclusions


## Exclude directories from snippet and markdown discovery

To exclude directories use `-e` or `--exclude-directories`.

For example the following will exclude any directory containing 'foo' or 'bar'

```ps
mdsnippets -e foo:bar
```


## Exclude snippets from directories

To exclude directories from snippet discovery use `--exclude-snippet-directories`.

For example the following will exclude any directory containing 'foo' or 'bar'

```ps
mdsnippets --exclude-snippet-directories foo:bar
```

## Exclude markdown from directories

To exclude directories from markdown discovery use `--exclude-markdown-directories`.

For example the following will exclude any directory containing 'foo' or 'bar'

```ps
mdsnippets --exclude-markdown-directories foo:bar
```


## Exclude files from snippet discovery

To exclude specific files from snippet discovery, add an `ExcludeSnippetFiles` array to [`mdsnippets.json`](/docs/config-file.md). Each entry is a glob pattern matched (case-insensitively) against the file *name* — not the full path. Patterns support `*` (any sequence of characters) and `?` (a single character).

For example, the following excludes all Verify snapshot files so that `<!-- begin-snippet -->` markers hand-added to a `*.verified.txt` are not picked up as snippet sources:

```json
{
  "ExcludeSnippetFiles": [
    "*.verified.txt",
    "*.received.txt"
  ]
}
```

Matched files are still visible to the include/all-files enumeration — only snippet scanning skips them.


## Exclude files from markdown processing

To exclude specific markdown files from being processed, add an `ExcludeMarkdownFiles` array to [`mdsnippets.json`](/docs/config-file.md). Each entry is a glob pattern matched (case-insensitively) against the file *name* — not the full path. Patterns support `*` (any sequence of characters) and `?` (a single character).

For example, the following excludes any draft file and a specific changelog from processing:

```json
{
  "ExcludeMarkdownFiles": [
    "*.draft.md",
    "changelog.source.md"
  ]
}
```

The pattern is matched against the source file name. For [SourceTransform](/readme.md#sourcetransform) that is the `*.source.md` (or `*.source.mdx`) file; for [InPlaceOverwrite](/readme.md#inplaceoverwrite) it is the `*.md` (or `*.mdx`) file. Excluded files are skipped entirely: no output is generated for them. `*.include.md` files are unaffected — use directory exclusions to control includes.


## Ignored paths

### Directory exclusion rules:

snippet: DefaultDirectoryExclusions.cs


### File exclusion rules

All binary files as defined by https://github.com/sindresorhus/binary-extensions/:

snippet: BinaryFileExtensions


### No comment files

Files that cannot contain comments are excluded.

snippet: NoAcceptCommentsExtensions