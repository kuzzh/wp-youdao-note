
using Newtonsoft.Json;
namespace YoudaoNoteLib
{
    public class UploadAttachResult
    {
        /// <summary>
        /// 附件地址。
        /// </summary>
        [JsonProperty("url")]
        public string AttachUrl { get; set; }
        /// <summary>
        /// 附件图标地址。
        /// </summary>
        [JsonProperty("src")]
        public string IconUrl { get; set; }
    }
}
