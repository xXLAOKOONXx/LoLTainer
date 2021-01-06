using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Loggings
{
    public static class Logger
    {
        private static string _loggingFolder = Path.Combine(AppContext.BaseDirectory, "Logs");
        public static string LoggingFolder
        {
            get
            {
                if (!Directory.Exists(_loggingFolder))
                {
                    Directory.CreateDirectory(_loggingFolder);
                }
                return _loggingFolder;
            }
            set
            {
                if (Directory.Exists(value))
                {
                    _loggingFolder = value;
                }

            }
        }

        public static void Log(LogType logType, string message)
        {
            foreach (var item in TransformToLogFiles(logType))
            {
                WriteLineToLogFile(item, message);
            }
        }

        public static void WriteLineToLogFile(LogFile logFile, string message)
        {
            var logFilePath = GetLogFileLocation(logFile);
            if (logFilePath == null)
            {
                return;
            }
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose();
            }
            var text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + message;
            WriteTextAsync(logFilePath, text).Wait();
        }

        /// <summary>
        /// https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/concepts/async/using-async-for-file-access
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static async Task WriteTextAsync(string filePath, string text)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath, append: true))
                    await streamWriter.WriteLineAsync(text);
            }
            catch (IOException ioex)
            {
                await WriteTextAsync(filePath, text);
            }
            /*
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
            */
        }

        private static IEnumerable<LogFile> TransformToLogFiles(LogType logType)
        {
            var logFiles = new List<LogFile>();
            logFiles.Add(LogFile.Full);
            switch (logType)
            {
                case LogType.Sound: logFiles.Add(LogFile.Sound); break;
                default:
                    break;
            }
            return logFiles;
        }

        private static string GetLogFileLocation(LogFile logFile)
        {
            string fileName;
            switch (logFile)
            {
                case LogFile.Full:
                    fileName = "full.log";
                    break;
                case LogFile.IngameAPI:
                    fileName = "ingameAPI.log";
                    break;
                case LogFile.LCU:
                    fileName = "LCU.log";
                    break;
                case LogFile.Settings:
                    fileName = "Settings.log";
                    break;
                case LogFile.UI:
                    fileName = "UI.log";
                    break;
                case LogFile.Sound:
                    fileName = "Sound.log";
                    break;
                default:
                    return null;
            }
            return Path.Combine(LoggingFolder, fileName);
        }
    }

    public enum LogType
    {
        Settings,
        UI,
        IngameAPI,
        LCU,
        Sound
    }

    public enum LogFile
    {
        Full,
        Settings,
        UI,
        IngameAPI,
        LCU,
        Sound
    }
}
