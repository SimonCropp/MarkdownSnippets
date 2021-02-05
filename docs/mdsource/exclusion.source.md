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

When scanning for snippets the following are ignored:

 * All directories and files starting with a period `.`
 * Any of the following directory names: `bin`, `obj`
 * All binary files as defined by https://github.com/sindresorhus/binary-extensions/:

snippet: BinaryFileExtensions

 * Files that cannot contain comments:

snippet: NoAcceptCommentsExtensions