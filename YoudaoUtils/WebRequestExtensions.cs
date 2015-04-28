
using System.Threading.Tasks;

namespace YoudaoNoteUtils
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;


    public static class WebRequestExtensions
    {
        public static WebResponse GetResponse(this WebRequest request)
        {
            try
            {
                var autoResetEvent = new AutoResetEvent(false);

                var asyncResult = request.BeginGetResponse(r => autoResetEvent.Set(), null);

                // Wait until the call is finished
                autoResetEvent.WaitOne();
                return request.EndGetResponse(asyncResult);
            }
            catch
            {
                throw;
            }
        }


        public static Stream GetRequestStream(this WebRequest request)
        {
            try
            {
                var autoResetEvent = new AutoResetEvent(false);

                var asyncResult = request.BeginGetRequestStream(r => autoResetEvent.Set(), null);

                // Wait until the call is finished
                autoResetEvent.WaitOne();

                return request.EndGetRequestStream(asyncResult);
            }
            catch
            {
                throw;
            }
        }
    }
}
