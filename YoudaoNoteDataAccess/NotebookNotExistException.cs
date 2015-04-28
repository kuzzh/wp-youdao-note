

namespace YoudaoNoteDataAccess
{
    using System;

    public class NotebookNotExistException : Exception
    {
        private readonly string _notebookName;

        public NotebookNotExistException(string notebookName)
        {
            _notebookName = notebookName;
        }
    }
}
