

namespace YoudaoNoteLib
{
    using Newtonsoft.Json;
    using System.Text;
    using YoudaoNoteUtils;

    //{"id":"a8f790a9fd63a4f8efa219576985da0c64cffbf2","register_time":1325431526986,
    //"used_size":81905310,"last_login_time":1417348167537,"total_size":5039441028,
    //"last_modify_time":1417350362732,"default_notebook":"/ZI2phvdkbR8","user":"sar***"}
    public sealed class User
    {
        /// <summary>
        /// 用户 ID。
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// 用户注册时间，单位毫秒
        /// </summary>
        [JsonProperty("register_time")]
        public long RegisterTime { get; set; }
        /// <summary>
        /// 用户已经使用了的空间大小，单位字节
        /// </summary>
        [JsonProperty("used_size")]
        public long UsedSize { get; set; }
        /// <summary>
        /// 用户最后登录时间，单位毫秒
        /// </summary>
        [JsonProperty("last_login_time")]
        public long LastLoginTime { get; set; }
        /// <summary>
        /// 用户总的空间大小，单位字节
        /// </summary>
        [JsonProperty("total_size")]
        public long TotalSize { get; set; }
        /// <summary>
        /// 用户最后修改时间，单位毫秒
        /// </summary>
        [JsonProperty("last_modify_time")]
        public long LastModifyTime { get; set; }
        /// <summary>
        /// 该用户的默认笔记本
        /// </summary>
        [JsonProperty("default_notebook")]
        public string DefaultNotebook { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [JsonProperty("user")]
        public string UserName { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("用户 Id：" + Id);
            sb.AppendLine("用户名：" + UserName);
            sb.AppendLine("该用户的默认笔记本：" + DefaultNotebook);
            sb.AppendLine("用户注册时间：" + DateUtils.ConvertFromMillisecondsToLocalDatetime(RegisterTime));
            sb.AppendLine("用户最后登录时间：" + DateUtils.ConvertFromMillisecondsToLocalDatetime(LastLoginTime));
            sb.AppendLine("用户最后修改时间：" + DateUtils.ConvertFromMillisecondsToLocalDatetime(LastModifyTime));
            sb.AppendLine("用户已经使用了的空间大小：" + UsedSize / 1024 + " KB");
            sb.AppendLine("用户总的空间大小：" + TotalSize / 1024 + " KB");

            return sb.ToString();
        }
    }
}
