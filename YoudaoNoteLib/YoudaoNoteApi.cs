

namespace YoudaoNoteLib
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using YoudaoNoteUtils;

    public class YoudaoNoteApi : IYoudaoNoteApi
    {
        private readonly string _accessToken;
        public YoudaoNoteApi(string accessToken)
        {
            _accessToken = accessToken;
        }
        public User GetUser()
        {
            var url = "https://note.youdao.com/yws/open/user/get.json?oauth_token=" + _accessToken;
            var requestResult = HttpUtils.SendGetRequest(url);

            if (requestResult.OperateResult == OperateResultEnum.Success)
            {
                return JsonConvert.DeserializeObject(requestResult.Result, typeof(User)) as User;
            }
            throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
        }


        public List<Notebook> GetAllNotebooks()
        {
            var url = "https://note.youdao.com/yws/open/notebook/all.json?oauth_token=" + _accessToken;
            var requestResult = HttpUtils.SendPostRequest(url);

            if (requestResult.OperateResult == OperateResultEnum.Success)
            {
                return JsonConvert.DeserializeObject(requestResult.Result, typeof(List<Notebook>)) as List<Notebook>;
            }
            throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
        }


        public List<string> GetNoteList(string notebook)
        {
            var url = "https://note.youdao.com/yws/open/notebook/list.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("notebook", notebook, PostDataParamType.Field));

            var requestResult = HttpUtils.SendPostRequest(url, postData);

            if (requestResult.OperateResult == OperateResultEnum.Success)
            {
                return JsonConvert.DeserializeObject(requestResult.Result, typeof(List<string>)) as List<string>;
            }
            throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
        }


        public Notebook CreateNotebook(string name)
        {
            var url = "https://note.youdao.com/yws/open/notebook/create.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("name", name, PostDataParamType.Field));

            var requestResult = HttpUtils.SendPostRequest(url, postData);

            if (requestResult.OperateResult == OperateResultEnum.Success)
            {
                return JsonConvert.DeserializeObject(requestResult.Result, typeof(Notebook)) as Notebook;
            }
            throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
        }


        public void DeleteNotebook(string path, long modifyTime = 0)
        {
            var url = "https://note.youdao.com/yws/open/notebook/delete.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("notebook", path, PostDataParamType.Field));

            if (0 != modifyTime)
            {
                postData.Params.Add(new PostDataParam("modify_time", modifyTime.ToString(), PostDataParamType.Field));
            }
            var requestResult = HttpUtils.SendPostRequest(url, postData);

            if (requestResult.OperateResult == OperateResultEnum.Fail)
            {
                throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
            }
        }


        public string CreateNote(string content, string title = "", string author = "", string notebook = "", string source = "", long createTime = 0)
        {
            var url = "https://note.youdao.com/yws/open/note/create.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("content", content, PostDataParamType.Field));

            if (!string.IsNullOrEmpty(source))
            {
                postData.Params.Add(new PostDataParam("source", source, PostDataParamType.Field));
            }
            if (!string.IsNullOrEmpty(author))
            {
                postData.Params.Add(new PostDataParam("author", author, PostDataParamType.Field));
            }
            if (!string.IsNullOrEmpty(title))
            {
                postData.Params.Add(new PostDataParam("title", title, PostDataParamType.Field));
            }
            if (0 != createTime)
            {
                postData.Params.Add(new PostDataParam("create_time", createTime.ToString(), PostDataParamType.Field));
            }
            if (!string.IsNullOrEmpty(notebook))
            {
                postData.Params.Add(new PostDataParam("notebook", notebook, PostDataParamType.Field));
            }
            var requestResult = HttpUtils.SendPostRequestWithMultipart(url, _accessToken, postData);

            if (requestResult.OperateResult == OperateResultEnum.Fail)
                throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
            var jObject = JsonConvert.DeserializeObject(requestResult.Result, typeof(JObject)) as JObject;
            return (jObject != null) ? jObject["path"].ToString() : string.Empty;
        }


        public Note GetNote(string path)
        {
            var url = "https://note.youdao.com/yws/open/note/get.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("path", path, PostDataParamType.Field));

            var requestResult = HttpUtils.SendPostRequest(url, postData);

            if (requestResult.OperateResult == OperateResultEnum.Success)
            {
                return JsonConvert.DeserializeObject(requestResult.Result, typeof(Note)) as Note;
            }
            throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
        }

        public void UpdateNote(string path, string content, string source = "", string author = "", string title = "", long modifyTime = 0)
        {
            var url = "https://note.youdao.com/yws/open/note/update.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("path", path, PostDataParamType.Field));
            if (!string.IsNullOrEmpty(source))
            {
                postData.Params.Add(new PostDataParam("source", source, PostDataParamType.Field));
            }
            if (!string.IsNullOrEmpty(author))
            {
                postData.Params.Add(new PostDataParam("author", author, PostDataParamType.Field));
            }
            if (!string.IsNullOrEmpty(title))
            {
                postData.Params.Add(new PostDataParam("title", title, PostDataParamType.Field));
            }
            postData.Params.Add(new PostDataParam("content", content, PostDataParamType.Field));
            if (0 != modifyTime)
            {
                postData.Params.Add(new PostDataParam("modify_time", modifyTime.ToString(), PostDataParamType.Field));
            }

            var requestResult = HttpUtils.SendPostRequestWithMultipart(url, _accessToken, postData);

            if (requestResult.OperateResult == OperateResultEnum.Fail)
            {
                throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
            }
        }


        public string MoveNote(string path, string notebook)
        {
            var url = "https://note.youdao.com/yws/open/note/move.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("notebook", notebook, PostDataParamType.Field));
            postData.Params.Add(new PostDataParam("path", path, PostDataParamType.Field));

            var requestResult = HttpUtils.SendPostRequest(url, postData);

            if (requestResult.OperateResult == OperateResultEnum.Fail)
                throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
            var jObject = JsonConvert.DeserializeObject(requestResult.Result, typeof(JObject)) as JObject;
            return jObject != null ? jObject["path"].ToString() : string.Empty;
        }


        public void DeleteNote(string path, long modifyTime = 0)
        {
            var url = "https://note.youdao.com/yws/open/note/delete.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("path", path, PostDataParamType.Field));
            if (0 != modifyTime)
            {
                postData.Params.Add(new PostDataParam("modify_time", modifyTime.ToString(), PostDataParamType.Field));
            }
            var requestResult = HttpUtils.SendPostRequest(url, postData);

            if (requestResult.OperateResult == OperateResultEnum.Fail)
            {
                throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
            }
        }


        public string ShareNote(string path)
        {
            var url = "https://note.youdao.com/yws/open/share/publish.json?oauth_token=" + _accessToken;

            var postData = new PostData();
            postData.Params.Add(new PostDataParam("path", path, PostDataParamType.Field));

            var requestResult = HttpUtils.SendPostRequest(url, postData);

            if (requestResult.OperateResult == OperateResultEnum.Fail)
                throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
            var jObject = JsonConvert.DeserializeObject(requestResult.Result, typeof(JObject)) as JObject;
            return jObject != null ? jObject["url"].ToString() : string.Empty;
        }


        public string UploadImage(string filepath)
        {
            var url = "https://note.youdao.com/yws/open/resource/upload.json?oauth_token=" + _accessToken;

            var postData = new PostData();

            // Read file data
            var data = IsoStoreUtil.ReadFileAsByte(filepath);

            var filename = FileUtils.GetFileName(filepath);
            postData.Params.Add(new PostDataParam(filename, data, filepath, PostDataParamType.File));

            var requestResult = HttpUtils.SendPostRequestWithMultipart(url, _accessToken, postData);

            if (requestResult.OperateResult == OperateResultEnum.Fail)
                throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
            var jObject = JsonConvert.DeserializeObject(requestResult.Result, typeof(JObject)) as JObject;
            return jObject != null ? jObject["url"].ToString() : string.Empty;
        }

        public UploadAttachResult UploadAttach(string filepath)
        {
            var url = "https://note.youdao.com/yws/open/resource/upload.json?oauth_token=" + _accessToken;

            var postData = new PostData();

            // Read file data
            var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            var filename = FileUtils.GetFileName(filepath);
            postData.Params.Add(new PostDataParam(filename, data, filepath, PostDataParamType.File));

            var requestResult = HttpUtils.SendPostRequestWithMultipart(url, _accessToken, postData);

            if (requestResult.OperateResult == OperateResultEnum.Success)
            {
                return JsonConvert.DeserializeObject(requestResult.Result, typeof(UploadAttachResult)) as UploadAttachResult;
            }
            throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
        }

        
        public Resource DownloadAttach(string url)
        {
            var path = url + "?oauth_token=" + _accessToken;

            var requestResult = HttpUtils.SendGetRequestWithReturnStream(path);

            if (requestResult.OperateResult == OperateResultEnum.Success)
            {
                return requestResult.Result;
            }
            throw new YoudaoNoteException(requestResult.Code, requestResult.ErrorMsg);
        }
    }
}
