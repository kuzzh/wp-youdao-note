
namespace WPYoudaoNoteApp.Views
{
    using Microsoft.Phone.Controls;
    using System.Windows.Navigation;
    using WPYoudaoNoteApp.Utils;
    using YoudaoNoteDataAccess;
    using YoudaoNoteLog;

    public partial class FullScreenReadingView : PhoneApplicationPage
    {
        public static NoteEntity NoteEntity;

        public FullScreenReadingView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (null == NoteEntity)
            {
                Toast.Prompt("额，貌似哪里不太对了！");
                return;
            }
            try
            {
                TitleTextBlcok.Text = NoteEntity.Title;

                var html = TidyHtml.TideHtml(NoteEntity.Content);

                ContentBrowser.NavigateToString(html);
            }
            catch (System.Exception ex)
            {
                LoggerFactory.GetLogger().Error("", ex);
                Toast.Prompt("额，貌似哪里不好了！");
            }
        }

        private void OnGoBack_Click(object sender, System.EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void ContentBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}