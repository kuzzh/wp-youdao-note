

namespace WPYoudaoNoteApp.Views
{
    using Coding4Fun.Toolkit.Controls;
    using HtmlAgilityPack;
    using Microsoft.Phone.Controls;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using WPYoudaoNoteApp.UC;
    using WPYoudaoNoteApp.Utils;
    using YoudaoNoteDataAccess;
    using YoudaoNoteLog;
    using YoudaoNoteSync;
    using YoudaoNoteUtils;

    public partial class ViewNoteView : PhoneApplicationPage
    {
        private bool _isNewPage;

        private static NoteEntity _noteEntity;
        public static NoteEntity NoteEntity
        {
            get
            {
                return _noteEntity;
            }
            set
            {
                if (value == null)
                {
                    _noteEntity = value;
                    return;
                }
                var doc = new HtmlDocument();
                doc.LoadHtml(value.Content);

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

                var charsetTag = doc.DocumentNode.SelectSingleNode("//meta[@charset]");
                if (null == charsetTag)
                {
                    charsetTag = HtmlNode.CreateNode("<meta charset=\"utf-8\">");
                    headTag.AppendChild(charsetTag);
                }else
                {
                    charsetTag.SetAttributeValue("charset", "utf-8");
                }

                value.Content = doc.DocumentNode.InnerHtml;

                _noteEntity = value;
            }
        }
        public ViewNoteView()
        {
            InitializeComponent();

            _isNewPage = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SaveState();
        }

        private void SaveState()
        {
            State["Restore"] = "";
            State["NoteEntity"] = NoteEntity;
        }

        private void RestoreState()
        {
            if (State.ContainsKey("NoteEntity"))
            {
                NoteEntity = (NoteEntity)State["NoteEntity"];
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_isNewPage == false)
            {
                return;
            }

            if (State.ContainsKey("Restore"))
            {
                RestoreState();
            }

            await loadNote();

            if (NavigationContext.QueryString.ContainsKey("from"))
            {
                if (NavigationContext.QueryString["from"] == "edit")
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.RemoveBackEntry(); // remove EditNoteView.xaml
                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.RemoveBackEntry(); // remove ViewNoteView.xaml
                        }
                    }
                }
                if (NavigationContext.QueryString["from"] == "NewNotePage")
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.RemoveBackEntry();
                    }
                }
                NavigationContext.QueryString.Clear();
            }

            _isNewPage = false;
        }

        private async Task loadNote()
        {
            using (var waitPopup = new WaitPopup("正在加载笔记", this))
            {
                try
                {
                    tbTitle.Text = NoteEntity.Title;
                    tbNotebook.Text = NoteEntity.NotebookName;

                    if (NoteUtil.NeedCache(NoteEntity.Content))
                    {
                        if (Util.IsNetworkAvailable())
                        {
                            waitPopup.SetTip("正在下载图片");
                            NoteEntity.Content = await SyncCore.GetInst().DownloadImageToLocalAsync(NoteEntity.NotePath, NoteEntity.Content, (count, totalCount) =>
                            {
                                waitPopup.SetTip("正在下载图片：" + count + "/" + totalCount);
                            });
                            NoteDao.Inst.ModifyIfExist(NoteEntity);

                            ContentWebBrowser.NavigateToString(NoteEntity.Content);
                        }
                        else
                        {
                            Toast.Prompt("额，网络不可用，图片未能下载！");
                        }
                    }
                    else
                    {
                        ContentWebBrowser.NavigateToString(NoteEntity.Content);
                    }
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().Error("", ex);
                    Toast.Prompt("额，发生不可预知的错误，请稍后重试！");
                }
            }
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        private async void btnSync_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.IsNetworkAvailable())
            {
                Toast.Prompt("额，网络不可用，请检查网络配置！");
                return;
            }
            using (new WaitPopup("正在同步笔记", this))
            {
                try
                {
                    await SyncCore.GetInst().SyncNoteAsync(NoteEntity, (type, msg) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            switch (type)
                            {
                                case SyncCompletedType.Note:
                                    NoteEntity = NoteDao.Inst.GetNoteById(NoteEntity.Id);
                                    ContentWebBrowser.NavigateToString(NoteEntity.Content);
                                    tbTitle.Text = NoteEntity.Title;

                                    Toast.Prompt("笔记修改成功！");
                                    break;
                                case SyncCompletedType.Failed:
                                    LoggerFactory.GetLogger().Error("同步笔记失败", (Exception)msg);
                                    Toast.Prompt("额，同步笔记失败，请稍后重试！");
                                    break;
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().Error("同步笔记失败", ex);
                    Toast.Prompt("额，同步笔记失败，请稍后重试！");
                }
            }
        }

        private void ContentWebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            // prevent from navigating
            e.Cancel = true;
        }

        private void appBarBtnViewNoteInfo_Click(object sender, EventArgs e)
        {
            var ucNoteDetail = new ViewNoteDetail()
            {
                Source = NoteEntity.Source,
                Author = NoteEntity.Author,
                CreatedTime = DateUtils.ConvertFromSecondsToLocalDatetime(NoteEntity.CreateTime),
                ModifiedTime = DateUtils.ConvertFromSecondsToLocalDatetime(NoteEntity.ModifyTime),
                Notebook = NoteEntity.NotebookName,
                WordCount = NoteEntity.Content.Length
            };
            var messagePrompt = new MessagePrompt
            {
                Title = "笔记信息",
                Body = ucNoteDetail,
                Background = new SolidColorBrush(ConstantPool.AppForeColor)
            };
            messagePrompt.Show();
        }
        private void appBarBtnFullScreenRead_Click(object sender, EventArgs e)
        {
            if (null == NoteEntity)
            {
                Toast.Prompt("额，貌似什么地方出错了！");
                return;
            }
            FullScreenReadingView.NoteEntity = NoteEntity;

            NavigationService.Navigate(new Uri("/Views/FullScreenReadingView.xaml", UriKind.Relative));
        }

        private async void appbarBtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除该条笔记？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            await deleteNote();
        }

        private async Task deleteNote()
        {
            using (new WaitPopup("正在删除笔记", this))
            {
                try
                {
                    var noteEntity = NoteDao.Inst.GetNoteById(NoteEntity.Id);
                    if (noteEntity.NoteStatus == NoteStatus.Added)
                    {
                        NoteDao.Inst.DeleteIfExist(NoteEntity.Id);
                        Toast.Prompt("笔记已被删除！");
                    }
                    else
                    {
                        NoteDao.Inst.MarkAsDeletedIfExist(NoteEntity.Id);

                        // 若网络可用，直接将远程服务器上的笔记删除
                        if (Util.IsNetworkAvailable())
                        {
                            await SyncCore.GetInst().DeleteNoteAsync(noteEntity, (type, msg) =>
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    switch (type)
                                    {
                                        case SyncCompletedType.Note:
                                            Toast.Prompt("笔记已被删除");
                                            break;
                                        case SyncCompletedType.Failed:
                                            LoggerFactory.GetLogger().Error("删除笔记失败", (Exception)msg);
                                            Toast.Prompt("额，删除笔记失败，请稍后重试！");
                                            break;
                                    }
                                });
                            });
                        }
                    }

                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().Error("删除笔记失败", ex);
                    Toast.Prompt("额，删除笔记失败，请稍后重试！");
                }
            }
        }

        private void appbarBtnEdit_Click(object sender, EventArgs e)
        {
            EditNoteView.NoteEntity = NoteEntity;

            NavigationService.Navigate(new Uri("/Views/EditNoteView.xaml", UriKind.Relative));
        }
    }
}