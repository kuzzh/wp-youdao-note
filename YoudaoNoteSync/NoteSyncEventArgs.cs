

namespace YoudaoNoteSync
{
    using System;
using YoudaoNoteDataAccess;

    public class NoteSyncEventArgs : EventArgs
    {
        public int TotalNoteCount { get; set; }
        public int SyncedNoteCount { get; set; }
        public NoteEntity SyncedNote { get; set; }

        public NoteSyncEventArgs(int totalNoteCount, int syncedNoteCount, NoteEntity syncedNote)
        {
            TotalNoteCount = totalNoteCount;
            SyncedNoteCount = syncedNoteCount;
            SyncedNote = syncedNote;
        }
    }
}
