

using System.Collections.Generic;
using YoudaoNoteDataAccess;

namespace YoudaoNoteDatabaseCache
{
    public class NoteEntityComparer : IComparer<NoteEntity>
    {
        public int Compare(NoteEntity x, NoteEntity y)
        {
            return y.ModifyTime.CompareTo(x.ModifyTime);
        }
    }
}
