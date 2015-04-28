

namespace WPYoudaoNoteApp.Views
{
    using Microsoft.Phone.Controls;
    using System.Windows;
    using Utils;
    public partial class BootStrapView : PhoneApplicationPage
    {
        public BootStrapView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (ConstantPool.ShowBulletinBoard)
            //{
            //    var content = "本次版本有重大更新（Version 1.5.0.0），如果你是新安装的 APP，请无视下面的内容。如果你是从以往的版本升级的，请先卸载再从新安装，否则可能会出问题。给您带来的不便深表歉意！";
            //    BBSBoard.Title = "重要通知";
            //    BBSBoard.Content = content;
            //    BBSBoard.Visibility = System.Windows.Visibility.Visible;
            //    StartTextBlock.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            {
                NavigationService.Navigate(new System.Uri("/Views/MainView.xaml?from=bootstrap", System.UriKind.Relative));
            }
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ConstantPool.ShowBulletinBoard = false;
            NavigationService.Navigate(new System.Uri("/Views/MainView.xaml", System.UriKind.Relative));
        }
    }
}