

namespace YoudaoNoteDataAccess
{
    using System;

    public class NoteNotExistException : Exception
    {
        private readonly string _noteName;

        public NoteNotExistException()
        {

        }

        public NoteNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public NoteNotExistException(string noteName)
        {
            _noteName = noteName;
        }
    }
}
