# Code indentation

The code snippets will do smart trimming of snippet indentation.

For example given this snippet:

<pre>
&#8226;&#8226;// startcode MySnippetName
&#8226;&#8226;Line one of the snippet
&#8226;&#8226;&#8226;&#8226;Line two of the snippet
&#8226;&#8226;// endcode
</pre>

The leading two spaces (&#8226;&#8226;) will be trimmed and the result will be:

```
Line one of the snippet
••Line two of the snippet
```

The same behavior will apply to leading tabs.


## Do not mix tabs and spaces

If tabs and spaces are mixed there is no way for the snippets to work out what to trim.

So given this snippet:

<pre>
&#8226;&#8226;// startcode MySnippetNamea
&#8226;&#8226;Line one of the snippet
&#10137;&#10137;Line one of the snippet
&#8226;&#8226;// endcode
</pre>

Where &#10137; is a tab.

The resulting markdown will be will be

<pre>
Line one of the snippet
&#10137;&#10137;Line one of the snippet
</pre>

Note that none of the tabs have been trimmed.