

using YoudaoNoteDatabaseCache;

namespace WPYoudaoNoteApp.UC
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Media;
    using WPYoudaoNoteApp.Utils;
    using YoudaoNoteDataAccess;
    using YoudaoNoteLog;
    using YoudaoNoteSync;

    public partial class NotebookList
    {
        private NotebookCollection _notebookSource;

        public event Action NotebookTapped;
        public event Action NotebookSelectionChanged;

        public bool NeedSaveSelectedNotebook { get; set; }

        public NotebookEntity SelectedNotebook
        {
            get
            {
                return lbNotebook.SelectedItem as NotebookEntity;
            }
        }

        public NotebookList()
        {
            InitializeComponent();

            NeedSaveSelectedNotebook = true;

            _notebookSource = NotebookListCache.GetNotebookCollection();
            lbNotebook.ItemsSource = _notebookSource;
        }

        public async Task SycnNotebooksAsync()
        {
            await SyncCore.GetInst().SyncNotebooksAsync((type, obj) =>
                {
                    switch (type)
                    {
                        case SyncCompletedType.Notebook:
                            Toast.Prompt("笔记本同步成功");
                            break;
                        case SyncCompletedType.Failed:
                            LoggerFactory.GetLogger().Error("笔记本同步失败", (Exception)obj);
                            Toast.Prompt("额，笔记本同步失败，请稍后再试");
                            break;
                    }
                });
        }

        public void SetSelectedNotebook(NotebookEntity notebookEntity)
        {
            if (null == notebookEntity)
            {
                lbNotebook.SelectedItem = null;
                Debug.WriteLine("NotebookList.xaml.cs::SetSelectedNotebook notebookEntity 为空");
                return;
            }

            var findNotebook = _notebookSource.FirstOrDefault(entity => entity.Path.Equals(notebookEntity.Path));
            if (null == findNotebook)
            {
                LoggerFactory.GetLogger().Warn("NotebookList.xaml.cs::SetSelectedNotebook 未能找到笔记名称为：" + notebookEntity.Name +" 的笔记实例");
                return;
            }
            lbNotebook.SelectedItem = findNotebook;

            //lbNotebook_SelectionChanged(null, null);
        }

        public NotebookEntity GetDefaultSelectedNotebook()
        {
            var lastSelectedNotebook = Util.ReadLastSelectedNotebook();
            if (lastSelectedNotebook != null)
            {
                return lastSelectedNotebook;
            }
            if (_notebookSource != null && _notebookSource.Count > 0) 
                return _notebookSource[0];
            return null;
        }

        public NotebookEntity FindNotebookEntity(string notebookPath)
        {
            return _notebookSource.FirstOrDefault(entity => entity.Path == notebookPath);
        }

        private void lbNotebook_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (null == SelectedNotebook)
            {
                Debug.WriteLine("选中的笔记本为空");
                return;
            }

            if (NotebookTapped != null)
                NotebookTapped();
        }

        private void SetNotebookForeColor(NotebookEntity entity, Color foreColor)
        {
            if (null == entity)
            {
                return;
            }
            entity.ForeColor = foreColor;
        }

        private void lbNotebook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null == _notebookSource) return;
            if (null == SelectedNotebook) return;

            foreach (var item in _notebookSource)
            {
                if (null != item)
                {
                    if (item.Path != SelectedNotebook.Path)
                    {
                        SetNotebookForeColor(item, NotebookEntity.DefaultForeColor);
                    }
                    else
                    {
                        SetNotebookForeColor(item, NotebookEntity.SelectedNotebookForeColor);
                    }
                }
            }

            Debug.WriteLine("选中笔记本：" + SelectedNotebook.Name);

            if (NeedSaveSelectedNotebook)
            {
                Util.SaveLastSelectedNotebook(SelectedNotebook);
                Debug.WriteLine("已保存选中的笔记本：" + SelectedNotebook.Name);
            }

            if (NotebookSelectionChanged != null)
            {
                NotebookSelectionChanged();
            }
        }
    }
}
