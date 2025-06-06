# <img src="/src/icon.png" height="30px"> MarkdownSnippets

[![Build status](https://ci.appveyor.com/api/projects/status/8ijthhby6mhw8fk3/branch/main?svg=true)](https://ci.appveyor.com/project/SimonCropp/MarkdownSnippets)
[![NuGet Status](https://img.shields.io/nuget/v/MarkdownSnippets.Tool.svg?label=dotnet%20tool)](https://www.nuget.org/packages/MarkdownSnippets.Tool/)
[![NuGet Status](https://img.shields.io/nuget/v/MarkdownSnippets.MsBuild.svg?label=MsBuild%20Task)](https://www.nuget.org/packages/MarkdownSnippets.MsBuild/)
[![NuGet Status](https://img.shields.io/nuget/v/MarkdownSnippets.svg?label=.net%20API)](https://www.nuget.org/packages/MarkdownSnippets/)

A [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) or [MsBuild Task](/docs/msbuild.md) that extract snippets from code files and merges them into markdown documents.

**See [Milestones](../../milestones?state=closed) for release notes.**

**[.net 8](https://dotnet.microsoft.com/download/dotnet/8.0) or higher is required to run the dotnet tool.**


## Value Proposition

Automatically extract snippets from code and injecting them into markdown documents has several benefits:

 * Snippets can be verified by a compiler or parser.
 * Tests can be run on snippets, or snippets can be pulled from existing tests.
 * Changes in code are automatically reflected in documentation.
 * Snippets are less likely to get out of sync with the main code-base.
 * Snippets in markdown is easier to create and maintain since any preferred editor can be used to edit them.


## Behavior

 * Recursively scan the target directory for code files containing snippets. (See [exclusion](/docs/exclusion.md)).
 * Recursively scan the target directory for markdown (`.md` or `mdx`) files. (See [Document Scanning](#document-convention)).
 * Merge the snippets into those markdown files.


## Installation

Ensure [dotnet CLI is installed](https://docs.microsoft.com/en-us/dotnet/core/tools/).

Install [MarkdownSnippets.Tool](https://nuget.org/packages/MarkdownSnippets.Tool/)

```ps
dotnet tool install -g MarkdownSnippets.Tool
```

See also: [MsBuild Task usage](/docs/msbuild.md)


## Usage

```ps
mdsnippets C:\Code\TargetDirectory
```

If no directory is passed the current directory will be used, but only if it exists with a git repository directory tree. If not an error is returned.


### Document Convention

There are two approaches scanning and modifying markdown files.


#### SourceTransform

This is the default.


##### source.md file

The file convention recursively scans the target directory for all `*.source.md` files. Once snippets are merged the `.source.md` to produce `.md` files. So for example `readme.source.md` would be merged with snippets to produce `readme.md`. Note that this process will overwrite any existing `.md` files that have matching `.source.md` files.


#### mdsource directory

There is a secondary convention that leverages the use of a directory named `mdsource`. Where `.source.md` files are placed in a `mdsource` sub-directory, the `mdsource` part of the file path will be removed when calculating the target path. This allows the `.source.md` to be grouped in a sub directory and avoid cluttering up the main documentation directory.

When using the `mdsource` convention, all references to other files, such as links and images, should specify the full path from the root of the repository. This will allow those links to work correctly in both the source and generated markdown files. Relative paths cannot work for both the source and the target file.


#### InPlaceOverwrite

Recursively scans the target directory for all `*.md` files and merges snippets into those files.


##### Command line

```ps
mdsnippets -c InPlaceOverwrite
```

```ps
mdsnippets --convention InPlaceOverwrite
```


##### Config file

Can be enabled in [mdsnippets.json config file](/docs/config-file.md).

```json
{
  "Convention": "InPlaceOverwrite"
}
```


#### Moving from SourceTransform to InPlaceOverwrite

 * Ensure `"WriteHeader": false` is used in `mdsnippets.json`.
 * Ensure `"ReadOnly": false` is used in `mdsnippets.json`.
 * Ensure using the current stable version and a docs generation has run.
 * Delete all `.source.md` files.
 * Modify `mdsnippets.json` to add `"Convention": "InPlaceOverwrite"`.
 * Run the docs generation.


### Mark resulting files as read only

To mark the resulting documents files as read only use `-r` or `--read-only`.

This can be helpful in preventing incorrectly editing the documents file instead of the `.source.` file conventions.

```ps
mdsnippets -r true
```

```ps
mdsnippets --read-only true
```


## Defining Snippets

Any code wrapped in a convention based comment will be picked up. The comment needs to start with `begin-snippet:` which is followed by the key. The snippet is then terminated by `end-snippet`.

```
// begin-snippet: MySnippetName
My Snippet Code
// end-snippet
```

Named [C# regions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-region) will also be picked up, with the name of the region used as the key.

To stop regions collapsing in Visual Studio [disable 'enter outlining mode when files open'](/docs/stop-regions-collapsing.png). See [Visual Studio outlining](https://docs.microsoft.com/en-us/visualstudio/ide/outlining).


### UrlsAsSnippets

Urls to files to be included as snippets. Space ` ` separated for multiple values.

Each url will be accessible using the file name as a key. Any snippets within the files will be extracted and accessible as individual keyed snippets.

```ps
mdsnippets --urls-as-snippets "https://github.com/SimonCropp/MarkdownSnippets/snippet.cs"
```

```ps
mdsnippets -u "https://github.com/SimonCropp/MarkdownSnippets/snippet.cs"
```


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
    <!-- snippet: MySnippetName -->
    <a id='snippet-MySnippetName'></a>
    ```
    My Snippet Code
    ```
    <sup><a href='/relativeUrlToFile#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-MySnippetName' title='Start of snippet'>anchor</a></sup>
    <!-- endSnippet -->

Notes:

 * The vertical bar ( | ) is used to separate adjacent links as per web accessibility recommendations: https://webaim.org/techniques/hypertext/hypertext_links#groups
 * [H33: Supplementing link text with the title attribute](https://www.w3.org/TR/WCAG20-TECHS/H33.html)


### Including a snippet from the web

Snippets that start with `http` will be downloaded and the contents rendered. For example:

    snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/license.txt

Will render:

	<!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/license.txt -->
	<a id='snippet-https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/license.txt'></a>
	```txt
	The MIT License (MIT)
	...
	```
	<sup><a href='#snippet-https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/license.txt' title='Snippet source file'>anchor</a></sup>
	<!-- endSnippet -->

Files are downloaded to `%temp%MarkdownSnippets` with a maximum of 100 files kept.


### Including a full file

If no snippet is found matching the defined name. The target directory will be searched for a file matching that name. For example:

    snippet: license.txt

Will render:

	<!-- snippet: license.txt -->
	<a id='snippet-license.txt'></a>
	```txt
	The MIT License (MIT)
	...
	```
	<sup><a href='#snippet-license.txt' title='Start of snippet'>anchor</a></sup>
	<!-- endSnippet -->


### LinkFormat

Defines the format of `snippet source` links that appear under each snippet.


#### Command line

```ps
mdsnippets --link-format Bitbucket
```

```ps
mdsnippets -l Bitbucket
```


#### Values

snippet: LinkFormat.cs

Link format `None` will omit the source link but still keep the snippet anchor.


### OmitSnippetLinks

The links below a snippet can be omitted.


#### Command line

```ps
mdsnippets --omit-snippet-links true
```


#### Config file

```
{
  "OmitSnippetLinks": true
}
```


#### How links are constructed

snippet: BuildLink


### UrlPrefix

UrlPrefix allows a string to be defined that will prefix all snippet links. This is helpful when the markdown file are being hosted on a site that is not co-located with the source code files. It can be defined in the [config file](/docs/config-file.md), the [MsBuild task](/docs/msbuild.md), and the dotnet tool.


#### Command line

```ps
mdsnippets --url-prefix "TheUrlPrefix"
```


#### Config file

```
{
  "UrlPrefix": "TheUrlPrefix"
}
```


## Add to Windows Explorer

Use [src/context-menu.reg](context-menu.reg) to add MarkdownSnippets to the Windows Explorer context menu.

snippet: context-menu.reg


## Expressive Code / Snippet Metadata

When defining snippets, you can add additional metadata at the source to the rendered snippet using the following syntax.

```csharp
// begin-snippet: HelloWorld(title=Program.cs {1})
Console.WriteLine("Hello, World");
// end-snippet
```

Note the text within the parenthesis; this is metadata we want to add to the rendered Markdown block immediately after the language declaration. The metadata is stripped and the key remains `HelloWorld`. The feature produces the following output at your target destination (will vary based on your configuration):

````markdown
<-- begin-snippet: HelloWorld -->
```csharp title=Program.cs {1}
Console.WriteLine("Hello, World");
```
<-- end-snippet -->
````

This syntax is known as [Expressive Code](https://expressive-code.com/) and is supported in documentation systems such as [Astro Starlight](https://github.com/withastro/starlight/) but can be installed in any Markdown-powered tool that supports [reHype](https://github.com/rehypejs/rehype).

It is important to note, the metadata is not explicitly limited to Expressive code. Any text within the `()` will be rendered after the language block. This can be useful for adding additional information based on your specific rendering engine. For example, if you use a presentation tool such as [Sli.dev](https://sli.dev/), you can use this feature to apply [magic-move animations](https://sli.dev/features/shiki-magic-move).

```csharp
// begin-snippet: EncapsulateVariable({*|2})
Console.WriteLine("Hello, World");
// end-snippet
```

The above snippet will render as follows:

````markdown
<-- begin-snippet: EncapsulateVariable -->
```csharp {*|2}
Console.WriteLine("Hello, World");
```
<-- end-snippet -->
````


## More Documentation

include: doc-index


## Credits

Loosely based on some code from https://github.com/shiftkey/scribble.


## Icon

[Down](https://thenounproject.com/AlfredoCreates/collection/arrows-5-glyph/) by [Alfredo Creates](https://thenounproject.com/AlfredoCreates) from [The Noun Project](https://thenounproject.com/).