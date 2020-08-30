# Snippet Exclusions


## Exclude directories from snippet discovery

To exclude directories use `-e` or `--exclude`.

For example the following will exclude any directory containing 'foo' or 'bar'

```ps
mdsnippets -e foo:bar
```


## Ignored paths

When scanning for snippets the following are ignored:

 * All directories and files starting with a period `.`
 * Any of the following directory names: `bin`, `obj`
 * All binary files as defined by https://github.com/sindresorhus/binary-extensions/:

snippet: ExcludedFileExtensions

 * Files that cannot contain comments:

snippet: NoAcceptCommentsExtensions