using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownSnippets
{
    /// <summary>
    /// Extracts <see cref="Snippet"/>s from a given input.
    /// </summary>
    public static class FileSnippetExtractor
    {
        public static Task AppendUrlAsSnippet(this ICollection<Snippet> snippets, string url)
        {
            Guard.AgainstNullAndEmpty(url, nameof(url));
            return AppendUrlAsSnippet(snippets, url, Path.GetFileName(url).ToLowerInvariant());
        }

        public static async Task AppendUrlsAsSnippets(this ICollection<Snippet> snippets, params string[] urls)
        {
            Guard.AgainstNull(urls, nameof(urls));
            foreach (var url in urls)
            {
                await AppendUrlAsSnippet(snippets, url).ConfigureAwait(false);
            }
        }

        public static async Task AppendUrlAsSnippet(ICollection<Snippet> snippets, string url, string key)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNullAndEmpty(url, nameof(url));
            var text = await Downloader.DownloadFile(url).ConfigureAwait(false);
            var snippet = Snippet.Build(1, text.LineCount(), text, key, GetLanguageFromPath(url), null);
            snippets.Add(snippet);
        }

        public static void AppendFileAsSnippet(this ICollection<Snippet> snippets, string filePath)
        {
            Guard.FileExists(filePath, nameof(filePath));
            AppendFileAsSnippet(snippets, filePath, Path.GetFileName(filePath).ToLowerInvariant());
        }

        public static void AppendFilesAsSnippets(this ICollection<Snippet> snippets, params string[] filePaths)
        {
            Guard.AgainstNull(filePaths, nameof(filePaths));
            foreach (var filePath in filePaths)
            {
                AppendFileAsSnippet(snippets, filePath);
            }
        }

        public static void AppendFileAsSnippet(ICollection<Snippet> snippets, string filePath, string key)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.FileExists(filePath, nameof(filePath));
            var text = File.ReadAllText(filePath);
            var snippet = Snippet.Build(1, text.LineCount(), text, key, GetLanguageFromPath(filePath), filePath);
            snippets.Add(snippet);
        }

        /// <summary>
        /// Read from a paths.
        /// </summary>
        /// <param name="paths">The paths to extract <see cref="Snippet"/>s from.</param>
        public static IEnumerable<Snippet> Read(IEnumerable<string> paths)
        {
            Guard.AgainstNull(paths, nameof(paths));
            return paths.SelectMany(Read);
        }

        /// <summary>
        /// Read from a path.
        /// </summary>
        /// <param name="path">The current path to extract <see cref="Snippet"/>s from.</param>
        public static IEnumerable<Snippet> Read(string path)
        {
            Guard.AgainstNullAndEmpty(path, nameof(path));
            using (var reader = File.OpenText(path))
            {
                return Read(reader, path).ToList();
            }
        }

        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path being used to extract <see cref="Snippet"/>s from. Only used for logging purposes in this overload.</param>
        public static IEnumerable<Snippet> Read(TextReader textReader, string path)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNullAndEmpty(path, nameof(path));
            try
            {
                var reader = new IndexReader(textReader);
                return GetSnippets(reader, path);
            }
            catch (Exception exception)
            {
                throw new Exception($"Could not extract snippets from '{path}'.", exception);
            }
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            var s = extension?.TrimStart('.');
            if (s == null)
            {
                return string.Empty;
            }
            return s;
        }

        static IEnumerable<Snippet> GetSnippets(IndexReader stringReader, string path)
        {
            var language = GetLanguageFromPath(path);
            var loopStack = new LoopStack();

            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                {
                    if (loopStack.IsInSnippet)
                    {
                        var current = loopStack.Current;
                        yield return Snippet.BuildError(
                            error: "Snippet was not closed",
                            path: path,
                            lineNumberInError: current.StartLine + 1,
                            key: current.Key);
                    }

                    break;
                }

                var trimmedLine = line.Trim()
                    .Replace("  ", " ")
                    .ToLowerInvariant();

                if (StartEndTester.IsStart(trimmedLine, path, out var key, out var endFunc))
                {
                    loopStack.Push(endFunc, key, stringReader.Index);
                    continue;
                }

                if (!loopStack.IsInSnippet)
                {
                    continue;
                }
                if (!loopStack.Current.EndFunc(trimmedLine))
                {
                    loopStack.AppendLine(line);
                    continue;
                }

                yield return BuildSnippet(stringReader, path, loopStack, language);
                loopStack.Pop();
            }
        }

        static Snippet BuildSnippet(IndexReader stringReader, string path, LoopStack loopStack, string language)
        {
            var loopState = loopStack.Current;
            var startRow = loopState.StartLine + 1;

            var value = loopState.GetLines();
            if (value.IndexOfAny(invalidCharacters) > -1)
            {
                var joinedInvalidChars = $"'{string.Join("', '", invalidCharacters)}'";
                return Snippet.BuildError(
                    error: $"Snippet contains invalid characters ({joinedInvalidChars}). This was probably caused by copying code from MS Word or Outlook. Dont do that.",
                    path: path,
                    lineNumberInError: startRow,
                    key: loopState.Key);
            }

            return Snippet.Build(
                startLine: loopState.StartLine,
                endLine: stringReader.Index,
                key: loopState.Key,
                value: value,
                path: path,
                language: language.ToLowerInvariant()
            );
        }

        static char[] invalidCharacters = {'“', '”', '—'};
    }
}