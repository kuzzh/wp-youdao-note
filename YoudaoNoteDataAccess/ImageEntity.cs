

namespace YoudaoNoteDataAccess
{
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    [Table]
    public class ImageEntity
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 必须要构造函数，否则会出错。
        /// </summary>
        public ImageEntity()
        {
            
        }
        public ImageEntity(string imgNameWithoutExt, string imgExt, string imgLocalSavePath, string imgRemoteUrl, string belongedNotePath)
        {
            ImgNameWithoutExt = imgNameWithoutExt;
            ImgExt = imgExt;
            ImgLocalSavePath = imgLocalSavePath;
            ImgRemoteUrl = imgRemoteUrl;
            BelongedNotePath = belongedNotePath;
        }

        private string _id;
        [Column(DbType = "INT IDENTITY NOT NULL", CanBeNull = false, IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
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

        private string _imgNameWithoutExt;
        [Column]
        public string ImgNameWithoutExt
        {
            get
            {
                return _imgNameWithoutExt;
            }
            set
            {
                if (_imgNameWithoutExt == value) return;
                OnPropertyChanging("ImgNameWithoutExt");
                _imgNameWithoutExt = value;
                OnPropertyChanged("ImgNameWithoutExt");
            }
        }

        private string _imgExt;
        [Column]
        public string ImgExt
        {
            get
            {
                return _imgExt;
            }
            set
            {
                if (_imgExt == value) return;
                OnPropertyChanging("ImgExt");
                _imgExt = value;
                OnPropertyChanged("ImgExt");
            }
        }

        private string _belongedNotePath;
        [Column]
        public string BelongedNotePath
        {
            get
            {
                return _belongedNotePath;
            }
            set
            {
                if (_belongedNotePath == value) return;
                OnPropertyChanging("BelongedNotePath");
                _belongedNotePath = value;
                OnPropertyChanged("BelongedNotePath");
            }
        }

        private string _imgLocalSavePath;
        [Column]
        public string ImgLocalSavePath
        {
            get
            {
                return _imgLocalSavePath;
            }
            set
            {
                if (_imgLocalSavePath == value) return;
                OnPropertyChanging("ImgLocalSavePath");
                _imgLocalSavePath = value;
                OnPropertyChanged("ImgLocalSavePath");
            }
        }

        private string _imgRemoteUrl;
        [Column]
        public string ImgRemoteUrl
        {
            get
            {
                return _imgRemoteUrl;
            }
            set
            {
                if (_imgRemoteUrl == value) return;
                OnPropertyChanging("ImgRemoteUrl");
                _imgRemoteUrl = value;
                OnPropertyChanged("ImgRemoteUrl");
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
    }
}
