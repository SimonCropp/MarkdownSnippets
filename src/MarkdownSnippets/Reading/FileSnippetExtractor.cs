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
        /// <summary>
        /// Each url will be accessible using the file name as a key. Any snippets within the files will be extracted and accessible as individual keyed snippets.
        /// </summary>
        public static Task AppendUrlAsSnippet(this ICollection<Snippet> snippets, string url)
        {
            Guard.AgainstNullAndEmpty(url, nameof(url));
            return AppendUrlAsSnippet(snippets, url, Path.GetFileName(url).ToLowerInvariant());
        }
        
        /// <summary>
        /// Each url will be accessible using the file name as a key. Any snippets within the files will be extracted and accessible as individual keyed snippets.
        /// </summary>
        public static Task AppendUrlsAsSnippets(this ICollection<Snippet> snippets, params string[] urls)
        {
            return AppendUrlsAsSnippets(snippets, (IEnumerable<string>) urls);
        }
        
        /// <summary>
        /// Each url will be accessible using the file name as a key. Any snippets within the files will be extracted and accessible as individual keyed snippets.
        /// </summary>
        public static async Task AppendUrlsAsSnippets(this ICollection<Snippet> snippets, IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                await AppendUrlAsSnippet(snippets, url);
            }
        }
        
        /// <summary>
        /// The url will be accessible using the file name as a key. Any snippets within the file will be extracted and accessible as individual keyed snippets.
        /// </summary>
        public static async Task AppendUrlAsSnippet(ICollection<Snippet> snippets, string url, string key)
        {
            Guard.AgainstNullAndEmpty(url, nameof(url));
            var (success, content) = await Downloader.DownloadContent(url);
            if (!success)
            {
                throw new SnippetException($"Unable to get UrlAsSnippet: {url}");
            }

            var snippet = Snippet.Build(1, content!.LineCount(), content!, key, GetLanguageFromPath(url), null);
            snippets.Add(snippet);

            using var reader = new StringReader(content!);
            foreach (var innerSnippet in Read(reader, url))
            {
                snippets.Add(innerSnippet);
            }
        }

        public static void AppendFileAsSnippet(this ICollection<Snippet> snippets, string filePath)
        {
            Guard.FileExists(filePath, nameof(filePath));
            AppendFileAsSnippet(snippets, filePath, Path.GetFileName(filePath).ToLowerInvariant());
        }

        public static void AppendFilesAsSnippets(this ICollection<Snippet> snippets, params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                AppendFileAsSnippet(snippets, filePath);
            }
        }

        public static void AppendFileAsSnippet(ICollection<Snippet> snippets, string filePath, string key)
        {
            Guard.FileExists(filePath, nameof(filePath));
            var text = File.ReadAllText(filePath);
            var snippet = Snippet.Build(1, text.LineCount(), text, key, GetLanguageFromPath(filePath), filePath);
            snippets.Add(snippet);
        }

        /// <summary>
        /// Read from paths.
        /// </summary>
        /// <param name="paths">The paths to extract <see cref="Snippet"/>s from.</param>
        /// <param name="maxWidth">Controls the maximum character width for snippets. Must be positive.</param>
        /// <param name="newLine">The string to use as a line separator in snippets.</param>
        public static IEnumerable<Snippet> Read(IEnumerable<string> paths, int maxWidth = int.MaxValue, string newLine = "\n")
        {
            return paths
                .Where(x => SnippetFileExclusions.CanContainCommentsExtension(Path.GetExtension(x).Substring(1)))
                .SelectMany(path => Read(path, maxWidth, newLine));
        }

        /// <summary>
        /// Read from a path.
        /// </summary>
        /// <param name="path">The current path to extract <see cref="Snippet"/>s from.</param>
        /// <param name="maxWidth">Controls the maximum character width for snippets. Must be positive.</param>
        /// <param name="newLine">The string to use as a line separator in snippets.</param>
        public static IEnumerable<Snippet> Read(string path, int maxWidth = int.MaxValue, string newLine = "\n")
        {
            Guard.AgainstNegativeAndZero(maxWidth, nameof(maxWidth));
            Guard.AgainstNullAndEmpty(path, nameof(path));
            if (!File.Exists(path))
            {
                return Enumerable.Empty<Snippet>();
            }

            using var reader = File.OpenText(path);
            return Read(reader, path, maxWidth, newLine).ToList();
        }

        /// <summary>
        /// Read from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to read from.</param>
        /// <param name="path">The current path being used to extract <see cref="Snippet"/>s from. Only used for logging purposes in this overload.</param>
        /// <param name="maxWidth">Controls the maximum character width for snippets. Must be positive.</param>
        /// <param name="newLine">The string to use as a line separator in snippets.</param>
        public static IEnumerable<Snippet> Read(TextReader textReader, string path, int maxWidth = int.MaxValue, string newLine = "\n")
        {
            Guard.AgainstNegativeAndZero(maxWidth, nameof(maxWidth));
            Guard.AgainstNullAndEmpty(path, nameof(path));
            return GetSnippets(textReader, path, maxWidth, newLine);
        }

        static string GetLanguageFromPath(string path)
        {
            var extension = Path.GetExtension(path);
            var s = extension?.TrimStart('.');
            return s ?? string.Empty;
        }

        static IEnumerable<Snippet> GetSnippets(TextReader stringReader, string path, int maxWidth, string newLine)
        {
            var language = GetLanguageFromPath(path);
            LoopStack loopStack = new();
            var index = 0;
            while (true)
            {
                index++;
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

                var trimmedLine = line.Trim();

                if (StartEndTester.IsStart(trimmedLine, path, out var key, out var endFunc))
                {
                    loopStack.Push(endFunc, key, index, maxWidth, newLine);
                    continue;
                }

                if (!loopStack.IsInSnippet)
                {
                    continue;
                }

                if (!loopStack.Current.EndFunc(trimmedLine))
                {
                    Snippet? error = null;
                    try
                    {
                        loopStack.AppendLine(line);
                    }
                    catch (LineTooLongException exception)
                    {
                        var current = loopStack.Current;
                        error = Snippet.BuildError(
                            error: "Line too long: " + exception.Line,
                            path: path,
                            lineNumberInError: current.StartLine + 1,
                            key: current.Key);
                    }

                    if (error != null)
                    {
                        yield return error;
                        break;
                    }

                    continue;
                }

                yield return BuildSnippet(path, loopStack, language, index);
                loopStack.Pop();
            }
        }

        static Snippet BuildSnippet(string path, LoopStack loopStack, string language, int index)
        {
            var loopState = loopStack.Current;

            var value = loopState.GetLines();

            return Snippet.Build(
                startLine: loopState.StartLine,
                endLine: index,
                key: loopState.Key,
                value: value,
                path: path,
                language: language.ToLowerInvariant()
            );
        }
    }
}