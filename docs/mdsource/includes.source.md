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


## Markdown includes

Markdown includes are pulled into the document before passing the content through the snippet insertion.


### Defining an include

Add a file anywhere in the target directory that is suffixed with `.include.md`. For example, the file might be named `theKey.include.md`.


### Using an include

Add the following to the markdown:

   ```
   include: theKey
   ```