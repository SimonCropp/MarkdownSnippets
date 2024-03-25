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


## Ignored paths

### Directory exclusion rules:

snippet: DefaultDirectoryExclusions.cs


### File exclusion rules

All binary files as defined by https://github.com/sindresorhus/binary-extensions/:

snippet: BinaryFileExtensions


### No comment files

Files that cannot contain comments are excluded.

snippet: NoAcceptCommentsExtensions