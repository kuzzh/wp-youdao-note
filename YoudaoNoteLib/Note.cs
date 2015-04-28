

namespace YoudaoNoteLib
{
    using Newtonsoft.Json;
    using System.Text;
    using YoudaoNoteUtils;

    // “title” : “工作记录”, // 笔记标题
    // “author” : “Tom”, // 笔记作者
    // “source” : “http://note.youdao.com”, // 笔记来源URL
    // “size” : “1024”, // 笔记大小，包括笔记正文及附件
    // “create_time” : “1323310917” // 笔记的创建时间，单位秒
    // “modify_time” : “1323310949” // 笔记的最后修改时间，单位秒
    // “content” : “<p>This is a test note</p> // 笔记正文
    public class Note
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("size")]
        public long Size { get; set; }
        [JsonProperty("create_time")]
        public long CreateTime { get; set; }
        [JsonProperty("modify_time")]
        public long ModifyTime { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("笔记标题：" + Title);
            sb.AppendLine("笔记作者：" + Author);
            sb.AppendLine("笔记来源 URL：" + Source);
            sb.AppendLine("笔记大小：" + Size / 1024 + " KB");
            sb.AppendLine("笔记创建时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(CreateTime));
            sb.AppendLine("笔记最后修改时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(ModifyTime));
            sb.AppendLine("笔记内容前 100 个字符：" + Content.Substring(0, (Content.Length > 100) ? 100 : Content.Length));

            return sb.ToString();
        }
    }
}
