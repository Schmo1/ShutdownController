using System.Runtime.CompilerServices;

namespace ShutdownController.Utility
{
    public interface ILogger
    {
        void Debug(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null);
        void Info(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null);
        void Warn(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null);
        void Error(string message, [CallerMemberName] string nameOfCaller = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int lineNumberOfCaller = 0, string arg = null);
    }
}
