

namespace YoudaoNoteDataAccess
{
    using System;
    public class NoteAlreadyExistException : Exception
    {
        private string NoteName { get; set; }
        public NoteAlreadyExistException()
        {

        }

        public NoteAlreadyExistException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public NoteAlreadyExistException(string noteName)
        {
            NoteName = noteName;
        }
    }
}
