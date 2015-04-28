
namespace YoudaoNoteUtils
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;


    public class PostData
    {
        public List<PostDataParam> Params { get; private set; }

        public PostData()
        {
            Params = new List<PostDataParam>();
        }


        /// <summary>
        /// Returns the parameters array formatted for multi-part/form data
        /// </summary>
        /// <returns></returns>
        public byte[] GetPostDataWithMultipart(string boundary)
        {
            var ms = new MemoryStream();

            foreach (var p in Params)
            {
                if (p.Type == PostDataParamType.File)
                {
                    var header = string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\";\r\nContent-Type: application/octet-stream\r\n\r\n",
                        boundary,
                        p.FilePath);
                    ms.Write(Encoding.UTF8.GetBytes(header), 0, Encoding.UTF8.GetByteCount(header));

                    ms.Write((byte[])p.Value, 0, ((byte[])p.Value).Length);
                }
                else
                {
                    var postData = string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        p.Name,
                        p.Value);
                    ms.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
                }
            }
            var footer = "\r\n--" + boundary + "--\r\n";
            ms.Write(Encoding.UTF8.GetBytes(footer), 0, Encoding.UTF8.GetByteCount(footer));

            ms.Position = 0;
            var tempBuffer = new byte[ms.Length];
            ms.Read(tempBuffer, 0, tempBuffer.Length);
            ms.Close();  

            return tempBuffer;
        }

        public string GetPostData()
        {
            var sb = new StringBuilder();
            foreach (var p in Params)
            {
                sb.AppendFormat("{0}={1}&", p.Name, p.Value);
            }
            return sb.ToString();
        }

        
    }
}
