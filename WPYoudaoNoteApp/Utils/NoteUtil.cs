
using HtmlAgilityPack;

namespace WPYoudaoNoteApp.Utils
{
    public static class NoteUtil
    {
        public static bool NeedCache(string noteContent)
        {
            var dom = new HtmlDocument();
            dom.LoadHtml(noteContent);

            var root = dom.DocumentNode;

            var imgNodes = root.Descendants("img");
            foreach (var img in imgNodes)
            {
                // Invalid imgNode tag.
                if (!img.Attributes.Contains("src") || string.IsNullOrEmpty(img.Attributes["src"].Value))
                {
                    continue;
                }
                // The image is from Youdao server.
                if (img.Attributes["src"].Value.Contains("https://note.youdao.com/yws/open/resource/download"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
