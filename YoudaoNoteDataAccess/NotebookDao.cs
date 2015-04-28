

namespace YoudaoNoteDataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YoudaoNoteLib;

    public class NotebookDao
    {
        public static readonly NotebookDao Inst = new NotebookDao();

        public event Action<NotebookEntity> NotebookCreated;
        public event Action<NotebookEntity> NotebookModified;

        private NotebookDao()
        {
            
        }

        public void ModifyOrAddIfNotExistsAll(List<Notebook> notebooks)
        {
            if (null == notebooks)
            {
                throw new ArgumentNullException("notebooks");
            }
            using (var dc = new MyDataContext())
            {
                foreach (var nb in notebooks)
                {
                    var notebookEntity = dc.NotebookTable.FirstOrDefault(n => n.Path == nb.Path);
                    if (null == notebookEntity)
                    {
                        var entity = new NotebookEntity(nb);
                        dc.NotebookTable.InsertOnSubmit(entity);
                        if (NotebookCreated != null)
                        {
                            NotebookCreated(entity);
                        }
                    }
                    else
                    {
                        if (notebookEntity.Name == nb.Name && notebookEntity.Path == nb.Path &&
                            notebookEntity.NotesNum == nb.NotesNum && notebookEntity.CreateTime == nb.CreateTime &&
                            notebookEntity.ModifyTime == nb.ModifyTime) continue;
                        notebookEntity.Name = nb.Name;
                        notebookEntity.Path = nb.Path;
                        notebookEntity.NotesNum = nb.NotesNum;
                        notebookEntity.CreateTime = nb.CreateTime;
                        notebookEntity.ModifyTime = nb.ModifyTime;

                        if (NotebookModified != null)
                        {
                            NotebookModified(notebookEntity);
                        }
                    }
                }
                dc.SafeSubmitChanges();
            }
        }

        public List<NotebookEntity> GetAllNotebook()
        {
            using (var dc = new MyDataContext())
            {
                return !dc.DatabaseExists()
                    ? new List<NotebookEntity>()
                    : dc.NotebookTable.ToList();

            }
        }

        public int GetTotalNotesNum()
        {
            using (var dc = new MyDataContext())
            {
                return !dc.DatabaseExists()
                    ? 0
                    : Enumerable.Sum(dc.NotebookTable, notebook => notebook.NotesNum);
            }
        }

        public void Clear()
        {
            using (var dc = new MyDataContext())
            {
                dc.NotebookTable.DeleteAllOnSubmit(from e in dc.NotebookTable select e);
                dc.SafeSubmitChanges();
            }
        }
    }
}
