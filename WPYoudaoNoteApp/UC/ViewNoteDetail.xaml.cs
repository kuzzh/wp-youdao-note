

namespace WPYoudaoNoteApp.UC
{
    using Microsoft.Phone.Tasks;
    using System;
    using WPYoudaoNoteApp.Utils;
    using YoudaoNoteLog;

    public partial class ViewNoteDetail
    {
        public ViewNoteDetail()
        {
            InitializeComponent();
        }

        public string Source
        {
            private get
            {
                return tbSource.Text;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbSource.Text = value;
                }
            }
        }

        public string Author
        {
/*
            get
            {
                return tbAuthor.Text;
            }
*/
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbAuthor.Text = value;
                }
            }
        }

        public DateTime CreatedTime
        {
/*
            get
            {
                return new DateTime(Int64.Parse(tbCreatedTime.Text));
            }
*/
            set
            {
                tbCreatedTime.Text = value.ToString("G");
            }
        }

        public DateTime ModifiedTime
        {
/*
            get
            {
                return new DateTime(Int64.Parse(tbModifiedTime.Text));
            }
*/
            set
            {
                tbModifiedTime.Text = value.ToString("G");
            }
        }

        public string Notebook
        {
/*
            get
            {
                return tbNotebook.Text;
            }
*/
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbNotebook.Text = value;
                }
            }
        }

        public int WordCount
        {
/*
            get
            {
                int wordCount;
                return Int32.TryParse(tbWordCount.Text, out wordCount) ? wordCount : 0;
            }
*/
            set
            {
                tbWordCount.Text = value.ToString();
            }
        }

        private void BrowseButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Source))
                {
                    var webBrowserTask = new WebBrowserTask { Uri = new Uri(Source) };
                    webBrowserTask.Show();
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.GetLogger().Error("打开笔记来源网址发生错误", ex);
                Toast.Prompt("额，发生不可预知的错误，请稍后重试！");
            }
        }
    }
}
