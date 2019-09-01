# MsBuild

An [MsBuild task](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-task) for merging snippets into markdown documents.

MsBuild has a [convention](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#from-a-convention-based-working-directory) to automatically run build tasks from included NuGet packages. This package takes advantage of that hook to run markdownsnippets on build.

This package only need to be included in one project of the solution. A logical choice is the test project.


## NuGet package

https://nuget.org/packages/MarkdownSnippets.MsBuild/ [![NuGet Status](https://img.shields.io/nuget/v/MarkdownSnippets.MsBuild.svg)](https://www.nuget.org/packages/MarkdownSnippets.MsBuild/)


## More Info

 * [Config file convention](/docs/config-file.md).