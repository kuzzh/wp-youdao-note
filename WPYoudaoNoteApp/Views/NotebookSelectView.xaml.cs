using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using YoudaoNoteDataAccess;
using System.Diagnostics;
using YoudaoNoteLog;
using WPYoudaoNoteApp.Utils;

namespace WPYoudaoNoteApp.Views
{
    public partial class NotebookSelectView : PhoneApplicationPage
    {
        private bool _isNewPage;
        private string _notebookPath;
        public static NotebookEntity Notebook;
        public NotebookSelectView()
        {
            InitializeComponent();

            NotebookList.NeedSaveSelectedNotebook = false;
            NotebookList.NotebookTapped += NotebookList_NotebookTapped;

            _isNewPage = true;
        }

        void NotebookList_NotebookTapped()
        {
            _notebookPath = NotebookList.SelectedNotebook.Path;
            Notebook = NotebookList.SelectedNotebook;

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SaveState();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
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
            else
            {
                if (NavigationContext.QueryString.ContainsKey("notebookPath"))
                {
                    _notebookPath = NavigationContext.QueryString["notebookPath"];
                }
                else
                {
                    Debug.WriteLine("NotebookSelectView::OnNavigatedTo 参数不正确，未能找到参数：notebookPath");
                    LoggerFactory.GetLogger().Error("NotebookSelectView::OnNavigatedTo 参数不正确，未能找到参数：notebookPath");
                }
            }

            _isNewPage = false;
        }

        private void SaveState()
        {
            State["Restore"] = "";
            State["_notebookPath"] = _notebookPath;
            State["Notebook"] = Notebook;
        }

        private void RestoreState()
        {
            if (State.ContainsKey("_notebookPath"))
            {
                _notebookPath = (string)State["_notebookPath"];
            }
            if (State.ContainsKey("Notebook"))
            {
                Notebook = (NotebookEntity)State["Notebook"];
            }
        }


        private void NotebookList_Loaded(object sender, RoutedEventArgs e)
        {
            Notebook = NotebookList.FindNotebookEntity(_notebookPath);
            NotebookList.SetSelectedNotebook(Notebook);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(_notebookPath))
            {
                Toast.Prompt("额，你还没选择笔记本呢！");
                e.Cancel = true;
            }
        }
    }
}