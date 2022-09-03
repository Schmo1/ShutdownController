using System;
using Diagn = System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;

namespace ShutdownController.Utility
{
    public class MyLogger : ILogger
    {

        // singleton Logger



        private static MyLogger instance;
        private static Logger logger;

        private const string rulesConfName = "MyAppLoggerRules";

        private MyLogger()
        {

        }



        public static MyLogger Instance()
        {
            //Creates a new Instance if it's null
            if (instance == null)
                instance = new MyLogger();
            return instance;
        }

        private Logger GetLogger(string theLogger)
        {
            if (logger == null)
                logger = LogManager.GetLogger(theLogger);
            return logger;

        }



        //Methods to Logg with the caller Name
        public void Info(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null)
        {

            string logMessage = CreateLogString(message, nameOfCaller, sourceFilePath, lineNumberOfCaller);
            if (arg == null)
                GetLogger(rulesConfName).Info(logMessage);
            else
                GetLogger(rulesConfName).Info(logMessage, arg);

#if DEBUG
            Diagn.Debug.WriteLine(logMessage);
#endif
        }



        public void Debug(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null)
        {
            string logMessage = CreateLogString(message, nameOfCaller, sourceFilePath, lineNumberOfCaller);
            if (arg == null)
                GetLogger(rulesConfName).Debug(logMessage);
            else
                GetLogger(rulesConfName).Debug(logMessage, arg);

#if DEBUG
            Diagn.Debug.WriteLine(logMessage);
#endif
        }



        public void Error(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null)
        {
            string logMessage = CreateLogString(message, nameOfCaller, sourceFilePath, lineNumberOfCaller);
            if (arg == null)
                GetLogger(rulesConfName).Error(logMessage);
            else
                GetLogger(rulesConfName).Error(logMessage, arg);

#if DEBUG
            Diagn.Debug.WriteLine(logMessage);
#endif
        }



        public void Warn(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null)
        {
            string logMessage = CreateLogString(message, nameOfCaller, sourceFilePath, lineNumberOfCaller);
            if (arg == null)
                GetLogger(rulesConfName).Warn(logMessage);
            else
                GetLogger(rulesConfName).Warn(logMessage, arg);

#if DEBUG
            Diagn.Debug.WriteLine(logMessage);
#endif
        }







        private string CreateLogString(string message, string nameOfCaller, string sourceFilePath, int lineNumberOfCaller)
        {
            string className = sourceFilePath.Substring(sourceFilePath.LastIndexOf('\\') + 1);

            return "Class |" + className + "| " + "LineNumber |" + lineNumberOfCaller.ToString() + "| Message |" + message + "|";
        }
    }
}
