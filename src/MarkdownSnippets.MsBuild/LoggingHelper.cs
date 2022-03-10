using Microsoft.Build.Utilities;

static class LoggingHelper
{
    public static void LogFileError(this TaskLoggingHelper loggingHelper, string message, string? file, int line, int column) =>
        loggingHelper.LogError(null, null, null, file, line, column, 0, 0, message);
}