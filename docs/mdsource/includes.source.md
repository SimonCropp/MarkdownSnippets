# Includes


## Including full code files

When snippets are read all source files are stored in a list. When searching for a snippet with a specified key, and that key is not found, the list of files are used as a secondary lookup. The lookup is done by finding all files that have a suffix matching the key. This results in the ability to include full files as snippets using the following syntax:

<pre>
snippet&#58; directory/FileToInclude.txt
</pre>

The path syntax uses forward slashes `/`.


## Including urls


<pre>
snippet&#58; http://myurl
</pre>

## Including a snippet from an external URL

To include a specific named snippet from a file using an external URL, use the `web-snippet:` keyword followed by the URL and the snippet key separated by a `#`:

<pre>
web-snippet&#58;https://raw.githubusercontent.com/owner/repo/branch/path/to/file.cs#snippetKey
</pre>

This will fetch the file from the URL, extract the snippet with the given key, and embed it in your Markdown.


## Markdown includes

Markdown includes are pulled into the document before passing the content through the snippet insertion.


### Defining an include

Add a file anywhere in the target directory that is suffixed with `.include.md`. For example, the file might be named `theKey.include.md`.


### Using an include

Add the following to the markdown:

   ```
   include: theKey
   ```