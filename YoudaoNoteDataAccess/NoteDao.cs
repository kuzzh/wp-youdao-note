
namespace YoudaoNoteDataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class NoteDao
    {
        private const string EmptyNoteTitle = "无标题笔记";

        public static readonly NoteDao Inst = new NoteDao();

        public event Action<NoteEntity> NoteCreated;
        public event Action<NoteEntity> NoteDeleted;
        public event Action<NoteEntity, NoteEntity> NoteModified;
        public event Action NoteCleared;
        public event Action NoteChanged;

        private NoteDao()
        {

        }

        public void AddIfNotExist(NoteEntity note)
        {
            if (null == note)
            {
                throw new ArgumentNullException("note");
            }

            using (var dc = new MyDataContext())
            {
                //var a = new System.IO.StringWriter();

                //dc.Log = a;

                var entity = dc.NoteTable.FirstOrDefault(n => n.NotePath == note.NotePath);
                if (null != entity) return;
                ensureTitleIsNotEmpty(note);
                dc.NoteTable.InsertOnSubmit(note);
                dc.SafeSubmitChanges();
            }
            //Debug.WriteLine(a);
            if (NoteCreated != null)
            {
                NoteCreated(note);
            }

            if (NoteChanged != null)
            {
                NoteChanged();
            }
        }

        public void BatchOperate(Dictionary<NoteBatchOperateEnum, List<NoteEntity>> dic)
        {
            if (null == dic)
            {
                throw new ArgumentNullException("dic");
            }
            var isNoteChangedFlag = false;
            using (var dc = new MyDataContext())
            {
                foreach (var noteEntity in dic[NoteBatchOperateEnum.Add])
                {
                    if (null == noteEntity) continue;
                    var entity = dc.NoteTable.FirstOrDefault(n => n.NotePath == noteEntity.NotePath);
                    if (null != entity) return;
                    ensureTitleIsNotEmpty(noteEntity);
                    dc.NoteTable.InsertOnSubmit(noteEntity);

                    if (NoteCreated != null)
                    {
                        NoteCreated(noteEntity);
                    }
                    isNoteChangedFlag = true;
                }
                foreach (var noteEntity in dic[NoteBatchOperateEnum.Modify])
                {
                    var note = dc.NoteTable.FirstOrDefault(n => n.Id == noteEntity.Id);
                    if (null == note)
                    {
                        return;
                    }
                    ensureTitleIsNotEmpty(noteEntity);
                    var oldNote = note.DeepClone();
                    note.NotePath = noteEntity.NotePath;
                    note.Title = noteEntity.Title;
                    note.CreateTime = noteEntity.CreateTime;
                    note.ModifyTime = noteEntity.ModifyTime;
                    note.Size = noteEntity.Size;
                    note.Source = noteEntity.Source;
                    note.NotebookName = noteEntity.NotebookName;
                    note.NotebookPath = noteEntity.NotebookPath;
                    note.Content = noteEntity.Content;
                    note.Author = noteEntity.Author;
                    note.NoteStatus = noteEntity.NoteStatus;

                    if (NoteModified != null)
                    {
                        NoteModified(oldNote, note);
                    }
                    isNoteChangedFlag = true;
                }

                foreach (var noteEntity in dic[NoteBatchOperateEnum.Delete])
                {
                    var entity = dc.NoteTable.FirstOrDefault(n => n.Id == noteEntity.Id);
                    if (null == entity) return;
                    dc.NoteTable.DeleteOnSubmit(entity);

                    if (NoteDeleted != null)
                    {
                        NoteDeleted(entity);
                    }
                    isNoteChangedFlag = true;
                }

                dc.SafeSubmitChanges();
            }
            if (isNoteChangedFlag)
            {
                if (NoteChanged != null)
                {
                    NoteChanged();
                }
            }
        }

        public bool NoteExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            using (var dc = new MyDataContext())
            {
                var note = dc.NoteTable.FirstOrDefault(n => n.NotePath == path);
                return (note != null);
            }
        }

        /// <summary>
        /// 如果笔记存在，则将笔记删除。
        /// </summary>
        /// <param name="id"></param>
        public void DeleteIfExist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.NoteTable.FirstOrDefault(n => n.Id == id);
                if (null == entity) return;
                dc.NoteTable.DeleteOnSubmit(entity);
                dc.SafeSubmitChanges();

                if (NoteDeleted != null)
                {
                    NoteDeleted(entity);
                }

                if (NoteChanged != null)
                {
                    NoteChanged();
                }
            }
        }

        public void DeleteAllIfExist(List<string> ids)
        {
            if (null == ids)
            {
                throw new ArgumentNullException("ids");
            }
            var isNoteChangedFlag = false;
            using (var dc = new MyDataContext())
            {
                foreach (var id in ids)
                {
                    var entity = dc.NoteTable.FirstOrDefault(n => n.Id == id);
                    if (null == entity) return;
                    dc.NoteTable.DeleteOnSubmit(entity);
                    
                    if (NoteDeleted != null)
                    {
                        NoteDeleted(entity);
                    }
                    isNoteChangedFlag = true;
                }
                dc.SafeSubmitChanges();
            }
            if (isNoteChangedFlag)
            {
                if (NoteChanged != null)
                {
                    NoteChanged();
                }
            }
        }

        /// <summary>
        /// 将笔记标记为已删除。
        /// </summary>
        /// <param name="id"></param>
        public void MarkAsDeletedIfExist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.NoteTable.FirstOrDefault(n => n.Id == id);
                if (null == entity) return;
                entity.NoteStatus = NoteStatus.Deleted;
                dc.SafeSubmitChanges();
            }
        }

        public void ModifyIfExist(NoteEntity entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException("entity");
            }

            using (var dc = new MyDataContext())
            {
                //var a = new System.IO.StringWriter();
                //dc.Log = a;

                var note = dc.NoteTable.FirstOrDefault(n => n.Id == entity.Id);
                if (null == note)
                {
                    Debug.WriteLine("ModifyIfExist::note is null.");
                    return;
                }
                ensureTitleIsNotEmpty(entity);
                var oldNote = note.DeepClone();
                note.NotePath = entity.NotePath;
                note.Title = entity.Title;
                note.CreateTime = entity.CreateTime;
                note.ModifyTime = entity.ModifyTime;
                note.Size = entity.Size;
                note.Source = entity.Source;
                note.NotebookName = entity.NotebookName;
                note.NotebookPath = entity.NotebookPath;
                note.Content = entity.Content;
                note.Author = entity.Author;
                note.NoteStatus = entity.NoteStatus;

                dc.SafeSubmitChanges();

                //Debug.WriteLine(a);

                if (NoteModified != null)
                {
                    NoteModified(oldNote, note);
                }

                if (NoteChanged != null)
                {
                    NoteChanged();
                }
            }
        }

        public NoteEntity GetNoteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.NoteTable.FirstOrDefault(n => n.Id == id);
                return entity;
            }
        }

        public NoteEntity GetNoteByNotePath(string notePath)
        {
            if (string.IsNullOrEmpty(notePath))
            {
                throw new ArgumentNullException("notePath");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.NoteTable.FirstOrDefault(n => n.NotePath == notePath);
                return entity;
            }
        }
        public List<NoteEntity> GetLocalCreatedNotes(string notebookName)
        {
            if (string.IsNullOrEmpty(notebookName))
            {
                throw new ArgumentNullException("notebookName");
            }
            using (var dc = new MyDataContext())
            {
                var entities = from n in dc.NoteTable
                    where n.NotebookName == notebookName && n.NoteStatus == NoteStatus.Added
                    select n;
                return entities.ToList();
            }
        }

        public List<NoteEntity> GetAllLocalCreatedNotes()
        {
            using (var dc = new MyDataContext())
            {
                var entities = from n in dc.NoteTable
                    where n.NoteStatus == NoteStatus.Added
                    select n;
                return entities.ToList();
            }
        }

        public NoteEntity GetNoteByPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.NoteTable.FirstOrDefault(n => n.NotePath == path);
                return entity;
            }
        }

        public List<NoteEntity> GetNotesByNotebookPath(string notebookPath)
        {
            
            if (string.IsNullOrEmpty(notebookPath))
            {
                throw new ArgumentNullException("notebookPath");
            }
            using (var dc = new MyDataContext())
            {
                var entities = from n in dc.NoteTable
                    where n.NotebookPath == notebookPath && n.NoteStatus != NoteStatus.Deleted
                    orderby n.ModifyTime descending
                    select n;

                return entities.ToList();
            }
            
        }

        public List<NoteEntity> GetAllNote()
        {
            using (var dc = new MyDataContext())
            {
                var entities = from n in dc.NoteTable
                    where n.NoteStatus != NoteStatus.Deleted
                    orderby n.ModifyTime descending
                    select n;
                return entities.ToList();
            }
        }

        public void Clear()
        {
            using (var dc = new MyDataContext())
            {
                dc.NoteTable.DeleteAllOnSubmit(from e in dc.NoteTable select e);
                dc.SafeSubmitChanges();
            }
            if (NoteCleared != null)
            {
                NoteCleared();
            }
            if (NoteChanged != null)
            {
                NoteChanged();
            }
        }

        private static void ensureTitleIsNotEmpty(NoteEntity note)
        {
            if (string.IsNullOrEmpty(note.Title))
            {
                note.Title = EmptyNoteTitle;
            }
        }
    }
}
