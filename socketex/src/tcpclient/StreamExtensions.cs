using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketEx
{
    public static class StreamExtensions
    {
        private static int parseHeader(Stream stream, out TcpClientResponse tcpClientResponse)
        {
            tcpClientResponse = null;
            var header = new StringBuilder();
            //string headertext = "";
            while (true)
            {
                byte[] data = new byte[1];
                int recv = stream.Read(data, 0, 1);
                char c = (char)data[0];
                header.Append(c);
                if (header.ToString().IndexOf("\r\n\r\n") > 0)
                {
                    tcpClientResponse = new TcpClientResponse();
                    var strHeader = header.ToString();
                    tcpClientResponse.ContentLength = extractContentLength(strHeader);
                    tcpClientResponse.ContentType = extractContentType(strHeader);
                    tcpClientResponse.StatusCode = extractStatusCode(strHeader);
                    tcpClientResponse.StatusDescription = extractStatusDescription(strHeader);
                    //headertext = header.ToString().Substring(start + content.Length);
                    //int end = headertext.IndexOf("\r\n");
                    //headertext = headertext.Substring(0, end); //包体长度
                    break;
                }
            }

            return header.Length;
        }

        private static long extractContentLength(string header)
        {
            var content = "CONTENT-LENGTH:";
            var start = header.ToUpperInvariant().IndexOf(content);
            var temp = header.Substring(start + content.Length);
            var end = temp.IndexOf("\r\n");
            return Int64.Parse(temp.Substring(0, end));
        }

        private static string extractContentType(string header)
        {
            var content = "CONTENT-TYPE:";
            var start = header.ToUpperInvariant().IndexOf(content);
            var temp = header.Substring(start + content.Length);
            var end = temp.IndexOf("\r\n");
            return temp.Substring(0, end);
        }

        private static HttpStatusCode extractStatusCode(string header)
        {
            var content = "HTTP/1.1";
            var start = header.ToUpperInvariant().IndexOf(content);
            return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), header.Substring(start + content.Length + 1, 3));
        }

        private static string extractStatusDescription(string header)
        {
            var content = "HTTP/1.1";
            var start = header.ToUpperInvariant().IndexOf(content);
            var temp = header.Substring(start + content.Length + 5);
            var end = temp.IndexOf("\r\n");
            return temp.Substring(0, end);
        }

        public static TcpClientResponse GetTcpClientResponse(this Stream stream)
        {
            TcpClientResponse tcpClientResponse;

            var headerLength = parseHeader(stream, out tcpClientResponse);
            if (null == tcpClientResponse)
            {
                return null;
            }

            var buffer = new byte[1024];
            var bytesRead = 0;
            var ms = new MemoryStream();

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, bytesRead);
            }

            ms.Position = 0;
            var tempBuffer = new byte[ms.Length];
            ms.Read(tempBuffer, 0, tempBuffer.Length);
            ms.Close();

            tcpClientResponse.ResponseByte = new byte[tempBuffer.Length];
            Array.Copy(tempBuffer, 0, tcpClientResponse.ResponseByte, 0, tempBuffer.Length);

            return tcpClientResponse;
        }
    }
    public class TcpClientResponse
    {
        public long ContentLength { get; internal set; }
        public string ContentType { get; internal set; }
        public HttpStatusCode StatusCode { get; internal set; }
        public string StatusDescription { get; internal set; }
        public byte[] ResponseByte { get; internal set; }
    }
}
