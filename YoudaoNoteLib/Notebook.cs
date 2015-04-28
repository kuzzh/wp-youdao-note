
namespace YoudaoNoteLib
{
    using Newtonsoft.Json;
    using System.Text;
    using YoudaoNoteUtils;

    // {“path” : “/4AF64012E9864C”, // 笔记本的路径
    // “name” : “笔记本1”, // 笔记本的名称
    // “notes_num” : “3” // 该笔记本中笔记的数目
    // “create_time” : “1323310917” // 笔记本的创建时间，单位秒
    // “modify_time” : “1323310949” // 笔记本的最后修改时间，单位秒
    // }
    public class Notebook
    {
        /// <summary>
        /// 笔记本的路径
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }
        /// <summary>
        /// 笔记本的名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 该笔记本中笔记的数目
        /// </summary>
        [JsonProperty("notes_num")]
        public int NotesNum { get; set; }
        /// <summary>
        /// 笔记本的创建时间，单位秒
        /// </summary>
        [JsonProperty("create_time")]
        public long CreateTime { get; set; }
        /// <summary>
        /// 笔记本的最后修改时间，单位秒
        /// </summary>
        [JsonProperty("modify_time")]
        public long ModifyTime { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("笔记本名称：" + Name);
            sb.AppendLine("笔记本路径：" + Path);
            sb.AppendLine("笔记本中笔记的数目：" + NotesNum);
            sb.AppendLine("笔记本创建时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(CreateTime));
            sb.AppendLine("笔记本最后修改时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(ModifyTime));

            return sb.ToString();
        }
    }
}
