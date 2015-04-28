

namespace YoudaoNoteUtils
{

    public class Resource
    {
        public string Url { get; set; }
        public byte[] Buffer { get; set; }

        public Resource(string url, byte[] buffer)
        {
            Url = url;
            Buffer = buffer;
        }
    }
}
