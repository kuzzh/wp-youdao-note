

namespace YoudaoNoteDataAccess
{
    using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Text;
using System.Windows.Media;
using YoudaoNoteLib;
using YoudaoNoteUtils;

    /// <summary>
    /// 实例实体类，对应于数据库中的表。
    /// </summary>
    [Table]
    public class NotebookEntity : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public static Color DefaultForeColor = Colors.Black;
        public static readonly Color SelectedNotebookForeColor = Color.FromArgb(255, 147, 196, 210);
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public NotebookEntity()
        {
            Id = Guid.NewGuid().ToString("N");
            ForeColor = DefaultForeColor;
        }
        public NotebookEntity(Notebook notebook)
            : this()
        {
            Name = notebook.Name;
            Path = notebook.Path;
            NotesNum = notebook.NotesNum;
            CreateTime = notebook.CreateTime;
            ModifyTime = notebook.ModifyTime;
        }

        //public NotebookEntity DeepClone()
        //{
        //    var entity = new NotebookEntity
        //    {
        //        Id = Id,
        //        Name = Name,
        //        Path = Path,
        //        NotesNum = NotesNum,
        //        CreateTime = CreateTime,
        //        ModifyTime = ModifyTime
        //    };

        //    return entity;
        //}

        private string _id;
        //[Column(DbType = "INT IDENTITY NOT NULL", CanBeNull = false, IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        [Column(CanBeNull = false, IsPrimaryKey = true)]
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

        private Color _foreColor;
        public Color ForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                if (_foreColor == value) return;
                OnPropertyChanging("ForeColor");
                _foreColor = value;
                OnPropertyChanged("ForeColor");
            }
        }

        private string _name;
         [Column]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) return;
                OnPropertyChanging("Name");
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _path;
         [Column]
        public string Path
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

        private int _notesNum;
         [Column]
        public int NotesNum
        {
            get
            {
                return _notesNum;
            }
            set
            {
                if (_notesNum == value) return;
                OnPropertyChanging("NotesNum");
                _notesNum = value;
                OnPropertyChanged("NotesNum");
            }
        }

        public string FormatNotesNum
         {
             get
             {
                 return string.Format("({0})", NotesNum);
             }
         }

        private long _createTime;
         [Column]
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

        private long _modifyTime;
         [Column]
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

        private void OnPropertyChanging(string propertyName)
        {
            if (null != PropertyChanging)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("笔记本名称：" + Name);
            sb.AppendLine("笔记本路径：" + Path);
            sb.AppendLine("笔记本中笔记的数目：" + NotesNum);
            sb.AppendLine("笔记本创建时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(CreateTime));
            sb.AppendLine("笔记本最后修改时间：" + DateUtils.ConvertFromSecondsToLocalDatetime(ModifyTime));

            return sb.ToString();
        }
    }
}
