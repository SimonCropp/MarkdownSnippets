# Header

When a .md file is written, a header is include. The default header is:

snippet: HeaderWriterTests.DefaultHeader.verified.txt


## Disable Header

To disable the header use `--write-header`

```ps
mdsnippets --write-header false
```


## Custom Header

To apply a custom header use `--header`. `{relativePath}` will be replaced with the relative path of the `.source.md` file.

```ps
mdsnippets --header "GENERATED FILE - Source File: {relativePath}"
```


## Newlines in Header

To insert a newline use `\n`

```ps
mdsnippets --header "GENERATED FILE\nSource File: {relativePath}"
```
