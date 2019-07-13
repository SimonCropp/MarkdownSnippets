using Microsoft.Build.Utilities;

static class LoggingHelper
{
    public static void LogFileError(this TaskLoggingHelper loggingHelper, string message, string file, int line)
    {
        loggingHelper.LogError(null, null, null, file, line, 0, 0, 0, message);
    }
}