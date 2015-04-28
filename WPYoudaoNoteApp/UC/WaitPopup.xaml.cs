


namespace WPYoudaoNoteApp.UC
{
    using System.Windows.Controls.Primitives;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using WPYoudaoNoteApp.Utils;
    using System.Windows;
using System.ComponentModel;

    public sealed partial class WaitPopup : IDisposable
    {
        private bool _disposed;
        private Popup _popupControl;
        private readonly ApplicationBar _appBar;
        private readonly PhoneApplicationPage _parentPage;

        public WaitPopup()
        {
            InitializeComponent();

            createPopup(0, 0);
        }

        public WaitPopup(string tip, PhoneApplicationPage page = null)
            : this()
        {
            _parentPage = page;
            if (null != _parentPage)
            {
                _appBar = _parentPage.ApplicationBar as ApplicationBar;

                _parentPage.BackKeyPress += onPageBackKeyPress;
            }
            SetTip(tip);

            Show();
        }

        ~WaitPopup()
        {
            //必须为false
            Dispose(false);
        }

        private void onPageBackKeyPress(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public bool IsOpen
        {
            get { return _popupControl.IsOpen; }
        }

        private void Show()
        {
            _popupControl.IsOpen = true;

            if (null != _appBar)
            {
                _appBar.IsVisible = false;
            }
        }

        public void Hide()
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                doHide();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => doHide());
            }
            
        }

        private void doHide()
        {
            _popupControl.IsOpen = false;

            if (null != _appBar)
            {
                _appBar.IsVisible = true;
            }
        }

        public void SetTip(string tip)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                ProgressBar.Tip = tip;
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => ProgressBar.Tip = tip);
            }
        }
       
        private void createPopup(double x, double y)
        {
            _popupControl = new Popup
            {
                Child = this,
                IsOpen = false,
                HorizontalOffset = x,
                VerticalOffset = y,
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Width = ConstantPool.ScreenWidth;
            Height = ConstantPool.ScreenHeight;
        }

        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 非密封类修饰用protected virtual
        /// 密封类修饰用private
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (null != _parentPage)
                {
                    _parentPage.BackKeyPress -= onPageBackKeyPress;
                }
                // 清理托管资源
                Hide();
            }

            //让类型知道自己已经被释放
            _disposed = true;
        }

    }
}
