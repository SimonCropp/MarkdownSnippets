# Table of contents

If a line is `toc` it will be replaced with a table of contents

So if a markdown document contains the following:

snippet: tocBefore.txt

The result will be rendered:

snippet: tocAfter.txt


## Heading Level

Headings with level 2 (`##`) or greater can be rendered. By default all level 2 and level 3 headings are included.

To include more levels use the `--toc-level` argument. So for example to include headings levels 2 though level 6 use:

```ps
mdsnippets --toc-level 5
```


## Ignore Headings

To exclude headings use the `--toc-excludes` argument. So for example to exclude `heading1` and `heading2` use:

```ps
mdsnippets --toc-excludes heading1:heading2
```