

using System.Windows;

namespace YoudaoNoteDataAccess
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;
    using System.Text;
    using YoudaoNoteLib;
    using YoudaoNoteUtils;

    [Table]
    public class NoteEntity : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private delegate void OnUiThreadDelegate();

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public NoteEntity()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public NoteEntity(Note note, string notePath, string notebookName, string notebookPath)
            : this()
        {
            if (null == note)
            {
                throw new ArgumentNullException("note");
            }
            if (null == notebookName)
            {
                throw new ArgumentNullException("notebookName");
            }
            
            NotePath = notePath;
            Title = note.Title;
            CreateTime = note.CreateTime;
            ModifyTime = note.ModifyTime;
            Size = note.Size;
            Source = note.Source;
            NotebookPath = notebookPath;
            Content = note.Content;
            Author = note.Author;
            NotebookName = notebookName;
            NoteStatus = NoteStatus.Normal;
        }

        public NoteEntity DeepClone()
        {
            var entity = new NoteEntity
            {
                Id = Id,
                NotePath = NotePath,
                Title = Title,
                CreateTime = CreateTime,
                ModifyTime = ModifyTime,
                Size = Size,
                Source = Source,
                NotebookName = NotebookName,
                NotebookPath = NotebookPath,
                Content = Content,
                Author = Author,
                NoteStatus = NoteStatus
            };

            return entity;
        }

        public void Copy(NoteEntity newNoteEntity)
        {
            if (null == newNoteEntity)
            {
                throw new ArgumentNullException("newNoteEntity");
            }
            Id = newNoteEntity.Id;
            NotePath = newNoteEntity.NotePath;
            Title = newNoteEntity.Title;
            CreateTime = newNoteEntity.CreateTime;
            ModifyTime = newNoteEntity.ModifyTime;
            Size = newNoteEntity.Size;
            Source = newNoteEntity.Source;
            NotebookName = newNoteEntity.NotebookName;
            NotebookPath = newNoteEntity.NotebookPath;
            Content = newNoteEntity.Content;
            Author = newNoteEntity.Author;
            NoteStatus = newNoteEntity.NoteStatus;
        }

        private string _id;
        //[Column(DbType = "INT IDENTITY NOT NULL", CanBeNull = false, IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        [Column(CanBeNull = false, IsPrimaryKey = true, UpdateCheck = UpdateCheck.Never)]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id == value) return;
                OnPropertyChanging("Id");
                _id = value;
                OnPropertyChanged("Id");
            }
        }
        private string _title;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value) return;
                OnPropertyChanging("Title");
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private string _notebookName;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public string NotebookName
        {
            get
            {
                return _notebookName;
            }
            set
            {
                if (_notebookName == value) return;
                OnPropertyChanging("NotebookName");
                _notebookName = value;
                OnPropertyChanged("NotebookName");
            }
        }

        private string _path;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public string NotePath
        {
            get
            {
                return _path;
            }
            set
            {
                if (_path == value) return;
                OnPropertyChanging("Path");
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        private string _notebookPath;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public string NotebookPath
        {
            get
            {
                return _notebookPath;
            }
            set
            {
                if (_notebookPath == value) return;
                OnPropertyChanging("NotebookPath");
                _notebookPath = value;
                OnPropertyChanged("NotebookPath");
            }
        }

        private string _author;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (_author == value) return;
                OnPropertyChanging("Author");
                _author = value;
                OnPropertyChanged("Author");
            }
        }

        private string _source;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (_source == value) return;
                OnPropertyChanging("Source");
                _source = value;
                OnPropertyChanged("Source");
            }
        }

        private long _size;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public long Size
        {
            get
            {
                return _size;
            }
            set
            {
                if (_size == value) return;
                OnPropertyChanging("Size");
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        private long _createTime;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public long CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                if (_createTime == value) return;
                OnPropertyChanging("CreateTime");
                _createTime = value;
                OnPropertyChanged("CreateTime");
            }
        }

        private string _content;
        [Column(DbType = "NTEXT", UpdateCheck = UpdateCheck.Never)]
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content == value) return;
                OnPropertyChanging("Content");
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        private long _modifyTime;
       [Column(UpdateCheck = UpdateCheck.Never)]
        public long ModifyTime
        {
            get
            {
                return _modifyTime;
            }
            set
            {
                if (_modifyTime == value) return;
                OnPropertyChanging("ModifyTime");
                _modifyTime = value;
                OnPropertyChanged("ModifyTime");
            }
        }

        public string FormatModifyTime
        {
            get
            {
                return DateUtils.ConvertFromSecondsToLocalDatetime(ModifyTime).ToString("yyyy/MM/dd HH:mm ddd");
            }
        }

        private NoteStatus _noteStatus;
        [Column(UpdateCheck = UpdateCheck.Never)]
        public NoteStatus NoteStatus
        {
            get
            {
                return _noteStatus;
            }
            set
            {
                if (_noteStatus == value) return;
                OnPropertyChanging("NoteStatus");
                _noteStatus = value;
                OnPropertyChanged("NoteStatus");
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            if (null != PropertyChanging)
            {
                 PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                OnUiThread(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }


        private void OnUiThread(OnUiThreadDelegate onUiThreadDelegate)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                onUiThreadDelegate();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(onUiThreadDelegate);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("笔记标题：" + Title);
            sb.AppendLine("笔记作者：" + Author);
            sb.AppendLine("笔记来源 URL：" + Source);
            sb.AppendLine("笔记大小：" + Size / 1024 + " KB");
            sb.AppendLine("笔记创建时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(CreateTime));
            sb.AppendLine("笔记最后修改时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(ModifyTime));
            sb.AppendLine("笔记状态：" + NoteStatus);
            sb.AppendLine("笔记内容前 100 个字符：" + Content.Substring(0, (Content.Length > 100) ? 100 : Content.Length));

            return sb.ToString();
        }
    }
}
