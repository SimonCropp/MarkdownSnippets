# <img src="https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/src/icon.png" height="40px"> MarkdownSnippets

Extract code snippets from any language to be used when building documentation


## Using Snippets

The keyed snippets can be used in any documentation `.md` file by adding the text `snippet: KEY`.

Then snippets with the key (all versions) will be rendered in a tabbed manner. If there is only a single version then it will be rendered as a simple code block with no tabs.

For example

<pre>
Some blurb about the below snippet
snippet&#58; MySnippetName
</pre>

The resulting markdown will be will be:

    Some blurb about the below snippet
    ```
    My Snippet Code
    ```


### Including full files

When snippets are read all source files are stored in a list. When searching for a snippet with a specified key, and that key is not found, the list of files are used as a secondary lookup. The lookup is done by finding all files have that have a suffix matching the key. This results in the ability to include full files as snippets using the following syntax:

<pre>
snippet&#58; directory/FileToInclude.txt
</pre>

The path syntax uses forward slashes `/`.


## Defining Snippets


### Using comments

Any code wrapped in a convention based comment will be picked up. The comment needs to start with `startcode` which is followed by the key. The snippet is then terminated by `endcode`.

```
// startcode MySnippetName
My Snippet Code
// endcode
```


### Using regions

Any code wrapped in a named C# region will be picked up. The name of the region is used as the key.

```
#region MySnippetName
My Snippet Code
#endregion
```


### Code indentation

The code snippets will do smart trimming of snippet indentation.

For example given this snippet:

<pre>
&#8226;&#8226;#region MySnippetName
&#8226;&#8226;Line one of the snippet
&#8226;&#8226;&#8226;&#8226;Line two of the snippet
&#8226;&#8226;#endregion
</pre>

The leading two spaces (&#8226;&#8226;) will be trimmed and the result will be:

```
Line one of the snippet
••Line two of the snippet
```

The same behavior will apply to leading tabs.


### Do not mix tabs and spaces

If tabs and spaces are mixed there is no way for the snippets to work out what to trim.

So given this snippet:

<pre>
&#8226;&#8226;#region MySnippetNamea
&#8226;&#8226;Line one of the snippet
&#10137;&#10137;Line one of the snippet
&#8226;&#8226;#endregion
</pre>

Where &#10137; is a tab.

The resulting markdown will be will be

<pre>
Line one of the snippet
&#10137;&#10137;Line one of the snippet
</pre>

Note that none of the tabs have been trimmed.


### Ignored paths

When scanning for snippets the following are ignored:

 * All directories and files starting with a period `.`
 * All binary files as defined by https://github.com/sindresorhus/binary-extensions/
 * Any of the following directory names: `bin`, `obj`


## MarkdownSnippets.Tool

A [.NET Core Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for merging snippets into markdown documents.


### The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/MarkdownSnippets.Tool.svg?style=flat)](https://www.nuget.org/packages/MarkdownSnippets.Tool/)

https://nuget.org/packages/MarkdownSnippets.Tool/


### Target directory

The target directory can be defined via one of the following:

 * Passed in as a single argument; or
 * The current directory is used. Only if it exists with a git repository, that is a directory tree that contains a directory names `.git`.


### Behavior

 * Recursively scan the [target directory](#target-directory) for all non [ignored files](#ignore-paths) for snippets.
 * Recursively scan the [target directory](#target-directory) for all `*.source.md` files.
 * Merge the snippets with the `.source.md` to produce `.md` files. So for example `readme.source.md` would be merged with snippets to produce `readme.md`. Note that this process will overwrite any existing `.md` files that have matching `.source.md` files.


#### mdsource directory convention

There is a secondary convention that leverages the use of a directory named `mdsource`. Where `.source.md` files are placed in a `mdsource` sub-directory, the `mdsource` part of the file path will be removed when calculating the target path. This allows the `.source.md` to be grouped in a sub directory and avoid cluttering up the main documentation directory.

When using `mdsource` convention, all references to other files, such as hyperlinks and images, should specify the full path from the root of the repository. This will allow those links to work correctly in both the source and generated markdown files. Relative paths cannot work for both the source and the target file.


### Installation

To install use:

```ps
dotnet tool install -g MarkdownSnippets.Tool
```

To update use:

```ps
dotnet tool update -g MarkdownSnippets.Tool
```

To uninstall use:

```ps
dotnet tool uninstall -g MarkdownSnippets.Tool
```


### Usage

For running in the current directory:

```ps
mdsnippets
```

For running against a target directory:

```ps
mdsnippets C:\Code\TheTargetDirectory
```


#### Exclude directories

To exclude directories use `-e` or `--exclude`.

For example the following will exclude any directory containing 'foo' or 'bar'

```ps
mdsnippets -e foo:bar
```


## Release Notes

See [closed milestones](https://github.com/SimonCropp/MarkdownSnippets/milestones?state=closed).


## Creadits

Loosely based on some code from  https://github.com/shiftkey/scribble


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com) and is licensed under Creative Commons Attribution as:

> ["Down"](https://thenounproject.com/AlfredoCreates/collection/arrows-5-glyph/) by [Alfredo Creates](https://thenounproject.com/AlfredoCreates) from The Noun Project