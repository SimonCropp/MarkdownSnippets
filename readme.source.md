# <img src="https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/src/icon.png" height="40px"> MarkdownSnippets

A [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) that extract snippets from code files and merges them into markdown documents.


### More Info

 * [.net API](/docs/api.md)
 * [MsBuild Task](/docs/msbuild.md)
 * [Config file convention](/docs/config-file.md)
 * [Indentation](/docs/indentation.md)

toc

## Installation

Ensure [dotnet CLI is installed](https://docs.microsoft.com/en-us/dotnet/core/tools/).

**There is known a issue with dotnet tools on macOS and Linux that results in [installed tools not being discovered in the current path](https://github.com/dotnet/cli/issues/9321). The workaround is to add `~/.dotnet/tools` to the PATH.**

Install [MarkdownSnippets.Tool](https://nuget.org/packages/MarkdownSnippets.Tool/)

```ps
dotnet tool install -g MarkdownSnippets.Tool
```


## Usage

```ps
mdsnippets C:\Code\TargetDirectory
```

If no directory is passed the current directory will be used, but only if it exists with a git repository directory tree. If not an error is returned.


### Behavior

 * Recursively scan the target directory for all non [ignored files](#ignore-paths) for snippets.
 * Recursively scan the target directory for all `*.source.md` files.
 * Merge the snippets with the `.source.md` to produce `.md` files. So for example `readme.source.md` would be merged with snippets to produce `readme.md`. Note that this process will overwrite any existing `.md` files that have matching `.source.md` files.


### mdsource directory convention

There is a secondary convention that leverages the use of a directory named `mdsource`. Where `.source.md` files are placed in a `mdsource` sub-directory, the `mdsource` part of the file path will be removed when calculating the target path. This allows the `.source.md` to be grouped in a sub directory and avoid cluttering up the main documentation directory.

When using the `mdsource` convention, all references to other files, such as links and images, should specify the full path from the root of the repository. This will allow those links to work correctly in both the source and generated markdown files. Relative paths cannot work for both the source and the target file.


## Defining Snippets

Any code wrapped in a convention based comment will be picked up. The comment needs to start with `begin-snippet:` which is followed by the key. The snippet is then terminated by `end-snippet`.

```
// begin-snippet: MySnippetName
My Snippet Code
// end-snippet
```

Named [C# regions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-region) will also be picked up, with the name of the region used as the key.

To stop regions collapsing in Visual Studio [disable 'enter outlining mode when files open'](/docs/stop-regions-collapsing.png). See [Visual Studio outlining](https://docs.microsoft.com/en-us/visualstudio/ide/outlining).


## Using Snippets

The keyed snippets can be used in any documentation `.md` file by adding the text `snippet: KEY`.

Then snippets with that key.

For example

<pre>
Some blurb about the below snippet
snippet&#58; MySnippetName
</pre>

The resulting markdown will be:

    Some blurb about the below snippet
    ```
    My Snippet Code
    ```


### Including full files

When snippets are read all source files are stored in a list. When searching for a snippet with a specified key, and that key is not found, the list of files are used as a secondary lookup. The lookup is done by finding all files that have a suffix matching the key. This results in the ability to include full files as snippets using the following syntax:

<pre>
snippet&#58; directory/FileToInclude.txt
</pre>

The path syntax uses forward slashes `/`.


## Snippet Exclusions


### Exclude directories from snippet discovery

To exclude directories use `-e` or `--exclude`.

For example the following will exclude any directory containing 'foo' or 'bar'

```ps
mdsnippets -e foo:bar
```


### Ignored paths

When scanning for snippets the following are ignored:

 * All directories and files starting with a period `.`
 * All binary files as defined by https://github.com/sindresorhus/binary-extensions/
 * Any of the following directory names: `bin`, `obj`


## Mark resulting files as read only

To mark the resulting `.md` files as read only use `-r` or `--readonly`.

This can be helpful in preventing incorrectly editing the `.md` file instead of the `.source.md` file.

```ps
mdsnippets -r true
```


## Table of contents

If a line is `toc` it will be replaced with a table of contents

So if a markdown document contains the following:

snippet: tocBefore.txt

The result will be rendered:

snippet: tocAfter.txt


### Heading Level

Headings with level 2 (`##`) or greater can be rendered. By default all level 2 and level 3 headings are included.

To include more levels use the `--toc-level` argument. So for example to include headings levels 2 though level 6 use:

```ps
mdsnippets --toc-level 5
```


### Ignore Headings

To exclude headings use the `--toc-excludes` argument. So for example to exclude `heading1` and `heading2` use:

```ps
mdsnippets --toc-excludes heading1:heading2
```

## Header

When a .md file is written, a header is include. The default header is:

snippet: HeaderWriterTests.DefaultHeader.approved.txt

To disable the header use '--write-header'

```ps
mdsnippets --write-header false
```

To apply a custom header use '--header'

```ps
mdsnippets --header "GENERATED FILE - Source File: {relativePath}"
```

To insert a newline use `\n`

```ps
mdsnippets --header "GENERATED FILE\nSource File: {relativePath}"
```


## LinkFormat

Defines the format of `snippet source` links that appear under each snippet.

snippet: LinkFormat.cs

snippet: BuildLink


## Release Notes

See [closed milestones](https://github.com/SimonCropp/MarkdownSnippets/milestones?state=closed).


## Credits

Loosely based on some code from https://github.com/shiftkey/scribble.


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com) and is licensed under Creative Commons Attribution as:

> ["Down"](https://thenounproject.com/AlfredoCreates/collection/arrows-5-glyph/) by [Alfredo Creates](https://thenounproject.com/AlfredoCreates) from [The Noun Project](https://thenounproject.com/)