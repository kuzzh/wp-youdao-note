

using System.Collections.Generic;

namespace YoudaoNoteDataAccess
{
    public class NoteEntityComparer : IComparer<NoteEntity>
    {
        public int Compare(NoteEntity x, NoteEntity y)
        {
            return y.ModifyTime.CompareTo(x.ModifyTime);
        }
    }
}
