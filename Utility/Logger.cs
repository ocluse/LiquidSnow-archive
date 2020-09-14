using System;
using System.Collections.Generic;
using System.IO;

namespace Thismaker.Utility
{
    public static class Logger
    {
        public delegate void Logged();
        public static event Logged OnLogged;
        public static string WorkingDir { get; set; }
        public static LogObject LastLog;

        public static void Log(LogObject log)
        {
            var list = new List<string>();
            list.Add(log.ToString());
            File.AppendAllLines(Path.Combine(WorkingDir, "log.sk"), list);

            LastLog = log;

            //Notify if necessary
            if (log.Notifiable)
                OnLogged?.Invoke();
        }
    }

    public struct LogObject
    {
        public LogLevel Level;
        public bool Notifiable;
        public string Message;

        public LogObject(string Message, LogLevel Level = LogLevel.Info, bool Notifiable = true)
        {
            this.Level = Level;
            this.Notifiable = Notifiable;
            this.Message = Message;
        }

        public override string ToString()
        {
            var prefix = "";
            if (Level == LogLevel.Error) prefix = "Error: ";
            if (Level == LogLevel.Info) prefix = "Info: ";
            if (Level == LogLevel.Warning) prefix = "Warning: ";

            return prefix + Message.ToString();
        }

    }

    public enum LogLevel
    {
        Info, Warning, Error, Success
    }

}
