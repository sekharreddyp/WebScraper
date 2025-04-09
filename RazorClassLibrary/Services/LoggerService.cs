using System;
using System.IO;

namespace WebScrapeApp.Services
{
    public static class LoggerService
    {
        private static readonly string downloadDirectory = @"C:\CustomDownloads";
        private static readonly string LogFilePath = Path.Combine(downloadDirectory, "logs", "application.log");

        static LoggerService()
        {
            // Ensure the logs directory exists
            var logDirectory = Path.GetDirectoryName(LogFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public static void Log(string message)
        {
            try
            {
                var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
    }
}
