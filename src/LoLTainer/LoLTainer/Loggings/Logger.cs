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
        private static string _loggingFolder = Path.Combine( AppContext.BaseDirectory, "Logs");
        public static string LoggingFolder
        {
            get
            {
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
            foreach(var item in TransformToLogFiles(logType))
            {
                WriteLineToLogFile(item, message);
            }
        }

        public static void WriteLineToLogFile(LogFile logFile, string message)
        {
            var logFilePath = GetLogFileLocation(logFile);
            if(logFilePath == null)
            {
                return;
            }
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath);
            }
            using (var v = File.AppendText(logFilePath))
            {
                v.WriteLineAsync(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + message).Wait();
            }
        }

        private static IEnumerable<LogFile> TransformToLogFiles(LogType logType)
        {
            var logFiles = new List<LogFile>();
            logFiles.Add(LogFile.Full);
            switch (logType)
            {
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
        LCU
    }

    public enum LogFile
    {
        Full,
        Settings,
        UI,
        IngameAPI,
        LCU
    }
}
