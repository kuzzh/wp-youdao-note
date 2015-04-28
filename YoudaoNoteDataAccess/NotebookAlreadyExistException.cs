

namespace YoudaoNoteDataAccess
{
    using System;

    public class NotebookAlreadyExistException : Exception
    {
        private readonly string _notebookName;

        public NotebookAlreadyExistException()
        {

        }

        public NotebookAlreadyExistException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public NotebookAlreadyExistException(string notebookName)
        {
            _notebookName = notebookName;
        }
    }
}
