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
        /// <summary>
        /// An event that is raised whenever a log is entered.
        /// </summary>
        public static event Action<LogObject> LogEntered;
        
        /// <summary>
        /// The path to the file to write the logs.
        /// </summary>
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

    /// <summary>
    /// Represents a log
    /// </summary>
    public struct LogObject
    {
        /// <summary>
        /// Gets or sets the level of severity of the log.
        /// </summary>
        public LogLevel Level { get; set; }
        
        /// <summary>
        /// Gets or sets a value determining whether the log will invoke the <see cref="Logger.LogEntered"/> event when logged.
        /// </summary>
        public bool Notifiable { get; set; }
        
        /// <summary>
        /// Gets or sets the message of the log.
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp of the log.
        /// </summary>
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

        /// <summary>
        /// A neatly formatted string representing the log
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{Timestamp:dd/MM/yyyy hh:mm:ss tt}] {Level.ToString().ToUpperInvariant()}: {Message}";
        }
    }

    /// <summary>
    /// Represents a log severity level
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// A verbose log
        /// </summary>
        Verbose,
        /// <summary>
        /// A debug log
        /// </summary>
        Debug,
        /// <summary>
        /// A successful log
        /// </summary>
        Success,
        /// <summary>
        /// An info log
        /// </summary>
        Info,
        /// <summary>
        /// A warning log
        /// </summary>
        Warning,
        /// <summary>
        /// An error log
        /// </summary>
        Error,
        /// <summary>
        /// A failure log
        /// </summary>
        Failure,

    }

}
