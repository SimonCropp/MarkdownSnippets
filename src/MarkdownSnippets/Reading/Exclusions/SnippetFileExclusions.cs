namespace MarkdownSnippets;

public static class SnippetFileExclusions
{
    public static bool IsBinary(string extension) =>
        binaryFileExtensionsFrozen.Contains(extension);

    public static bool CanContainCommentsExtension(string extension) =>
        !noAcceptCommentsExtensionsFrozen.Contains(extension);

    static FrozenSet<string> noAcceptCommentsExtensionsFrozen;
    static FrozenSet<string> binaryFileExtensionsFrozen;

    static HashSet<string> noAcceptCommentsExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        //files that dont accept comments hence cant contain snippets

        #region NoAcceptCommentsExtensions

        "DotSettings",
        "csv",
        "json",
        "geojson",
        "sln"

        #endregion
    };

    public static void AddNoAcceptCommentsExtensions(params string[] extensions)
    {
        foreach (var extension in extensions)
        {
            noAcceptCommentsExtensions.Add(extension);
        }
        noAcceptCommentsExtensionsFrozen = noAcceptCommentsExtensions.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }

    public static void RemoveNoAcceptCommentsExtensions(params string[] extensions)
    {
        foreach (var extension in extensions)
        {
            noAcceptCommentsExtensions.Remove(extension);
        }
        noAcceptCommentsExtensionsFrozen = noAcceptCommentsExtensions.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }

    static HashSet<string> binaryFileExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            #region BinaryFileExtensions

            "user",
            // extra binary
            "mdb",
            "binlog",
            "shp",
            "dbf",
            "shx",
            "pbf",
            "map",
            "sbn",

            //from https://github.com/sindresorhus/binary-extensions/blob/master/binary-extensions.json
            "3dm",
            "3ds",
            "3g2",
            "3gp",
            "7z",
            "a",
            "aac",
            "adp",
            "ai",
            "aif",
            "aiff",
            "alz",
            "ape",
            "apk",
            "appimage",
            "ar",
            "arj",
            "asf",
            "au",
            "avi",
            "bak",
            "baml",
            "bh",
            "bin",
            "bk",
            "bmp",
            "btif",
            "bz2",
            "bzip2",
            "cab",
            "caf",
            "cgm",
            "class",
            "cmx",
            "cpio",
            "cr2",
            "cur",
            "dat",
            "dcm",
            "deb",
            "dex",
            "djvu",
            "dll",
            "dmg",
            "dng",
            "doc",
            "docm",
            "docx",
            "dot",
            "dotm",
            "dra",
            "DS_Store",
            "dsk",
            "dts",
            "dtshd",
            "dvb",
            "dwg",
            "dxf",
            "ecelp4800",
            "ecelp7470",
            "ecelp9600",
            "egg",
            "eol",
            "eot",
            "epub",
            "exe",
            "f4v",
            "fbs",
            "fh",
            "fla",
            "flac",
            "flatpak",
            "fli",
            "flv",
            "fpx",
            "fst",
            "fvt",
            "g3",
            "gh",
            "gif",
            "graffle",
            "gz",
            "gzip",
            "h261",
            "h263",
            "h264",
            "icns",
            "ico",
            "ief",
            "img",
            "ipa",
            "iso",
            "jar",
            "jpeg",
            "jpg",
            "jpgv",
            "jpm",
            "jxr",
            "key",
            "ktx",
            "lha",
            "lib",
            "lvp",
            "lz",
            "lzh",
            "lzma",
            "lzo",
            "m3u",
            "m4a",
            "m4v",
            "mar",
            "mdi",
            "mht",
            "mid",
            "midi",
            "mj2",
            "mka",
            "mkv",
            "mmr",
            "mng",
            "mobi",
            "mov",
            "movie",
            "mp3",
            "mp4",
            "mp4a",
            "mpeg",
            "mpg",
            "mpga",
            "mxu",
            "nef",
            "npx",
            "numbers",
            "nupkg",
            "o",
            "oga",
            "ogg",
            "ogv",
            "otf",
            "pages",
            "pbm",
            "pcx",
            "pdb",
            "pdf",
            "pea",
            "pgm",
            "pic",
            "png",
            "pnm",
            "pot",
            "potm",
            "potx",
            "ppa",
            "ppam",
            "ppm",
            "pps",
            "ppsm",
            "ppsx",
            "ppt",
            "pptm",
            "pptx",
            "psd",
            "pya",
            "pyc",
            "pyo",
            "pyv",
            "qt",
            "rar",
            "ras",
            "raw",
            "resources",
            "rgb",
            "rip",
            "rlc",
            "rmf",
            "rmvb",
            "rpm",
            "rtf",
            "rz",
            "s3m",
            "s7z",
            "scpt",
            "sgi",
            "shar",
            "snap",
            "sil",
            "sketch",
            "slk",
            "smv",
            "snk",
            "so",
            "stl",
            "suo",
            "sub",
            "swf",
            "tar",
            "tbz",
            "tbz2",
            "tga",
            "tgz",
            "thmx",
            "tif",
            "tiff",
            "tlz",
            "ttc",
            "ttf",
            "txz",
            "udf",
            "uvh",
            "uvi",
            "uvm",
            "uvp",
            "uvs",
            "uvu",
            "viv",
            "vob",
            "war",
            "wav",
            "wax",
            "wbmp",
            "wdp",
            "weba",
            "webm",
            "webp",
            "whl",
            "wim",
            "wm",
            "wma",
            "wmv",
            "wmx",
            "woff",
            "woff2",
            "wrm",
            "wvx",
            "xbm",
            "xif",
            "xla",
            "xlam",
            "xls",
            "xlsb",
            "xlsm",
            "xlsx",
            "xlt",
            "xltm",
            "xltx",
            "xm",
            "xmind",
            "xpi",
            "xpm",
            "xwd",
            "xz",
            "z",
            "zip",
            "zipx"

            #endregion
        };

    public static void AddBinaryFileExtensions(params string[] extensions)
    {
        foreach (var extension in extensions)
        {
            binaryFileExtensions.Add(extension);
        }
        binaryFileExtensionsFrozen = binaryFileExtensions.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }

    public static void RemoveBinaryFileExtensions(params string[] extensions)
    {
        foreach (var extension in extensions)
        {
            binaryFileExtensions.Remove(extension);
        }
        binaryFileExtensionsFrozen = binaryFileExtensions.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }

    static SnippetFileExclusions()
    {
        noAcceptCommentsExtensionsFrozen = noAcceptCommentsExtensions.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
        binaryFileExtensionsFrozen = binaryFileExtensions.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }
}
