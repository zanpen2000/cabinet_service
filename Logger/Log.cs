using System;
using System.IO;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Logger
{
    public class Log
    {
        private static readonly object obj = new object();
        private static readonly ILog infologger;
        private static readonly ILog usermsglogger;
        private static readonly ILog errlogger;

        static Log()
        {
            lock (obj)
            {
                var cfgname = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
                XmlConfigurator.ConfigureAndWatch(new FileInfo(cfgname));


                infologger = LogManager.GetLogger("loginfo");
                infologger.Logger.IsEnabledFor(Level.All);

                usermsglogger = LogManager.GetLogger("logusermsg");
                usermsglogger.Logger.IsEnabledFor(Level.All);


                errlogger = LogManager.GetLogger("logerror");
                errlogger.Logger.IsEnabledFor(Level.All);
            }
        }

        public static void AppendErrorInfo(string line)
        {
            errlogger.Error(line);
        }

        public static void AppendErrorInfo(string line, Exception ex)
        {
            errlogger.Error(line, ex);
        }

        public static void AppendUserMessage(string line)
        {
            usermsglogger.Info(line);
        }

        public static void AppendUserMessage(string line, Exception ex)
        {
            usermsglogger.Info(line, ex);
        }

        public static void AppendInfo(string line)
        {
            infologger.Info(line);
        }

        public static void AppendInfo(string line, Exception ex)
        {
            infologger.Info(line, ex);
        }
    }
}