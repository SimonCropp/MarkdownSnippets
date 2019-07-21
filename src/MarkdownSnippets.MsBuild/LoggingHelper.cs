using Microsoft.Build.Utilities;

static class LoggingHelper
{
    public static void LogFileError(this TaskLoggingHelper loggingHelper, string message, string file, int lineNumber)
    {
        loggingHelper.LogError(null, null, null, file, lineNumber, 0, 0, 0, message);
    }
}