

using System.Net;

namespace YoudaoNoteUtils
{
    public enum OperateResultEnum
    {
        Success,
        Fail
    }

    /// <summary>
    /// 请求返回的结果。
    /// </summary>
    public class RequestResult<TResult>
    {
        /// <summary>
        /// 指示操作是否成功。
        /// </summary>
        public OperateResultEnum OperateResult { get; set; }
        
        /// <summary>
        /// 请求返回的状态码。
        /// </summary>
        public HttpStatusCode Code { get; set; }
        /// <summary>
        /// 错误时的提示信息。
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 成功时的返回值。
        /// </summary>
        public TResult Result { get; set; }
    }
}
