

namespace YoudaoNoteSync
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Globalization;
    using HtmlAgilityPack;
    using YoudaoNoteDataAccess;
    using YoudaoNoteLib;
    using YoudaoNoteUtils;
    using System.Threading;
    using YoudaoNoteDatabaseCache;
    using System.IO;

    public class SyncCore
    {
        private bool _cancelSync;
        private double _screenWidth;
        private YoudaoNoteApi _api;
        private static readonly SyncCore Inst = new SyncCore();

        public event Action<NoteSyncEventArgs> SyncNoteChanged;

        public bool CancelSync
        {
            private get
            {
                lock (this)
                {
                    return _cancelSync;
                }
            }
            set
            {
                lock (this)
                {
                    _cancelSync = value;
                }
            }
        }

        private SyncCore()
        {

        }

        #region Public Methods

        public static SyncCore GetInst()
        {
            return Inst;
        }

        public void InitializeSyncCore(string accessToken, double screenWidth)
        {
            _screenWidth = screenWidth;
            _api = new YoudaoNoteApi(accessToken);

            Debug.WriteLine("ScreenWidth:" + screenWidth);
            Debug.WriteLine("AccessToken:" + accessToken);
            Debug.WriteLine("SyncCore initialized.");
        }

        public async Task SyncNotebookNotesAsync(string notebookName, string notebookPath, Action<SyncCompletedType, object> syncCompleted)
        {
            if (string.IsNullOrEmpty(notebookName))
            {
                throw new ArgumentNullException("notebookName");
            }
            if (string.IsNullOrEmpty(notebookPath))
            {
                throw new ArgumentNullException("notebookPath");
            }

            await Task.Run(async () =>
                {
                    try
                    {
                        await _syncNotebookNotesAsync(notebookName, notebookPath, syncCompleted);
                    }
                    catch (Exception e)
                    {
                        if (null != syncCompleted)
                        {
                            syncCompleted(SyncCompletedType.Failed, e);
                        }
                    }
                });
        }

        public async Task SyncNoteAsync(NoteEntity noteEntity, Action<SyncCompletedType, object> syncCompleted)
        {
            await Task.Run(() =>
                {
                    try
                    {
                        InternalSyncNote(noteEntity, syncCompleted);
                    }
                    catch (Exception e)
                    {
                        if (null != syncCompleted)
                        {
                            syncCompleted(SyncCompletedType.Failed, e);
                        }
                    }
                });
        }

        public async Task SyncNotebooksAsync(Action<SyncCompletedType, object> syncCompleted)
        {
            await Task.Run(() =>
                {
                    try
                    {
                        performSyncNotebook(syncCompleted);
                    }
                    catch (Exception e)
                    {
                        if (null != syncCompleted)
                        {
                            syncCompleted(SyncCompletedType.Failed, e);
                        }
                    }
                });
        }

        /// <summary>
        /// 将笔记中图片的本地地址替换成远程地址。
        /// </summary>
        /// <param name="noteContent"></param>
        /// <returns></returns>
        public string ConvertImageLocalPathToRemoteUrl(string noteContent)
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

                var entity = ImageDao.GetImageByLocalSavePath(img.Attributes["src"].Value);
                if (entity != null)
                {
                    // Replace the image's src attribute to local path.
                    img.Attributes["src"].Value = entity.ImgRemoteUrl;
                }
                else // 在本地数据库没有找到相关图片记录说明是本地新添加的图片，则上传到服务器
                {
                    var imgUrl = _api.UploadImage(Path.Combine("Images", Path.GetFileName(img.Attributes["src"].Value)));
                    img.Attributes["src"].Value = imgUrl;
                }
            }
            return root.InnerHtml;
        }

        public async Task DeleteNoteAsync(NoteEntity entity, Action<SyncCompletedType, object> deleteCompleted)
        {
            if (null == entity)
            {
                throw new ArgumentNullException("entity");
            }
            
            await Task.Run(() => InternalDeleteNote(entity, deleteCompleted));
        }

        /// <summary>
        /// 将笔记中的远程图片下载到本地，并将远程地址替换成本地地址。
        /// </summary>
        /// <param name="notePath"></param>
        /// <param name="noteContent"></param>
        /// <returns></returns>
        public async Task<string> DownloadImageToLocalAsync(string notePath, string noteContent, Action<int, int> downloadImageCountChanged = null)
        {
            var dom = new HtmlDocument();
            dom.LoadHtml(noteContent);

            var doc = dom.DocumentNode;

            var lstImgNodeToDownload = ExtractImgNodes(doc);

            await Task.Run(() =>
                {
                    var downloadCount = 1;
                    var totalCount = lstImgNodeToDownload.Count();
                    foreach (var imgNode in lstImgNodeToDownload)
                    {
                        InternalDownloadImage(notePath, imgNode);
                        if (null != downloadImageCountChanged)
                        {
                            downloadImageCountChanged(downloadCount, totalCount);
                        }
                        downloadCount++;
                    }
                });
            
            return doc.InnerHtml;
        }

        private static IEnumerable<HtmlNode> ExtractImgNodes(HtmlNode root)
        {
            var imgNodes = root.Descendants("img");
            var lstImgNodeToDownload = new List<HtmlNode>();
            foreach (var imgNode in imgNodes)
            {
                // Invalid imgNode tag.
                if (!imgNode.Attributes.Contains("src") || string.IsNullOrEmpty(imgNode.Attributes["src"].Value))
                {
                    continue;
                }
                // The image is from Youdao server.
                if (imgNode.Attributes["src"].Value.Contains("https://note.youdao.com/yws/open/resource/download"))
                {
                    lstImgNodeToDownload.Add(imgNode);
                }
            }
            return lstImgNodeToDownload;
        }


        private async void InternalDownloadImage(string notePath, HtmlNode imgNode)
        {
            var imgUrl = imgNode.Attributes["src"].Value;
            var imgUrlSplits = imgUrl.Split('/');
            var fileName = imgUrlSplits[imgUrlSplits.Length - 1];

            ImageEntity image;
            var savePath = !ImageDao.ImageExists(fileName, out image)
                ? downloadAndSaveImageToLocal(notePath, imgNode, imgUrl, fileName)
                : image.ImgLocalSavePath;
            // Replace the image's src attribute to local path.
            imgNode.Attributes["src"].Value = savePath;
        }
        #endregion

        #region Private Methods
        private void InternalDeleteNote(NoteEntity entity, Action<SyncCompletedType, object> deleteCompleted)
        {
            try
            {
                deleteNote(entity);
                if (null != deleteCompleted)
                {
                    deleteCompleted(SyncCompletedType.Note, null);
                }
            }
            catch (Exception e)
            {
                if (null != deleteCompleted)
                {
                    deleteCompleted(SyncCompletedType.Failed, e);
                }
            }
        }

        private void InternalSyncNote(NoteEntity noteEntity, Action<SyncCompletedType, object> syncCompleted)
        {
            try
            {
                if (noteEntity.NoteStatus == NoteStatus.Added) // 新增加的笔记
                {
                    var content = ConvertImageLocalPathToRemoteUrl(noteEntity.Content);

                    var path = _api.CreateNote(content, noteEntity.Title, noteEntity.Author,
                        noteEntity.NotebookPath,
                        noteEntity.Source);
                    var note = _api.GetNote(path);
                    noteEntity.CreateTime = note.CreateTime;
                    noteEntity.ModifyTime = note.ModifyTime;
                    noteEntity.NotePath = path;
                    noteEntity.Size = note.Size;
                    noteEntity.NoteStatus = NoteStatus.Normal;
                    NoteDao.Inst.ModifyIfExist(noteEntity);
                }
                else
                {
                    syncNote(new NoteSync(noteEntity.NotePath, noteEntity.NotebookName, noteEntity.NotebookPath));
                }
                if (null != syncCompleted)
                {
                    syncCompleted(SyncCompletedType.Note, "笔记同步成功");
                }
            }
            catch (YoudaoNoteException e)
            {
                if (null != syncCompleted)
                {
                    syncCompleted(SyncCompletedType.Failed, e);
                }
            }
            catch (Exception e1)
            {
                if (null != syncCompleted)
                {
                    syncCompleted(SyncCompletedType.Failed, e1);
                }
            }
        }

        private async Task _syncNotebookNotesAsync(string notebookName, string notebookPath, Action<SyncCompletedType, object> syncCompleted)
        {
            try
            {
                var lstNoteSync = getSyncNoteList(notebookName, notebookPath);

                var dic = new Dictionary<NoteBatchOperateEnum, List<NoteEntity>>
                {
                    {NoteBatchOperateEnum.Add, new List<NoteEntity>()},
                    {NoteBatchOperateEnum.Modify, new List<NoteEntity>()},
                    {NoteBatchOperateEnum.Delete, new List<NoteEntity>()}
                };

                var syncCount = 1;
                var lstTasks = new List<Task<NoteEntity>>();
                foreach (var noteSync in lstNoteSync)
                {
                    var tmpNoteSync = noteSync;
                    lstTasks.Add(syncNoteAsync(tmpNoteSync, dic));
                }

                foreach (var task in lstTasks)
                {
                    if (CancelSync)
                    {
                        NoteDao.Inst.BatchOperate(dic);

                        if (null != syncCompleted)
                        {
                            syncCompleted(SyncCompletedType.All, "取消同步");
                        }
                        return;
                    }

                    var syncedNote = await task;

                    Debug.WriteLine("syncedNote - ThreadId:" + Thread.CurrentThread.ManagedThreadId);
                    if (null != SyncNoteChanged)
                    {
                        SyncNoteChanged(new NoteSyncEventArgs(lstNoteSync.Count, syncCount, syncedNote));
                    }
                    syncCount++;
                }

                // 同步该笔记本下新增加的笔记
                var newAddedNotes = await SyncNewAddedNotes(syncCompleted, notebookName, dic);

                // 将新增加的笔记添加到同步列表中
                if (null != newAddedNotes)
                {
                    AppendNewAddedNotesToNoteSyncList(newAddedNotes, lstNoteSync);
                }
                // 将本地有远程没有并且状态不是新增的笔记删掉
                deleteLocalNotesNotInServer(lstNoteSync, notebookPath, dic);

                NoteDao.Inst.BatchOperate(dic);

                if (null != syncCompleted)
                {
                    syncCompleted(SyncCompletedType.All, "同步完成");
                }
            }
            catch (YoudaoNoteException e)
            {
                if (null != syncCompleted)
                {
                    syncCompleted(SyncCompletedType.Failed, e);
                }
            }
            catch (Exception e1)
            {
                if (null != syncCompleted)
                {
                    syncCompleted(SyncCompletedType.Failed, e1);
                }
            }
        }

        private List<NoteSync> getSyncNoteList(string notebookName, string notebookPath)
        {
            var notePaths = _api.GetNoteList(notebookPath);
            var lstNoteSync = notePaths.Select(notePath => new NoteSync(notePath, notebookName, notebookPath)).ToList();
            return lstNoteSync;
        }

        private static void AppendNewAddedNotesToNoteSyncList(IEnumerable<NoteEntity> newAddedNotes, ICollection<NoteSync> lstNoteSync)
        {
            foreach (var newAddedNote in newAddedNotes)
            {
                lstNoteSync.Add(new NoteSync(newAddedNote.NotePath, newAddedNote.NotebookName, newAddedNote.NotebookPath));
            }
        }

        // 将本地有远程没有并且状态不是新增的笔记删掉
        // 将服务端已删除的笔记删掉
        private static void deleteLocalNotesNotInServer(List<NoteSync> lstNoteSync, 
            string notebookPath = null, Dictionary<NoteBatchOperateEnum, List<NoteEntity>> dic = null)
        {
            var localNotes = (null == notebookPath) ? NoteDao.Inst.GetAllNote() : NoteDao.Inst.GetNotesByNotebookPath(notebookPath);
            if (null == localNotes) return;

            var ids = new List<string>();
            foreach (var noteEntity in localNotes)
            {
                if (!lstNoteSync.Exists(noteSync => noteSync.NotePath == noteEntity.NotePath) && noteEntity.NoteStatus != NoteStatus.Added)
                {
                    //NoteDao.Inst.DeleteIfExist(noteEntity.Id);
                    if (null == dic)
                    {
                        ids.Add(noteEntity.Id);
                    }
                    else
                    {
                        dic[NoteBatchOperateEnum.Delete].Add(noteEntity);
                    }
                    
                }
            }
            if (null == dic)
            {
                NoteDao.Inst.DeleteAllIfExist(ids);
            }
        }

        private void performSyncNotebook(Action<SyncCompletedType, object> syncCompleted)
        {
            var notebooks = _api.GetAllNotebooks();

            if (null != notebooks)
            {
                NotebookDao.Inst.ModifyOrAddIfNotExistsAll(notebooks);
            }

            if (null != syncCompleted)
            {
                syncCompleted(SyncCompletedType.Notebook, notebooks);
            }
        }

        private async Task<List<NoteEntity>> SyncNewAddedNotes(Action<SyncCompletedType, string> syncCompleted, 
            string notebookName = null, Dictionary<NoteBatchOperateEnum, List<NoteEntity>> dic = null)
        {
            var newAddedNotes = notebookName == null ? NoteDao.Inst.GetAllLocalCreatedNotes() : NoteDao.Inst.GetLocalCreatedNotes(notebookName);
            var addedNoteCount = 1;
            var lstTask = new List<Task>();
            foreach (var addedNote in newAddedNotes)
            {
                var tmpAddedNote = addedNote;
                var syncTask = Task.Run(() =>
                {
                    var content = ConvertImageLocalPathToRemoteUrl(tmpAddedNote.Content);

                    var path = _api.CreateNote(content, tmpAddedNote.Title, tmpAddedNote.Author,
                        tmpAddedNote.NotebookPath, tmpAddedNote.Source);
                    var note = _api.GetNote(path);
                    tmpAddedNote.CreateTime = note.CreateTime;
                    tmpAddedNote.ModifyTime = note.ModifyTime;
                    tmpAddedNote.NotePath = path;
                    tmpAddedNote.Size = note.Size;
                    tmpAddedNote.NoteStatus = NoteStatus.Normal;
                    if (null == dic)
                    {
                        NoteDao.Inst.ModifyIfExist(tmpAddedNote);
                    }
                    else
                    {
                        dic[NoteBatchOperateEnum.Modify].Add(tmpAddedNote);
                    }
                });
                lstTask.Add(syncTask);
            }

            foreach (var task in lstTask)
            {
                await task;
                if (null != syncCompleted)
                {
                    syncCompleted(SyncCompletedType.AddedNote,
                        "同步新增加的笔记：" + (addedNoteCount++) + "/" + newAddedNotes.Count);
                }
            }

            return newAddedNotes;
        }

        private async Task<NoteEntity> syncNoteAsync(NoteSync noteSync, Dictionary<NoteBatchOperateEnum, List<NoteEntity>> dic =null)
        {
            return await Task.Run(() => syncNote(noteSync, dic));
        }

        private NoteEntity syncNote(NoteSync noteSync, Dictionary<NoteBatchOperateEnum, List<NoteEntity>> dic = null)
        {
            var serverNote = _api.GetNote(noteSync.NotePath);
            var findLocalNote = NoteListCache.GetNoteByPath(noteSync.NotebookPath, noteSync.NotePath);
            if (null != findLocalNote) // 本地有这篇笔记
            {
                if (serverNote.ModifyTime > findLocalNote.ModifyTime) // 服务端的笔记比本地上次同步过的新
                {
                    switch (findLocalNote.NoteStatus)
                    {
                        case NoteStatus.Normal:
                            return modifyNote(noteSync, serverNote, findLocalNote, dic);
                        case NoteStatus.Deleted:
                            deleteNote(findLocalNote, dic);
                            break;
                        case NoteStatus.Modified:
                            // 把本地的笔记备份一份新的，并标记为新增加的
                            var newNoteEntity = new NoteEntity
                            {
                                Title = "冲突 - " + findLocalNote.Title,
                                Content = findLocalNote.Content,
                                Author = findLocalNote.Author,
                                Source = findLocalNote.Source,
                                Size = findLocalNote.Size,
                                NotebookName = findLocalNote.NotebookName,
                                NotebookPath = findLocalNote.NotebookPath,
                                CreateTime = findLocalNote.CreateTime,
                                ModifyTime = findLocalNote.ModifyTime,
                                NoteStatus = NoteStatus.Added
                            };
                            if (null == dic) // 操作单篇笔记
                            {
                                NoteDao.Inst.AddIfNotExist(newNoteEntity);
                            }
                            else
                            {
                                if (null != newNoteEntity)
                                {
                                    dic[NoteBatchOperateEnum.Add].Add(newNoteEntity);
                                }
                            }

                            // 将服务端的更新到本地
                            return modifyNote(noteSync, serverNote, findLocalNote, dic);
                    }
                }
                else // 本地上次同步过的笔记比服务端的新
                {
                    switch (findLocalNote.NoteStatus)
                    {
                        case NoteStatus.Modified:
                            var content = ConvertImageLocalPathToRemoteUrl(findLocalNote.Content);
                            _api.UpdateNote(findLocalNote.NotePath, content, findLocalNote.Source, findLocalNote.Author, findLocalNote.Title);
                            // 为了获取到笔记在服务器端的更新时间，不得不重新获取笔记。不能用本地的时间是因为不同的客户端的时间可能不一样
                            var newNote = _api.GetNote(findLocalNote.NotePath);
                            findLocalNote.NoteStatus = NoteStatus.Normal;
                            findLocalNote.ModifyTime = newNote.ModifyTime;
                            if (null == dic) // 操作单篇笔记
                            {
                                NoteDao.Inst.ModifyIfExist(findLocalNote);
                            }
                            else
                            {
                                if (null != findLocalNote)
                                {
                                    dic[NoteBatchOperateEnum.Modify].Add(findLocalNote);
                                }
                            }
                            return findLocalNote;
                        case NoteStatus.Deleted:
                            deleteNote(findLocalNote, dic);
                            break;
                    }
                }
            }
            else // 本地没有这篇笔记，直接同步到本地
            {
                var entity = new NoteEntity(serverNote, noteSync.NotePath, noteSync.NotebookName, noteSync.NotebookPath);
                if (null == dic) // 操作单篇笔记
                {
                    NoteDao.Inst.AddIfNotExist(entity);
                }
                else
                {
                    if (null != entity)
                    {
                        dic[NoteBatchOperateEnum.Add].Add(entity);
                    }
                }
                return entity;
            }
            return null;
        }

        private NoteEntity modifyNote(NoteSync noteSync, Note serverNote, NoteEntity findLocalNote, Dictionary<NoteBatchOperateEnum, List<NoteEntity>> dic = null)
        {
            findLocalNote.NotePath = noteSync.NotePath;
            findLocalNote.Title = serverNote.Title;
            findLocalNote.CreateTime = serverNote.CreateTime;
            findLocalNote.ModifyTime = serverNote.ModifyTime;
            findLocalNote.Size = serverNote.Size;
            findLocalNote.Source = serverNote.Source;
            findLocalNote.NotebookPath = noteSync.NotebookPath;
            findLocalNote.Content = DownloadImageToLocalAsync(noteSync.NotePath, serverNote.Content).Result;
            findLocalNote.Author = serverNote.Author;
            findLocalNote.NotebookName = noteSync.NotebookName;
            findLocalNote.NoteStatus = NoteStatus.Normal;
            if (null == dic)
            {
                NoteDao.Inst.ModifyIfExist(findLocalNote);
            }
            else
            {
                if (null != findLocalNote)
                {
                    dic[NoteBatchOperateEnum.Modify].Add(findLocalNote);
                }
            }

            return findLocalNote;
        }

        private void deleteNote(NoteEntity findLocalNote, Dictionary<NoteBatchOperateEnum, List<NoteEntity>> dic = null)
        {
            Debug.WriteLine("删除服务端笔记：{0}", findLocalNote.Title);
            _api.DeleteNote(findLocalNote.NotePath);
            Debug.WriteLine("删除本地笔记：{0}", findLocalNote.Title);

            if (null == dic)
            {
                NoteDao.Inst.DeleteIfExist(findLocalNote.Id);
            }
            else
            {
                if (null != findLocalNote)
                {
                    dic[NoteBatchOperateEnum.Delete].Add(findLocalNote);
                }
            }
        }

        private List<NoteSync> getNoteSyncList(List<Notebook> notebooks)
        {
            if (null == notebooks)
            {
                throw new ArgumentNullException("notebooks");
            }
            return (from notebook in notebooks let notePaths = _api.GetNoteList(notebook.Path) from notePath in notePaths select new NoteSync(notePath, notebook.Name, notebook.Path)).ToList();
        }

        /// <summary>
        /// Download and save image to local.
        /// </summary>
        /// <param name="belongedNotePath">The path of belonged note.</param>
        /// <param name="imgNode">The image HtmlNode object.</param>
        /// <param name="imgRemoteUrl">The remote url of the image.</param>
        /// <param name="imgNameWithoutExt">The image name without extension.</param>
        /// <returns></returns>
        private string downloadAndSaveImageToLocal(string belongedNotePath, HtmlNode imgNode, string imgRemoteUrl, string imgNameWithoutExt)
        {
            Debug.WriteLine("Thread Id: " + Thread.CurrentThread.ManagedThreadId + " downloadAndSaveImageToLocal: " + imgRemoteUrl);
            var res = _api.DownloadAttach(imgRemoteUrl);

            var imgType = ImageUtil.GetImageType(res.Buffer);
            var localPath = "Images\\" + imgNameWithoutExt + "." + imgType;

            double width;
            if (imgNode.Attributes.Contains("width"))
            {
                if (Double.TryParse(imgNode.Attributes["width"].Value, out width))
                {
                    if (width > _screenWidth)
                    {
                        width = _screenWidth - 25;
                    }
                }
                imgNode.Attributes["width"].Value = width.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                var actualSize = ImageUtil.GetImageSize(imgType, res.Buffer);
                width = actualSize.Width > _screenWidth ? _screenWidth - 25 : actualSize.Width;
                imgNode.Attributes.Add("width", width.ToString(CultureInfo.InvariantCulture));
            }
            // Save to disk.
            var savePath = IsoStoreUtil.SaveToIsoStore(localPath, res.Buffer);
            // Save to db.
            var imageEntity = new ImageEntity(imgNameWithoutExt, imgType.ToString(), savePath, imgRemoteUrl, belongedNotePath);
            ImageDao.InsertIfNotExist(imageEntity);

            return savePath;
        } 
        #endregion
    }
}