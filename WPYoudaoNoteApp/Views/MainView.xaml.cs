
namespace WPYoudaoNoteApp.Views
{
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using WPYoudaoNoteApp.Extensions;
    using WPYoudaoNoteApp.UC;
    using WPYoudaoNoteApp.Utils;
    using YoudaoNoteLog;
    using YoudaoNoteSync;


    public partial class MainView : PhoneApplicationPage
    {
        private bool _isNewPage;
        private ApplicationBar _mainApplicationBar;
        public MainView()
        {
            InitializeComponent();

            CreateMainApplicationBar();
            InitNoteList();

            _isNewPage = true;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ConstantPool.AccessToken))
            {
                NavigationService.Navigate(new Uri("/Views/AuthorizeView.xaml", UriKind.Relative));
                return;
            }

            if (_isNewPage == false)
            {
                // 从新建页面返回
                var savedNotebook = Util.ReadLastSelectedNotebook();
                if (null != savedNotebook && null != NotebookList.SelectedNotebook)
                {
                    if (savedNotebook.Path != NotebookList.SelectedNotebook.Path)
                    {
                        NotebookList.SetSelectedNotebook(savedNotebook);
                    }
                }
                return;
            }

            InitNotebookList();

            if (ConstantPool.AutoSendErrorReport)
            {
                Util.SendErrorReportAsync();
            }

            _isNewPage = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("from"))
            {
                if (NavigationContext.QueryString["from"] == "bootstrap")
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.RemoveBackEntry();
                    }
                }
                NavigationContext.QueryString.Clear();
            }
        }

        private void OnBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定要退出？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    Debug.WriteLine("取消退出");
                    e.Cancel = true;
                    return;
                }
                Application.Current.Terminate();
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLogger().Error("", ex);
                Toast.Prompt("额，发生不可预知的错误，请稍后重试！");
            }
        }

        private void OnAboutAppbarButtonClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AboutView.xaml", UriKind.Relative));
        }

        private void OnSettingsAppbarButtonClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SettingView.xaml", UriKind.Relative));
        }

        private void OnAddNewNoteAppbarButtonClicked(object sender, EventArgs e)
        {
            if (null == NotebookList.SelectedNotebook)
            {
                Toast.Prompt("额，你可能还没有同步笔记本列表哦！");
                return;
            }
            NavigationService.Navigate(new Uri("/Views/NewNoteView.xaml?notebookName=" + HttpUtility.UrlEncode(NotebookList.SelectedNotebook.Name)
                + "&notebookPath=" + HttpUtility.UrlEncode(NotebookList.SelectedNotebook.Path),
                UriKind.Relative));
        }

        private void OnDiscussionMenuItemClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/DiscussionView.xaml", UriKind.Relative));
        }

        private async void OnSyncAppbarButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var pivotItem = MainPivot.SelectedItem as PivotItem;
                if (null == pivotItem) return;
                if (pivotItem.Name == null) return;
                switch ((pivotItem.Name))
                {
                    case "NoteListPivotItem":
                        using (var waitPopup = new WaitPopup("正在同步笔记中", this))
                        {
                            if (NotebookList.SelectedNotebook != null)
                            {
                                SyncCore.GetInst().SyncNoteChanged += (noteSyncEventArgs) =>
                                    {
                                        OnSyncNoteChanged(waitPopup, noteSyncEventArgs);
                                    };
                                await SyncCore.GetInst().SyncNotebookNotesAsync(NotebookList.SelectedNotebook.Name,
                                    NotebookList.SelectedNotebook.Path, (type, msg) =>
                                    {
                                        switch (type)
                                        {
                                            case SyncCompletedType.All:
                                                Toast.Prompt("笔记同步完成");
                                                break;
                                            case SyncCompletedType.AddedNote:
                                                waitPopup.SafeInvoke(() => waitPopup.SetTip(msg.ToString()));
                                                break;
                                            case SyncCompletedType.Failed:
                                                Toast.Prompt("额，同步笔记失败，请稍后重试！");
                                                LoggerFactory.GetLogger().Error("同步笔记失败", (Exception)msg);
                                                break;
                                        }
                                    });
                                // 取消事件订阅
                                SyncCore.GetInst().SyncNoteChanged -= (noteSyncEventArgs) =>
                                {
                                    OnSyncNoteChanged(waitPopup, noteSyncEventArgs);
                                };
                            }
                            else
                            {
                                Toast.Prompt("额，没有选择任何笔记本欸！");
                            }
                        }
                        break;
                    case "NotebookPivotItem":
                        using (new WaitPopup("正在同步笔记本列表", this))
                        {
                            await NotebookList.SycnNotebooksAsync();
                            NotebookList.SetSelectedNotebook(NotebookList.GetDefaultSelectedNotebook());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLogger().Error("同步发生错误", ex);
                Toast.Prompt("额，发生不可预知的错误，请稍后重试！");
            }
        }

        private static void OnSyncNoteChanged(WaitPopup wp, NoteSyncEventArgs noteSyncEventArgs)
        {
            wp.SafeInvoke(() => wp.SetTip(noteSyncEventArgs.SyncedNoteCount + "/" + noteSyncEventArgs.TotalNoteCount));
        }
       
        private void CreateMainApplicationBar()
        {
            _mainApplicationBar = new ApplicationBar()
            {
                IsVisible = true,
                IsMenuEnabled = true,
                BackgroundColor = ConstantPool.AppForeColor,
                Opacity = 0.99
            };
            Util.AddApplicationBarButton(_mainApplicationBar, "/Assets/Icons/new.png", "新建笔记", OnAddNewNoteAppbarButtonClicked);
            Util.AddApplicationBarButton(_mainApplicationBar, "/Assets/Icons/settings.png", "设置", OnSettingsAppbarButtonClicked);
            Util.AddApplicationBarButton(_mainApplicationBar, "/Assets/Icons/about.png", "关于", OnAboutAppbarButtonClicked);
            Util.AddApplicationBarButton(_mainApplicationBar, "/Assets/Icons/sync.png", "同步", OnSyncAppbarButtonClicked);

            Util.AddApplicationBarMenuItem(_mainApplicationBar, "讨论区", OnDiscussionMenuItemClicked);

            ApplicationBar = _mainApplicationBar;
        }

        private void InitNoteList()
        {
            NoteList.NoteSelected += () =>
            {
                NavigationService.Navigate(new Uri("/Views/ViewNoteView.xaml", UriKind.Relative));
            };
        }

        private void InitNotebookList()
        {
            NotebookList.NotebookSelectionChanged += () =>
            {
                if (null == NotebookList.SelectedNotebook)
                {
                    return;
                }
                NoteList.UpdateBind(NotebookList.SelectedNotebook);
                NoteListPivotItem.Header = new TextBlock
                {
                    Text = NotebookList.SelectedNotebook.Name,
                    FontFamily = new System.Windows.Media.FontFamily("Microsoft YaHei"),
                    FontSize = 40,
                    Margin = new Thickness(0, 10, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                MainPivot.SelectedItem = NoteListPivotItem;
            };

            NotebookList.SetSelectedNotebook(NotebookList.GetDefaultSelectedNotebook());
        }
    }
}