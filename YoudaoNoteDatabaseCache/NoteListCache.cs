using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoudaoNoteDataAccess;

namespace YoudaoNoteDatabaseCache
{
    public static class NoteListCache
    {
        private static readonly object LockObj = new object();
        // key: notebookPath, value: notes collection
        private static readonly Dictionary<string, NoteCollection> NoteCollectionDict = new Dictionary<string, NoteCollection>();

        static NoteListCache()
        {
            NoteDao.Inst.NoteCreated += NoteListCache_NoteCreated;
            NoteDao.Inst.NoteDeleted += NoteListCache_NoteDeleted;
            NoteDao.Inst.NoteModified += NoteListCache_NoteModified;
            NoteDao.Inst.NoteCleared += NoteListCache_NoteCleared;

            //initNoteListDictAsync();
        }

        static void NoteListCache_NoteCleared()
        {
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    noteCleared();
                });
            }
            else
            {
                noteCleared();
            }
        }

        static void noteCleared()
        {
            Clear();
        }

        static void NoteListCache_NoteModified(NoteEntity oldNoteEntity, NoteEntity newNoteEntity)
        {
            Debug.WriteLine("NoteListCache::笔记修改事件触发");
            Debug.Assert(oldNoteEntity != null);
            Debug.Assert(newNoteEntity != null);

            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    noteModified(oldNoteEntity, newNoteEntity);
                });
            }
            else
            {
                noteModified(oldNoteEntity, newNoteEntity);
            }
        }

        private static void noteModified(NoteEntity oldNoteEntity, NoteEntity newNoteEntity)
        {
            var belongNotebookPath = oldNoteEntity.NotebookPath;
            NoteCollection noteCollection;
            if (!NoteCollectionDict.TryGetValue(belongNotebookPath, out noteCollection)) return;
            var findNote = noteCollection.FirstOrDefault(n => n.Id == oldNoteEntity.Id);
            if (null == findNote)
            {
                return;
            }
            findNote.Copy(newNoteEntity);
        }

        static void NoteListCache_NoteDeleted(NoteEntity deleteNoteEntity)
        {
            Debug.WriteLine("NoteListCache::笔记删除事件触发");
            Debug.Assert(deleteNoteEntity != null);

            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    noteDeleted(deleteNoteEntity);
                });
            }
            else
            {
                noteDeleted(deleteNoteEntity);
            }
        }

        private static void noteDeleted(NoteEntity deleteNoteEntity)
        {
            var belongNotebookPath = deleteNoteEntity.NotebookPath;

            NoteCollection noteCollection;
            if (NoteCollectionDict.TryGetValue(belongNotebookPath, out noteCollection))
            {
                noteCollection.RemoveById(deleteNoteEntity.Id);
            }
        }

        static void NoteListCache_NoteCreated(NoteEntity newNoteEntity)
        {
            Debug.WriteLine("NoteListCache::笔记创建事件触发");
            Debug.Assert(newNoteEntity != null);

            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    noteCreated(newNoteEntity);
                });
            }
            else
            {
                noteCreated(newNoteEntity);
            }
        }

        private static void noteCreated(NoteEntity newNoteEntity)
        {
            var belongNotebookPath = newNoteEntity.NotebookPath;

            lock (LockObj)
            {
                NoteCollection noteCollection;
                if (NoteCollectionDict.TryGetValue(belongNotebookPath, out noteCollection))
                {
                    var flag = false;
                    for (var i = 0; i < noteCollection.Count; i++)
                    {
                        if (noteCollection[i].ModifyTime >= newNoteEntity.ModifyTime) continue;
                        noteCollection.Insert(i, newNoteEntity);
                        flag = true;
                        break;
                    }
                    if (!flag)
                    {
                        noteCollection.Add(newNoteEntity);
                    }
                }
                else
                {
                    noteCollection = new NoteCollection { newNoteEntity };
                    NoteCollectionDict.Add(belongNotebookPath, noteCollection);
                }
            }
        }

        public static NoteCollection GetNoteCollection(string notebookPath)
        {
            if (string.IsNullOrEmpty(notebookPath))
            {
                throw new ArgumentNullException("notebookPath");
            }
            lock (LockObj)
            {
                NoteCollection noteCollection;

                if (!NoteCollectionDict.TryGetValue(notebookPath, out noteCollection))
                {
                    noteCollection = new NoteCollection(NoteDao.Inst.GetNotesByNotebookPath(notebookPath));
                    NoteCollectionDict.Add(notebookPath, noteCollection);
                }
                noteCollection.Sort(n => n.ModifyTime, ListSortDirection.Descending);
                return noteCollection;
            }
        }

        public static NoteEntity GetNoteByPath(string notebookPath, string notePath)
        {
            if (string.IsNullOrEmpty(notebookPath))
            {
                throw new ArgumentNullException("notebookPath");
            }
            if (string.IsNullOrEmpty(notePath))
            {
                throw new ArgumentNullException("notePath");
            }
            lock (LockObj)
            {
                NoteCollection collection;
                if (NoteCollectionDict.TryGetValue(notebookPath, out collection))
                {
                    return collection.FirstOrDefault(n => n.NotePath == notePath);
                }
            }
            return null;
        }

        public static void Clear()
        {
            lock (LockObj)
            {
                NoteCollectionDict.Clear();
            }
        }

        private static void initNoteListDictAsync()
        {
            Task.Run(() =>
                {
                    var notebooks = NotebookDao.Inst.GetAllNotebook();
                    if (null == notebooks)
                    {
                        return;
                    }
                    foreach (var nb in notebooks)
                    {
                        var noteCollection = new NoteCollection(NoteDao.Inst.GetNotesByNotebookPath(nb.Path));
                        lock (LockObj)
                        {
                            if (!NoteCollectionDict.ContainsKey(nb.Path))
                            {
                                NoteCollectionDict.Add(nb.Path, noteCollection);
                            }
                        }
                    }
                });
        }
    }
}
