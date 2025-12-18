static class NewLineConfigReader
{
    public static string ReadNewLine(string directory, IEnumerable<string> mdFiles)
    {
        var newLine = TryReadFromGitAttributes(directory);
        if (newLine != null)
        {
            return newLine;
        }

        newLine = TryReadFromEditorConfig(directory);
        if (newLine != null)
        {
            return newLine;
        }

        return DetectFromFiles(mdFiles);
    }

    static string DetectFromFiles(IEnumerable<string> mdFiles)
    {
        foreach (var mdFile in mdFiles.OrderBy(_ => _.Length))
        {
            using var reader = File.OpenText(mdFile);
            if (reader.TryFindNewline(out var detectedNewLine))
            {
                return detectedNewLine;
            }
        }

        return Environment.NewLine;
    }

    static string? TryReadFromGitAttributes(string directory)
    {
        var gitAttributesPath = FindFileUpward(directory, ".gitattributes");
        if (gitAttributesPath == null)
        {
            return null;
        }

        var lines = File.ReadAllLines(gitAttributesPath);
        return ParseGitAttributesEol(lines);
    }

    static string? ParseGitAttributesEol(string[] lines)
    {
        string? wildcardEol = null;
        string? extensionEol = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
            {
                continue;
            }

            var eolValue = ExtractGitAttributeEol(trimmed);
            if (eolValue == null)
            {
                continue;
            }

            var pattern = GetGitAttributePattern(trimmed);
            if (pattern == "*")
            {
                wildcardEol = eolValue;
            }
            else if (pattern is "*.md" or "*.MD")
            {
                extensionEol = eolValue;
            }
        }

        // More specific pattern wins
        var eol = extensionEol ?? wildcardEol;
        return EolValueToNewLine(eol);
    }

    static string? ExtractGitAttributeEol(string line)
    {
        // Look for eol=lf or eol=crlf in the line
        var eolIndex = line.IndexOf("eol=", StringComparison.OrdinalIgnoreCase);
        if (eolIndex == -1)
        {
            return null;
        }

        var valueStart = eolIndex + 4;
        var valueEnd = valueStart;
        while (valueEnd < line.Length && !char.IsWhiteSpace(line[valueEnd]))
        {
            valueEnd++;
        }

        return line.Substring(valueStart, valueEnd - valueStart).ToLowerInvariant();
    }

    static string GetGitAttributePattern(string line)
    {
        // Pattern is the first whitespace-delimited token
        var end = 0;
        while (end < line.Length && !char.IsWhiteSpace(line[end]))
        {
            end++;
        }

        return line.Substring(0, end);
    }

    static string? TryReadFromEditorConfig(string directory)
    {
        var editorConfigPath = FindFileUpward(directory, ".editorconfig");
        if (editorConfigPath == null)
        {
            return null;
        }

        var lines = File.ReadAllLines(editorConfigPath);
        return ParseEditorConfigEol(lines);
    }

    static string? ParseEditorConfigEol(string[] lines)
    {
        string? globalEol = null;
        string? extensionEol = null;
        var inWildcardSection = false;
        var inExtensionSection = false;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#') || trimmed.StartsWith(';'))
            {
                continue;
            }

            // Check for section headers
            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                var section = trimmed.Substring(1, trimmed.Length - 2);
                inWildcardSection = section == "*";
                inExtensionSection = EditorConfigSectionMatchesMd(section);
                continue;
            }

            // Parse key=value
            var equalsIndex = trimmed.IndexOf('=');
            if (equalsIndex == -1)
            {
                continue;
            }

            var key = trimmed.Substring(0, equalsIndex).Trim().ToLowerInvariant();
            var value = trimmed.Substring(equalsIndex + 1).Trim().ToLowerInvariant();

            if (key == "end_of_line")
            {
                if (inExtensionSection)
                {
                    extensionEol = value;
                }
                else if (inWildcardSection)
                {
                    globalEol = value;
                }
            }
        }

        // More specific section wins
        var eol = extensionEol ?? globalEol;
        return EolValueToNewLine(eol);
    }

    static bool EditorConfigSectionMatchesMd(string section)
    {
        // Handle patterns like *.md, *.{md,txt}, etc.
        if (!section.StartsWith('*'))
        {
            return false;
        }

        var pattern = section.Substring(1);
        if (pattern.Equals(".md", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Handle {md,txt} style patterns like *.{md,txt}
        if (pattern.StartsWith('.') && pattern.Contains('{'))
        {
            var braceStart = pattern.IndexOf('{');
            var braceEnd = pattern.IndexOf('}');
            if (braceStart != -1 && braceEnd > braceStart)
            {
                var extensions = pattern.Substring(braceStart + 1, braceEnd - braceStart - 1).Split(',');
                return extensions.Any(e => e.Trim().Equals("md", StringComparison.OrdinalIgnoreCase));
            }
        }

        return false;
    }

    static string? EolValueToNewLine(string? eolValue) =>
        eolValue switch
        {
            "lf" => "\n",
            "crlf" => "\r\n",
            "cr" => "\r",
            _ => null
        };

    static string? FindFileUpward(string directory, string fileName)
    {
        var current = directory;
        while (current != null)
        {
            var filePath = Path.Combine(current, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            var parent = Directory.GetParent(current);
            current = parent?.FullName;
        }

        return null;
    }
}
