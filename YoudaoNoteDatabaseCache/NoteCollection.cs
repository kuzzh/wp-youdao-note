

using System;
using System.Collections.Generic;
using YoudaoNoteDataAccess;

namespace YoudaoNoteDatabaseCache
{
    public class NoteCollection : SortableObservableCollection<NoteEntity>
    {
        public NoteCollection()
        {
            
        }

        public NoteCollection(List<NoteEntity> list)
            : base(list)
        {
        }

        public void AddRange(IEnumerable<NoteEntity> entities)
        {
            if (null == entities)
            {
                return;
            }
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public void RemoveById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            foreach (var noteEntity in Items)
            {
                if (noteEntity.Id != id) continue;
                Remove(noteEntity);
                break;
            }
        }
    }
}
