# Table of contents

If a line is 'toc' it will be replaces with a table of contents containeing all level 2 headings (Lines starting with `## `).

So if a markdown document contains the following

```
# Title

toc

## Heading 1

Text1

## Heading 1

Text2
```

The result will be rendered:

```
# Title

<!-- toc -->
## Contents

 * [Heading 1](heading-1)
 * [Heading 2](heading-2)

## Heading 1

Text1

## Heading 2

Text2
```
