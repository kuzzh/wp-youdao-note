
namespace WPYoudaoNoteApp.Views
{
    using System.Windows;
    using System.Windows.Navigation;

    public partial class DiscussionView
    {
        public DiscussionView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            progressBar.Tip = "Loading";
            progressBar.Visibility = Visibility.Visible;
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void webBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
        }
    }
}