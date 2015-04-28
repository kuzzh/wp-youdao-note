using Coding4Fun.Toolkit.Controls;
using System.Threading;
using System.Windows.Controls;
using WPYoudaoNoteApp.Utils;

namespace WPYoudaoNoteApp.UC
{
    public partial class BBS : UserControl
    {
        public BBS()
        {
            InitializeComponent();
        }

        public BBS(string content, string title = "重要通知")
            : this()
        {
            Title = title;
            Content = content;
        }

        public string Title
        {
            get
            {
                return tbTitle.Text;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbTitle.Text = value;
                }
            }
        }

        public new string Content
        {
            get
            {
                return tbContent.Text;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbContent.Text = value;
                }
            }
        }

        public static void Show(string content, string title = "重要通知")
        {
            var bbs = new BBS(title, content);

            var messagePrompt = new MessagePrompt
            {
                Title = "公告栏",
                Body = bbs,
                IsCancelVisible = false
            };
            messagePrompt.Completed += messagePrompt_Completed;

            messagePrompt.Show();
        }

        static void messagePrompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            ConstantPool.ShowBulletinBoard = false;
        }
    }
}
