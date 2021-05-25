using System;
using System.Collections.Generic;
using System.IO;

namespace Thismaker.Core.Logging
{
    /// <summary>
    /// Provides an easy way to quickly log without the complexities
    /// </summary>
    public static class Logger
    {
        public static event Action<LogObject> LogEntered;
        public static string LogFilePath { get; set; }

        /// <summary>
        /// Logs to the set <see cref="LogFilePath"/>. If no file is set, an exception will be thrown.
        /// The log will raise the <see cref="LogEntered"/> event that will be experienced evironment wide.
        /// </summary>
        /// <param name="log">The <see cref="LogObject"/> to enter.</param>
        public static void Log(LogObject log)
        {
            if (LogFilePath == null) throw new NullReferenceException("No log file was specified");
            var list = new List<string>
            {
                log.ToString()
            };
            File.AppendAllLines(LogFilePath, list);
            if (log.Notifiable)
                LogEntered?.Invoke(log);
        }

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="message">The message of the error</param>
        public static void Error(string message)
        {
            var obj = new LogObject
            {
                Level = LogLevel.Error,
                Message = message,
                Notifiable = true,
                Timestamp = DateTime.Now
            };

            Log(obj);
        }

        /// <summary>
        /// Log some info
        /// </summary>
        /// <param name="message">The message of the info</param>
        public static void Info(string message)
        {
            var obj = new LogObject
            {
                Level = LogLevel.Info,
                Message = message,
                Notifiable = true,
                Timestamp = DateTime.Now
            };

            Log(obj);
        }

        /// <summary>
        /// Log some success
        /// </summary>
        /// <param name="message">The message of the success</param>
        public static void Success(string message)
        {
            var obj = new LogObject
            {
                Level = LogLevel.Success,
                Message = message,
                Notifiable = true,
                Timestamp = DateTime.Now
            };

            Log(obj);
        }

        /// <summary>
        /// Log a warning
        /// </summary>
        /// <param name="message">The message of the warning</param>
        public static void Warning(string message)
        {
            var obj = new LogObject
            {
                Level = LogLevel.Warning,
                Message = message,
                Notifiable = true,
                Timestamp = DateTime.Now
            };

            Log(obj);
        }
    }

    public struct LogObject
    {
        public LogLevel Level { get; set; }
        public bool Notifiable { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Creates a new LogObject, with the default timestamp set to the UTC now time.
        /// </summary>
        /// <param name="message">The message of the log</param>
        /// <param name="level">The level of the log, that is also used for verbosity</param>
        /// <param name="notifiable">Whether the logging of this log should raise a <see cref="Logger.LogEntered"/> event</param>
        public LogObject(string message, LogLevel level = LogLevel.Info, bool notifiable = true)
        {
            Level = level;
            Notifiable = notifiable;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            var prefix = Timestamp.ToString();
            if (Level == LogLevel.Error) prefix += " Error: ";
            if (Level == LogLevel.Info) prefix += " Info: ";
            if (Level == LogLevel.Warning) prefix += " Warning: ";

            return prefix + Message.ToString();
        }
    }

    public enum LogLevel
    {
        Success, Info, Warning, Error
    }

}
