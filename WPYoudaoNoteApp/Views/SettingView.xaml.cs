

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using WPYoudaoNoteApp.UC;
using WPYoudaoNoteApp.Utils;
using YoudaoNoteDataAccess;
using YoudaoNoteDatabaseCache;
using YoudaoNoteLog;
using YoudaoNoteSync;

namespace WPYoudaoNoteApp.Views
{
    public partial class SettingView
    {
        public SettingView()
        {
            InitializeComponent();
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private async void btnClearCache_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要清理缓存？", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            using (new WaitPopup("正在清理缓存", this))
            {
                await Task.Run(() =>
                {
                    Util.ClearImageCache();

                    var cachedNotePathList = ImageDao.GetCachedNotePathList();
                    if (null == cachedNotePathList) return;
                    foreach (var notePath in cachedNotePathList)
                    {
                        var noteEntity = NoteDao.Inst.GetNoteByNotePath(notePath);
                        if (null == noteEntity) continue;
                        noteEntity.Content = SyncCore.GetInst().ConvertImageLocalPathToRemoteUrl(noteEntity.Content);
                        NoteDao.Inst.ModifyIfExist(noteEntity);

                        ImageDao.DeleteImageByBelongedNotePath(noteEntity.NotePath);
                    }
                });
            }
            showCacheSize();
        }
        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要注销吗？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            using (new WaitPopup("正在注销中", this))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        clearNotebooks();

                        clearNotes();

                        clearPicCache();

                        clearLocalLog();

                        ConstantPool.AccessToken = "";
                        Util.SaveLastSelectedNotebook(null);

                        MyDataContext.DeleteDatabaseIfExists();
                    }
                    catch (Exception ex)
                    {
                        LoggerFactory.GetLogger().Error("注销发生错误", ex);
                        Toast.Prompt("额，注销发生错误，请稍后重试！");
                    }
                });
            }
            showCacheSize();
            navigateAuth();
        }

        private static void clearLocalLog()
        {
            LoggerFactory.GetLogger().SafeDeleteFile();
        }

        private void navigateAuth()
        {
            NavigationService.Navigate(new Uri("/Views/AuthorizeView.xaml", UriKind.Relative));
        }

        private static void clearPicCache()
        {
            Util.ClearImageCache();
        }

        private static void clearNotes()
        {
            //NoteListCache.Clear();
            NoteDao.Inst.Clear();
        }

        private static void clearNotebooks()
        {
            NotebookListCache.Clear();
            NotebookDao.Inst.Clear();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            showCacheSize();

            chkSendErrorReport.IsChecked = ConstantPool.AutoSendErrorReport;
        }

        private void showCacheSize()
        {
            // MB
            var cacheSize = (Util.GetCacheSize() / 1024.0 / 1024.0).ToString("0.000");
            btnClearCache.Content = "清除缓存（" + cacheSize + " M）";
        }

        private void chkSendErrorReport_Checked(object sender, RoutedEventArgs e)
        {
            ConstantPool.AutoSendErrorReport = true;
        }

        private void chkSendErrorReport_Unchecked(object sender, RoutedEventArgs e)
        {
            ConstantPool.AutoSendErrorReport = false;
        }
    }
}