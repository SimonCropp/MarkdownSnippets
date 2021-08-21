using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MarkdownSnippets
{
    /// <summary>
    /// Handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public class SnippetMarkdownHandling
    {
        LinkFormat linkFormat;
        string? urlPrefix;
        string targetDirectory;
        Func<Snippet, string> getAnchorId;

        public SnippetMarkdownHandling(string targetDirectory, LinkFormat linkFormat, bool hashSnippetAnchors, string? urlPrefix = null)
        {
            this.linkFormat = linkFormat;
            this.urlPrefix = urlPrefix;
            Guard.AgainstNullAndEmpty(targetDirectory, nameof(targetDirectory));
            targetDirectory = Path.GetFullPath(targetDirectory);
            this.targetDirectory = targetDirectory.Replace(@"\", "/");

            if (hashSnippetAnchors)
            {
                getAnchorId  = ComputeId;
            }
            else
            {
                getAnchorId = snippet => $"snippet-{snippet.Key}";
            }
        }

        public void Append(string key, IEnumerable<Snippet> snippets, Action<string> appendLine)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
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

        string GetAnchorText(Snippet snippet, uint index)
        {
            var id = getAnchorId(snippet);
            if (index == 0)
            {
                return id;
            }

            return  $"{id}-{index}";
        }

        static string ComputeId(Snippet snippet)
        {
            using SHA1Managed sha = new();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(snippet.Key));
            return string.Concat(hash.Take(4).Select(b => b.ToString("x2")));
        }

        bool TryGetSupText(Snippet snippet, string anchor, [NotNullWhen(true)] out string? supText)
        {
            var linkForAnchor = $"<a href='#{anchor}' title='Start of snippet'>anchor</a>";
            if (snippet.Path == null)
            {
                supText = linkForAnchor;
                return true;
            }

            var path = snippet.Path.Replace(@"\", "/");
            if (!path.StartsWith(targetDirectory))
            {
                // if file is not in the targetDirectory then the url wont work
                supText = linkForAnchor;
                return true;
            }

            path = path.Substring(targetDirectory.Length);

            var sourceLink = BuildLink(snippet, path);
            var linkForSource = $"<a href='{urlPrefix}{sourceLink}' title='Snippet source file'>snippet source</a>";
            supText = $"{linkForSource} | {linkForAnchor}";

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

            throw new($"Unknown LinkFormat: {linkFormat}");
        }
    }
}