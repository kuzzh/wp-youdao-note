

namespace WPYoudaoNoteApp.Views
{
    using Coding4Fun.Toolkit.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using WPYoudaoNoteApp.UC;
    using WPYoudaoNoteApp.Utils;
    using YoudaoNoteDataAccess;
    using YoudaoNoteLog;
    using YoudaoNoteSync;
    using YoudaoNoteUtils;

    public partial class EditNoteView
    {
        private bool _isNewPage;
        private PhotoChooserTask _photoChooserTask;
        private ApplicationBarIconButton _appbarBtnSave;
        private ApplicationBarIconButton _appbarBtnCancel;

        public static NoteEntity NoteEntity;


        public EditNoteView()
        {
            InitializeComponent();

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
            State["Restore"] = "";
            State["NoteEntity"] = NoteEntity;
            State["_appbarBtnSaveEnabled"] = _appbarBtnSave.IsEnabled;
            State["_appbarBtnCancelEnabled"] = _appbarBtnCancel.IsEnabled;
            State["tbTitleText"] = tbTitle.Text;
            State["rtbContentText"] = rtbContent.GetHtml();
            State["tbNotebookText"] = tbNotebook.Text;
            State["_photoChooserTask"] = _photoChooserTask;
        }

        private void RestoreState()
        {
            GetAppbarBtns();
            if (State.ContainsKey("NoteEntity"))
            {
                NoteEntity = State["NoteEntity"] as NoteEntity;
            }
            if (State.ContainsKey("_appbarBtnSaveEnabled"))
            {
                _appbarBtnSave.IsEnabled = (bool)State["_appbarBtnSaveEnabled"];
            }
            if (State.ContainsKey("_appbarBtnCancelEnabled"))
            {
                _appbarBtnCancel.IsEnabled = (bool)State["_appbarBtnCancelEnabled"];
            }
            if (State.ContainsKey("tbTitleText"))
            {
                tbTitle.Text = (string)State["tbTitleText"];
            }
            if (State.ContainsKey("rtbContentText"))
            {
                rtbContent.SetHtml((string)State["rtbContentText"]);
            }
            if (State.ContainsKey("tbNotebookText"))
            {
                tbNotebook.Text = (string)State["tbNotebookText"];
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
                return;
            }

            RestoreState();

            _isNewPage = false;
        }

        private void GetAppbarBtns()
        {
            var applicationBar = ApplicationBar as ApplicationBar;
            if (applicationBar != null)
            {
                _appbarBtnSave = applicationBar.Buttons[0] as ApplicationBarIconButton;
                _appbarBtnCancel = applicationBar.Buttons[1] as ApplicationBarIconButton;
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
                var imageName = Path.Combine("Images", getImageName(e.OriginalFileName));

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

                NoteEntity.Content = rtbContent.InsertImage(imagePath);
            }
            rtbContent.SetFocus();
        }

        private string getImageName(string originalFileName)
        {
            return Path.GetFileName(originalFileName);
        }

        private void appbarBtnSave_Click(object sender, EventArgs e)
        {
            Debug.Assert(null != NoteEntity);

            using (new WaitPopup("正在保存笔记", this))
            {
                try
                {
                    NoteEntity.Title = tbTitle.Text;
                    NoteEntity.ModifyTime = DateUtils.ConvertFromLocalDateTimeToSeconds(DateTime.Now);
                    NoteEntity.Content = rtbContent.GetHtml();
                    if (NoteEntity.NoteStatus != NoteStatus.Added)
                    {
                        NoteEntity.NoteStatus = NoteStatus.Modified;
                    }

                    NoteDao.Inst.ModifyIfExist(NoteEntity);

                    Toast.Prompt("保存笔记成功"); 

                    gotoViewNotePage();
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().Error("保存笔记失败", ex);
                    Toast.Prompt("额，保存笔记失败，请稍后重试！");
                }
            }
        }

        private void gotoViewNotePage()
        {
            ViewNoteView.NoteEntity = NoteEntity;
            NavigationService.Navigate(new Uri("/Views/ViewNoteView.xaml?from=edit", UriKind.Relative));
        }

        private void appbarBtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("笔记尚未保存，确定放弃更改？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }

                gotoViewNotePage();
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLogger().Error("取消修改笔记失败", ex);
                Toast.Prompt("额，发生不可预知的错误，请稍后重试！");
            }
        }

        private void onBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("笔记尚未保存，确定放弃更改？", "提示", MessageBoxButton.OKCancel) != MessageBoxResult.Cancel)
            {
                return;
            }
            e.Cancel = true;
            rtbContent.SetFocus();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            

            tbTitle.Text = NoteEntity.Title;
            tbNotebook.Text = NoteEntity.NotebookName;

            using (var waitPopup = new WaitPopup("正在加载笔记", this))
            {
                try
                {
                    GetAppbarBtns();
                    rtbContent.SetHtml(NoteEntity.Content);
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().Error("", ex);
                    Toast.Prompt("额，发生不可预知的错误，请稍后重试！");
                }
            }

            rtbContent.SetFocus();
        }
    }
}