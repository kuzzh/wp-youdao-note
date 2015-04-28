

namespace YoudaoNoteLog
{
    using System;

    public interface ILog
    {
        void Info(string msg);
        void Info(string msg, Exception e);
        void Debug(string msg);
        void Debug(string msg, Exception e);
        void Warn(string msg);
        void Warn(string msg, Exception e);
        void Error(string msg);
        void Error(string msg, Exception e);
        void Fatal(string msg);
        void Fatal(string msg, Exception e);
        string GetLogContent();
        void SafeDeleteFile();
        long GetLogSize();
    }
}
