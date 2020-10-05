using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MarkdownSnippets
{
    /// <summary>
    /// Handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public class SnippetMarkdownHandling
    {
        LinkFormat linkFormat;
        string? urlPrefix;
        string rootDirectory;

        public SnippetMarkdownHandling(string rootDirectory, LinkFormat linkFormat, string? urlPrefix = null)
        {
            this.linkFormat = linkFormat;
            this.urlPrefix = urlPrefix;
            Guard.AgainstNullAndEmpty(rootDirectory, nameof(rootDirectory));
            rootDirectory = Path.GetFullPath(rootDirectory);
            this.rootDirectory = rootDirectory.Replace(@"\", "/");
        }

        public void Append(string key, IEnumerable<Snippet> snippets, Action<string> appendLine)
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
            var anchor = GetAnchorText(snippet, index);

            appendLine($"<a id='{anchor}'></a>");
            WriteSnippetValueAndLanguage(appendLine, snippet);

            if (TryGetSupText(snippet,anchor, out var supText))
            {
                appendLine($"<sup>{supText}</sup>");
            }
        }

        static string GetAnchorText(Snippet snippet, uint index)
        {
            if (index == 0)
            {
                return $"snippet-{snippet.Key}";
            }

            return  $"snippet-{snippet.Key}-{index}";
        }

        bool TryGetSupText(Snippet snippet, string anchor, [NotNullWhen(true)] out string? supText)
        {
            var linkForAnchor = $"<a href='#{anchor}' title='Navigate to start of snippet `{snippet.Key}`'>anchor</a>";
            if (snippet.Path == null)
            {
                // id anchors not supported on TFS
                //https://developercommunity.visualstudio.com/content/problem/63289/anchors-in-markdown-documents-not-working.html
                if (linkFormat != LinkFormat.Tfs)
                {
                    supText = linkForAnchor;
                    return true;
                }

                supText = null;
                return false;
            }

            var path = snippet.Path.Replace(@"\", "/");
            if (!path.StartsWith(rootDirectory))
            {
                //if file is not in the rootDirectory then the url wont work
                supText = linkForAnchor;
                return true;
            }

            path = path.Substring(rootDirectory.Length);

            var sourceLink = BuildLink(snippet, path);
            var linkForSource = $"<a href='{urlPrefix}{sourceLink}' title='File snippet `{snippet.Key}` was extracted from'>snippet source</a>";
            if (linkFormat == LinkFormat.Tfs)
            {
                supText = linkForSource;
            }
            else
            {
                supText = $"{linkForSource} | {linkForAnchor}";
            }

            return true;
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