
using System.Diagnostics;
using System.Linq;
using System.Windows;
using YoudaoNoteDataAccess;

namespace YoudaoNoteDatabaseCache
{
    public static class NotebookListCache
    {
        private static readonly object SyncObj = new object();
        private static readonly NotebookCollection NotebookCollection;

        static NotebookListCache()
        {
            NotebookCollection = new NotebookCollection(NotebookDao.Inst.GetAllNotebook());

            NotebookDao.Inst.NotebookCreated += NotebookListCache_NotebookCreated;
            NotebookDao.Inst.NotebookModified += NotebookListCache_NotebookModified;            
        }

        static void NotebookListCache_NotebookModified(NotebookEntity obj)
        {
            Debug.WriteLine("NotebookListCache::笔记本修改事件触发");
            Debug.Assert(obj != null);

            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    notebookModified(obj);
                });
            }
            else
            {
                notebookModified(obj);
            }
            
        }

        private static void notebookModified(NotebookEntity obj)
        {
            lock (SyncObj)
            {
                var findNotebook = NotebookCollection.FirstOrDefault(nb => nb.Id == obj.Id);
                if (null == findNotebook)
                {
                    return;
                }
                NotebookCollection.Remove(findNotebook);
                NotebookCollection.Add(obj);
            }
        }

        static void NotebookListCache_NotebookCreated(NotebookEntity obj)
        {
            Debug.WriteLine("NotebookListCache::笔记本创建事件触发");
            Debug.Assert(obj != null);

            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    notebookCreated(obj);
                });
            }
            else
            {
                notebookCreated(obj);
            }
            
        }

        private static void notebookCreated(NotebookEntity obj)
        {
            lock (SyncObj)
            {
                NotebookCollection.Add(obj);
            }
        }

        public static NotebookCollection GetNotebookCollection()
        {
            return NotebookCollection;
        }

        public static NotebookEntity Find(string notebookPath)
        {
            return NotebookCollection.FirstOrDefault(entity => entity.Path == notebookPath);
        }
        public static int Count
        {
            get { return NotebookCollection.Count; }
        }

        public static void Clear()
        {
            lock (SyncObj)
            {
                if (Deployment.Current.Dispatcher.CheckAccess())
                {
                    NotebookCollection.Clear();
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => NotebookCollection.Clear());
                }
            }
        }
    }
}
