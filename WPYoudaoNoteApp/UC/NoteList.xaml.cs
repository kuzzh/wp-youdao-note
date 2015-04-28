
using YoudaoNoteDatabaseCache;

namespace WPYoudaoNoteApp.UC
{
    using Microsoft.Phone.Controls;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using WPYoudaoNoteApp.Extensions;
    using WPYoudaoNoteApp.Utils;
    using WPYoudaoNoteApp.Views;
    using YoudaoNoteDataAccess;
    using YoudaoNoteLog;
    using YoudaoNoteSync;

    public partial class NoteList
    {
        private bool _isNewPage;
        private NotebookEntity _currentNotebookEntity;

        public event Action NoteSelected;

        public NoteList()
        {
            InitializeComponent();

            NoteListControl.ItemsSource = new NoteCollection();

            NoteDao.Inst.NoteChanged += NoteList_NoteChanged;

            _isNewPage = true;
        }

        void NoteList_NoteChanged()
        {
            UpdateBind(_currentNotebookEntity);
        }


        public void UpdateBind(NotebookEntity notebookEntity)
        {
            Debug.WriteLine("UpdateBind occured,tid:{0}", Thread.CurrentThread.ManagedThreadId);
            if (null == notebookEntity)
            {
                Debug.WriteLine("NoteList.xaml.cs::UpdateBind notebookEntity 为空");
                return;
            }
            
            _currentNotebookEntity = notebookEntity;
            bindNoteList(_currentNotebookEntity);
        }

        private async void deleteNoteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要删除该条笔记？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            var menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                LoggerFactory.GetLogger().Error("NoteList.xaml.cs::deleteNoteMenuItem_Click menuItem 为空");
                Toast.Prompt("额，删除笔记失败，请稍后重试！");
                return;
            }
            var noteId = menuItem.CommandParameter as string;
            if (string.IsNullOrEmpty(noteId))
            {
                LoggerFactory.GetLogger().Error("NoteList.xaml.cs::deleteNoteMenuItem_Click noteId 为空");
                Toast.Prompt("额，删除笔记失败，请稍后重试！");
                return;
            }

            Debug.WriteLine(("即将删除笔记：" + noteId));

            var noteEntity = NoteDao.Inst.GetNoteById(noteId);
            if (null == noteEntity)
            {
                LoggerFactory.GetLogger().Error("NoteList.xaml.cs::deleteNoteMenuItem_Click noteEntity 为空");
                Toast.Prompt("额，删除笔记失败，请稍后重试！");
                return;
            }
            using (new WaitPopup("正在删除笔记"))
            {
                if (noteEntity.NoteStatus == NoteStatus.Added)
                {
                    NoteDao.Inst.DeleteIfExist(noteId);
                    Toast.Prompt("笔记已被删除！");

                    Debug.WriteLine("已删除笔记：" + noteId);
                }
                else
                {
                    NoteDao.Inst.MarkAsDeletedIfExist(noteId);

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
                                        Toast.Prompt("笔记已被删除！");
                                        Debug.WriteLine("已删除笔记：" + noteId);
                                        break;
                                    case SyncCompletedType.Failed:
                                        LoggerFactory.GetLogger().Error("删除笔记失败", (Exception) msg);
                                        Toast.Prompt("额，笔记删除失败，请稍后重试！");
                                        break;
                                }
                            });
                        });
                    }
                }
            }
        }

        private void bindNoteList(NotebookEntity notebookEntity)
        {
            if (null == notebookEntity)
            {
                return;
            }

            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    getNoteCollectionAndPopulateListUsingTimer(notebookEntity);
                });
            }
            else
            {
                getNoteCollectionAndPopulateListUsingTimer(notebookEntity);
            }
        }

        private void getNoteCollectionAndPopulateListUsingTimer(NotebookEntity notebookEntity)
        {
            Debug.WriteLine("getNoteCollectionAndPopulateListUsingTimer tid:{0}", Thread.CurrentThread.ManagedThreadId);
            NoteListControl.ItemsSource.Clear();
            var notes = NoteListCache.GetNoteCollection(notebookEntity.Path);
            populateListUsingTimer(notes);
        }

        private void lbNote_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isNewPage == false)
            {
                return;
            }
            _isNewPage = false;
        }

        // 如若不做此步操作，在连续删除两篇笔记后，删除第三篇的时候就会出错，获取到的笔记 Id 是第一篇的。
        private void ContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            var conmen = (sender as ContextMenu);
            if (conmen != null) conmen.ClearValue(DataContextProperty);
        }

        private void NoteListControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var note = NoteListControl.SelectedItem as NoteEntity;
            if (null == note)
            {
                return;
            }

            ViewNoteView.NoteEntity = note;

            if (null != NoteSelected)
            {
                NoteSelected();
            }
        }

        private const int INCREMENTAL_POPULATION_DELAY_IN_MS = 100;
        private const int NbrItemsToInitiallyPopulateWith = 3;
        private bool _hasPopulatedFirstScreen;
        private NoteCollection _collectionForIncrementalLoad;

        private void populateListUsingTimer(NoteCollection noteCollection)
        {
            if (noteCollection == null) return;
            _collectionForIncrementalLoad = noteCollection;
            var timer = createTimer();
            timer.Tick += (sender, e) => handleTick(sender);
            timer.Start();
        }

        private void handleTick(object sender)
        {
            IEnumerable<NoteEntity> items;
            if (_hasPopulatedFirstScreen)
            {
                _hasPopulatedFirstScreen = false;

                ((DispatcherTimer)sender).Stop();

                items = _collectionForIncrementalLoad.Skip(NbrItemsToInitiallyPopulateWith);
            }
            else
            {
                _hasPopulatedFirstScreen = true;

                items = _collectionForIncrementalLoad.Take(NbrItemsToInitiallyPopulateWith);
            }
            NoteListControl.ItemsSource.AddRange(items);
        }

        private DispatcherTimer createTimer()
        {
            return new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(INCREMENTAL_POPULATION_DELAY_IN_MS)
            };
        }
    }
}
