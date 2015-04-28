
namespace WPYoudaoNoteApp.Views
{
    using Microsoft.Phone.Tasks;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Navigation;
    using Utils;
    using YoudaoNoteDataAccess;
    using YoudaoNoteDatabaseCache;
    using YoudaoNoteLog;
    using YoudaoNoteUtils;

    public partial class NewNoteView
    {
        private const string EmptyNoteTitle = "无标题笔记";
        private bool _isNewPage;
        private PhotoChooserTask _photoChooserTask;

        private NoteEntity _noteEntity;
        public NewNoteView()
        {
            InitializeComponent();

            _noteEntity = new NoteEntity();

            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.ShowCamera = true;
            _photoChooserTask.Completed += photoChooserTask_Completed;

            _isNewPage = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SaveState();
        }

        private void SaveState()
        {
            if (null == _noteEntity) return;
            buildNoteEntity();
            State["Restore"] = "";
            State["_noteEntity"] = _noteEntity;
            State["_photoChooserTask"] = _photoChooserTask;
        }

        private void RestoreState()
        {
            if (State.ContainsKey("_noteEntity"))
            {
                _noteEntity = (NoteEntity)State["_noteEntity"];
            }
            if (State.ContainsKey("_photoChooserTask"))
            {
                _photoChooserTask = (PhotoChooserTask)State["_photoChooserTask"];
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_isNewPage == false)
            {
                if (null != NotebookSelectView.Notebook)
                {
                    tbNotebook.Text = NotebookSelectView.Notebook.Name;
                    _noteEntity.NotebookName = NotebookSelectView.Notebook.Name;
                    _noteEntity.NotebookPath = NotebookSelectView.Notebook.Path;
                }
                return;
            }
            if (State.ContainsKey("Restore"))
            {
                RestoreState();
            }
            else
            {
                if (NavigationContext.QueryString.ContainsKey("notebookName"))
                {
                    _noteEntity.NotebookName = NavigationContext.QueryString["notebookName"];
                }
                if (NavigationContext.QueryString.ContainsKey("notebookPath"))
                {
                    _noteEntity.NotebookPath = NavigationContext.QueryString["notebookPath"];
                }
            }

            tbNotebook.Text = _noteEntity.NotebookName;
            tbTitle.Text = EmptyNoteTitle;

            _isNewPage = false;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                rtbContent.SetHtml(_noteEntity.Content);

                rtbContent.Focus();
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLogger().Error("加载新笔记页面失败", ex);
                Toast.Prompt("额，加载新笔记页面失败，请稍后重试！");
            }
        }

        private void appbarBtnInsertPic_Click(object sender, EventArgs e)
        {
            if (null != _photoChooserTask)
            {
                _photoChooserTask.Show();
            }
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var imageName = Path.Combine("Images", Path.GetFileName(e.OriginalFileName));

                string imagePath = string.Empty;
                if (IsoStoreUtil.FileExists(imageName))
                {
                    imagePath = IsoStoreUtil.GetFileAbsolutePath(imageName);
                }
                else
                {
                    var tempBuffer = new byte[e.ChosenPhoto.Length];
                    e.ChosenPhoto.Read(tempBuffer, 0, tempBuffer.Length);

                    imagePath = IsoStoreUtil.SaveToIsoStore(imageName, tempBuffer);
                }

                _noteEntity.Content = rtbContent.InsertImage(imagePath);
            }
            rtbContent.SetFocus();
        }

        private void appbarBtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                buildNoteEntity();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        NoteDao.Inst.AddIfNotExist(_noteEntity);

                        ViewNoteView.NoteEntity = _noteEntity;

                        Dispatcher.BeginInvoke(() =>
                        {
                            var notebookEntity = NotebookListCache.Find(_noteEntity.NotebookPath);
                            if (null != notebookEntity)
                            {
                                Util.SaveLastSelectedNotebook(notebookEntity);
                                Debug.WriteLine("已保存选中的笔记本：" + notebookEntity.Name);
                            }
                            NavigationService.Navigate(new Uri("/Views/ViewNoteView.xaml?from=NewNotePage", UriKind.Relative));
                        });
                    }
                    catch (Exception ex1)
                    {
                        LoggerFactory.GetLogger().Error("保存笔记失败", ex1);
                        Toast.Prompt("额，保存笔记失败，请稍后重试！");
                    }
                });
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLogger().Error("保存笔记失败", ex);
                Toast.Prompt("额，保存笔记失败，请稍后再试！");
            }
        }

        private void appbarBtnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("笔记尚未保存，确定放弃更改？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                rtbContent.Focus();
                return;
            }

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void buildNoteEntity()
        {
            var title = tbTitle.Text.Trim();
            var text = rtbContent.GetText();

            if (string.IsNullOrEmpty(title) || title == EmptyNoteTitle)
            {
                if (text.Length <= 100)
                {
                    title = text;
                }else
                {
                    title = text.Substring(0, 100);
                }
            }
            _noteEntity.Title = title;
            _noteEntity.Content = rtbContent.GetHtml();
            //_noteEntity.NotebookName = NotebookSelectView.Notebook.Name;
            //_noteEntity.NotebookPath = NotebookSelectView.Notebook.Path;
            _noteEntity.CreateTime = DateUtils.ConvertFromLocalDateTimeToSeconds(DateTime.Now);
            _noteEntity.ModifyTime = DateUtils.ConvertFromLocalDateTimeToSeconds(DateTime.Now);
            _noteEntity.NoteStatus = NoteStatus.Added;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("笔记尚未保存，确定放弃更改？", "提示", MessageBoxButton.OKCancel) != MessageBoxResult.Cancel) return;
            e.Cancel = true;
            rtbContent.Focus();
        }

        private void tbNotebook_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/NotebookSelectView.xaml?notebookPath=" + HttpUtility.UrlEncode(_noteEntity.NotebookPath), UriKind.Relative));
        }
    }
}