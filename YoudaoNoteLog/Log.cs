

namespace YoudaoNoteLog
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Text;
    using System.Threading;
    using YoudaoNoteUtils;

    public class Log : ILog
    {
        private const string LogFileName = "log.txt";
        
        public void Info(string msg)
        {
            ensureFileExist();

            MakeNormalLog(msg, LogLevel.Info);
        }

        public void Info(string msg, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Debug(string msg)
        {
            ensureFileExist();

            MakeNormalLog(msg, LogLevel.Debug);
        }

        public void Debug(string msg, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Warn(string msg)
        {
            ensureFileExist();

            MakeNormalLog(msg, LogLevel.Warn);
        }

        private static void MakeNormalLog(string msg, LogLevel logLevel)
        {
            var data = new StringBuilder();
            data.AppendFormat("记录时间：{0}{1}", DateTime.Now.ToString("G"), Environment.NewLine);
            data.AppendFormat("线程 ID：[{0}]{1}", Thread.CurrentThread.ManagedThreadId, Environment.NewLine);
            data.AppendFormat("日志级别：{0}{1}", logLevel, Environment.NewLine);
            data.AppendFormat("描述：{0}{1}", msg, Environment.NewLine);

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var fs = isoStore.OpenFile(LogFileName, FileMode.Append))
                {
                    using (var bw = new BinaryWriter(fs))
                    {
                        bw.Write(Encoding.UTF8.GetBytes(data.ToString()));
                    }
                }
            }
        }

        public void Warn(string msg, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Error(string msg)
        {
            Error(msg, null);
        }

        public void Error(string msg, Exception e)
        {
            ensureFileExist();

            var data = new StringBuilder();
            data.AppendFormat("记录时间：{0}{1}", DateTime.Now.ToString("G"), Environment.NewLine);
            data.AppendFormat("线程 ID：[{0}]{1}", Thread.CurrentThread.ManagedThreadId, Environment.NewLine);
            data.AppendFormat("日志级别：{0}{1}", LogLevel.Error, Environment.NewLine);
            data.AppendFormat("错误描述：{0}{1}", msg, Environment.NewLine);

            var youdaoNoteException = e as YoudaoNoteException;
            if (null != youdaoNoteException)
            {
                data.AppendFormat("异常信息：{0}{1}", youdaoNoteException.Message, Environment.NewLine);
                data.AppendFormat("错误码：{0}{1}", youdaoNoteException.Code, Environment.NewLine);
                data.AppendFormat("ResponseMessage:{0}{1}", youdaoNoteException.ResponseMessage, Environment.NewLine);
                data.AppendLine(e.StackTrace);
            }
            else
            {
                if (null != e)
                {
                    data.AppendFormat("异常信息：{0}{1}", e.Message, Environment.NewLine);
                    data.AppendLine(e.StackTrace);
                }
            }
            data.AppendLine();

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var fs = isoStore.OpenFile(LogFileName, FileMode.Append))
                {
                    using (var bw = new BinaryWriter(fs))
                    {
                        bw.Write(Encoding.UTF8.GetBytes(data.ToString()));
                    }
                }
            }
        }

        public void Fatal(string msg)
        {
            ensureFileExist();

            MakeNormalLog(msg, LogLevel.Fatal);
        }

        public void Fatal(string msg, Exception e)
        {
            throw new NotImplementedException();
        }

        public string GetLogContent()
        {
            string content = string.Empty;
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(LogFileName))
                {
                    using (var fs = isoStore.OpenFile(LogFileName, FileMode.Open))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
            }
            return content;
        }


        public void SafeDeleteFile()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    isoStore.DeleteFile(LogFileName);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public long GetLogSize()
        {
            long total = 0;
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.FileExists(LogFileName)) return total;
                using (var fs = isoStore.OpenFile(LogFileName, FileMode.Open))
                {
                    total = fs.Length;
                }
            }
            return total;
        }

        private static void ensureFileExist()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(LogFileName)) return;
                var fs = isoStore.CreateFile(LogFileName);
                fs.Close();
            }
        }
    }
}
