

namespace YoudaoNoteLib
{
    using System.Collections.Generic;
    using YoudaoNoteUtils;

    public interface IYoudaoNoteApi
    {
        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <returns>用户信息。</returns>
        User GetUser();
        /// <summary>
        /// 获取所有笔记本。
        /// </summary>
        /// <returns>所有笔记本列表。</returns>
        List<Notebook> GetAllNotebooks();
        /// <summary>
        /// 获取指定笔记本下的所有笔记列表。
        /// </summary>
        /// <param name="notebook">指定笔记本路径。</param>
        /// <returns>所有笔记列表。</returns>
        List<string> GetNoteList(string notebook);
        /// <summary>
        /// 创建笔记本。
        /// </summary>
        /// <param name="name">笔记本名称。</param>
        /// <returns>新创建的笔记本信息。</returns>
        Notebook CreateNotebook(string name);
        /// <summary>
        /// 删除笔记本。
        /// </summary>
        /// <param name="path">笔记本路径。</param>
        /// <param name="modifyTime">删除时间，单位为 秒，如果不指定则使 用系统时间。</param>
        void DeleteNotebook(string path, long modifyTime = 0);

        /// <summary>
        /// 创建笔记。
        /// </summary>
        /// <param name="content">笔记正文。</param>
        /// <param name="title">笔记标题。</param>
        /// <param name="author">笔记作者。</param>
        /// <param name="notebook">该新建笔记所属的笔记本路径。</param>
        /// <param name="source">笔记来源 URL。</param>
        /// <param name="createTime">创建时间，单位为秒，如果不指定则使用系统时间。</param>
        /// <returns>新建笔记的路径。</returns>
        string CreateNote(string content, string title = "", string author = "", string notebook = "", string source = "", long createTime = 0);
        /// <summary>
        /// 查看笔记。
        /// </summary>
        /// <param name="path">笔记路径。</param>
        /// <returns>返回该笔记的相关信息。</returns>
        Note GetNote(string path);
        /// <summary>
        /// 修改笔记。
        /// </summary>
        /// <param name="path">修改笔记的路径。</param>
        /// <param name="content">修改后的笔记正文。</param>
        /// <param name="source">修改后的笔记来源 URL。</param>
        /// <param name="author">修改后的笔记作者。</param>
        /// <param name="title">修改后的笔记标题。</param>
        /// <param name="modifyTime">修改时间，单位为秒，如果不指定则使用系统时间。</param>
        void UpdateNote(string path, string content, string source = "", string author = "", string title = "", long modifyTime = 0);
        /// <summary>
        /// 移动笔记。
        /// </summary>
        /// <param name="path">想要移动的笔记原路径。</param>
        /// <param name="notebook">目标笔记本的路径。</param>
        /// <returns>返回移动后的笔记路径。</returns>
        string MoveNote(string path, string notebook);
        /// <summary>
        /// 删除笔记。
        /// </summary>
        /// <param name="path">想要删除的笔记原路径。</param>
        /// <param name="modifyTime">修改时间，单位为秒，如果不指定则使用系统时间。</param>
        void DeleteNote(string path, long modifyTime = 0);

        /// <summary>
        /// 分享笔记链接。
        /// </summary>
        /// <param name="path">想要分享的笔记的路径。</param>
        /// <returns>返回该分享笔记的链接。</returns>
        /// <remarks>重复分享同一篇笔记得到的链接是相同的。</remarks>
        string ShareNote(string path);
        /// <summary>
        /// 上传图片。
        /// </summary>
        /// <param name="filepath">上传的图片路径。</param>
        /// <returns>返回该图片的链接 URL 。</returns>
        /// <remarks>Multipart File / 附件大小限制 25 M 。</remarks>
        string UploadImage(string filepath);
        /// <summary>
        /// 上传附件。
        /// </summary>
        /// <param name="filepath">上传的附件路径。</param>
        /// <returns>包含附件地址和为附件生成的图标的地址。</returns>
        /// <remarks>Multipart File / 附件大小限制 25 M 。</remarks>
        UploadAttachResult UploadAttach(string filepath);
        /// <summary>
        /// 下载附件/图片/图标。
        /// </summary>
        /// <param name="url">附件/图片/图标的地址。</param>
        /// <returns>附件/图片/图标资源。</returns>
        Resource DownloadAttach(string url);
    }
}
