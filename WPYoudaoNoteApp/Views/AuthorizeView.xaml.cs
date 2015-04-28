

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;
using WPYoudaoNoteApp.Utils;
using YoudaoNoteDataAccess;
using YoudaoNoteLog;
using YoudaoNoteSync;

namespace WPYoudaoNoteApp.Views
{
    public partial class AuthorizeView
    {
        private const string AuthorizeUrl = "https://note.youdao.com/oauth/authorize2?client_id=" + ConstantPool.ConsumerKey
                + "&response_type=code&redirect_uri="; // 此处填写重定向地址
        public AuthorizeView()
        {
            InitializeComponent();
        }
      
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Navigate(new Uri(AuthorizeUrl));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            removeAllBackEntry();
        }

        private void removeAllBackEntry()
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            // 此处填写 access token callback 地址
            if (e.Uri.ToString().Contains(""))
            {
                var splits = e.Uri.Query.Split('=');
                if (splits.Length <= 1) return;
                ConstantPool.AccessToken = splits[1];
                MyDataContext.CreatetDatabaseIfNeeded();
                SyncCore.GetInst().InitializeSyncCore(ConstantPool.AccessToken, ConstantPool.ScreenWidth);
                btnGoHome.Visibility = Visibility.Visible;
                logout();
            }
#if DEBUG
            else if (e.Uri.ToString().Contains("https://note.youdao.com/oauth/login_mobile.html"))
            {
                const string username = ""; // 填写测试用户名
                const string password = ""; // 填写测试用户密码
                webBrowser.InvokeScript("eval", "document.getElementById('user').value = '" + username + "';document.getElementById('pass').value = '" + password + "';");
            }
#endif
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainView.xaml?from=authorizePage", UriKind.Relative));
        }

        private void logout()
        {
            var logoutUri = new Uri("http://note.youdao.com/?auto=1");
            var wb = new WebBrowser {IsScriptEnabled = true};
            wb.LoadCompleted += (sender, e) =>
            {
                try
                {
                    wb.InvokeScript("eval", "document.getElementById('dropdown-list').style.display = 'block';var links = document.getElementsByTagName('a');for (var i = 0; i < links.length; i++) { if (links[i].innerText == '注销') { links[i].click(); break; } }");
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().Error("退出有道云笔记登录时失败", ex);
                }
                finally
                {
                    NavigationService.Navigate(new Uri("/Views/MainView.xaml?from=authorizePage", UriKind.Relative));
                }
            };

            wb.Navigate(logoutUri);
        }
    }
}