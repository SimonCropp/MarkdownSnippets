using System;
using System.Collections.Generic;
using System.IO;

namespace MarkdownSnippets
{
    /// <summary>
    /// Handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public class SnippetMarkdownHandling
    {
        LinkFormat linkFormat;
        string rootDirectory;

        public SnippetMarkdownHandling(string rootDirectory, LinkFormat linkFormat)
        {
            this.linkFormat = linkFormat;
            Guard.AgainstNullAndEmpty(rootDirectory, nameof(rootDirectory));
            rootDirectory = Path.GetFullPath(rootDirectory);
            this.rootDirectory = rootDirectory.Replace(@"\", "/");
        }

        public void AppendGroup(string key, IEnumerable<Snippet> snippets, Action<string> appendLine)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendLine, nameof(appendLine));
            uint index = 0;
            foreach (var snippet in snippets)
            {
                WriteSnippet(appendLine, snippet, index);
                index++;
            }
        }

        void WriteSnippet(Action<string> appendLine, Snippet snippet, uint index)
        {
            string anchor;
            if (index == 0)
            {
                anchor = $"snippet-{snippet.Key}";
            }
            else
            {
                anchor = $"snippet-{snippet.Key}-{index}";
            }

            appendLine($"<a id='{anchor}'/></a>");
            var linkForAnchor = $"[anchor](#{anchor})";
            if (snippet.Path == null)
            {
                WriteSnippetValueAndLanguage(appendLine, snippet);
                // id anchors not supported on TFS
                //https://developercommunity.visualstudio.com/content/problem/63289/anchors-in-markdown-documents-not-working.html
                if (linkFormat != LinkFormat.Tfs)
                {
                    appendLine($"<sup>{linkForAnchor}</sup>");
                }
            }
            else
            {
                var path = snippet.Path.Replace(@"\", "/").Substring(rootDirectory.Length);
                var sourceLink = BuildLink(snippet, path);
                var linkForSource = $"[snippet source]({sourceLink})";
                WriteSnippetValueAndLanguage(appendLine, snippet);
                if (linkFormat == LinkFormat.Tfs)
                {
                    appendLine($"<sup>{linkForSource}</sup>");
                }
                else
                {
                    appendLine($"<sup>{linkForSource} | {linkForAnchor}</sup>");
                }
            }
        }

        static void WriteSnippetValueAndLanguage(Action<string> appendLine, Snippet snippet)
        {
            appendLine($"```{snippet.Language}");
            appendLine(snippet.Value);
            appendLine("```");
        }

        string BuildLink(Snippet snippet, string path)
        {
            #region BuildLink
            if (linkFormat == LinkFormat.GitHub)
            {
                return $"{path}#L{snippet.StartLine}-L{snippet.EndLine}";
            }
            if (linkFormat == LinkFormat.Tfs)
            {
                return $"{path}&line={snippet.StartLine}&lineEnd={snippet.EndLine}";
            }
            if (linkFormat == LinkFormat.Bitbucket)
            {
                return $"{path}#lines={snippet.StartLine}:{snippet.EndLine}";
            }
            if (linkFormat == LinkFormat.GitLab)
            {
                return $"{path}#L{snippet.StartLine}-{snippet.EndLine}";
            }
            #endregion

            throw new Exception($"Unknown LinkFormat: {linkFormat}");
        }
    }
}