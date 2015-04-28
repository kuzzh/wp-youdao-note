
using System.Collections.Generic;

namespace YoudaoNoteDataAccess
{
    public class NoteEntityEqualityComparer : IEqualityComparer<NoteEntity>
    {
        public bool Equals(NoteEntity x, NoteEntity y)
        {
            return x.NotePath == y.NotePath;
        }

        public int GetHashCode(NoteEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}
