

using System.Net;

namespace YoudaoNoteUtils
{
    using System;

    public class YoudaoNoteException : Exception
    {
        public HttpStatusCode Code { get; set; }
        public string ResponseMessage { get; set; }

        public YoudaoNoteException(string message)
            : base(message)
        {
            ResponseMessage = string.Empty;
        }

        public YoudaoNoteException(string message, Exception innerException)
            : base(message, innerException)
        {
            ResponseMessage = string.Empty;
        }

        public YoudaoNoteException(HttpStatusCode code, string message)
        {
            Code = code;
            ResponseMessage = message;
        }
    }
}
