

namespace WPYoudaoNoteApp
{
    using HtmlAgilityPack;
    using System;
    using System.Linq;
    using YoudaoNoteUtils;

    public static class TidyHtml
    {
        private static string StyleStr;
        public static string TideHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                throw new ArgumentNullException("html");
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // remove all script and style tags.
            doc.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style")
                .ToList()
                .ForEach(n => n.Remove());

            // remove all class and style attributes.
            doc.DocumentNode.Descendants()
                .Where(n => n.Attributes.Contains("class") || n.Attributes.Contains("style"))
                .ToList()
                .ForEach(n =>
                {
                    if (n.Attributes.Contains("class"))
                    {
                        n.Attributes.Remove("class");
                    }
                    if (n.Attributes.Contains("style"))
                    {
                        n.Attributes.Remove("style");
                    }
                });

            var headTag = doc.DocumentNode.SelectSingleNode("//head");
            if (null == headTag)
            {
                headTag = HtmlNode.CreateNode("<head></head>");
                doc.DocumentNode.InsertBefore(headTag, doc.DocumentNode.FirstChild);
            }

            var viewportTag = doc.DocumentNode.SelectSingleNode("//meta[@name=\"viewport\"]");
            if (null == viewportTag)
            {
                viewportTag = HtmlNode.CreateNode("<meta name=\"viewport\" content=\"user-scalable=no\" />");
                headTag.AppendChild(viewportTag);
            }
            else
            {
                viewportTag.SetAttributeValue("content", "user-scalable=no");
            }

            if (string.IsNullOrEmpty(StyleStr))
            {
                StyleStr = IsoStoreUtil.ReadFileAsString("style/global.css");
            }

            var styleNode = HtmlNode.CreateNode("<style type=\"text/css\">" + StyleStr + "</style>");
            headTag.AppendChild(styleNode);

            return doc.DocumentNode.InnerHtml;
        }
    }
}
