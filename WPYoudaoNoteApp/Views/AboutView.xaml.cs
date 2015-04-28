
namespace WPYoudaoNoteApp.Views
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using WPYoudaoNoteApp.Utils;
    using YoudaoNoteLog;
    using YoudaoNoteUtils;

    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();

            tbVersion.Text = ConstantPool.AppVersion.ToString();
        }

        private string UserEmail
        {
            get
            {
                return tbEmail.Text.Trim() == "选填" ? "" : tbEmail.Text.Trim();
            }
        }

        private string UserQq
        {
            get
            {
                return tbQq.Text.Trim() == "选填" ? "" : tbQq.Text.Trim();
            }
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void tbFeedback_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbFeedback.Text.Trim() != "请在此写下您的反馈意见……") return;
            tbFeedback.Text = "";
            tbFeedback.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void btnSendFeedback_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbFeedback.Text.Trim()) || tbFeedback.Text.Trim() == "请在此写下您的反馈意见……")
            {
                Toast.Prompt("额，请先填写反馈意见！");
                tbFeedback.Focus();
                return;
            }
            var feedback = new StringBuilder();
            feedback.AppendLine("反馈内容：" + tbFeedback.Text.Trim());
            feedback.AppendLine("用户邮箱地址：" + UserEmail);
            feedback.AppendLine("用户 QQ：" + UserQq);

            btnSendFeedback.IsEnabled = false;
            btnGoHome.IsEnabled = false;
            progressBar.Tip = "正在发送中...";
            progressBar.Visibility = Visibility.Visible;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    const string url = ""; // 此处填写接收反馈信息的网址
                    var postData = new PostData();
                    postData.Params.Add(new PostDataParam("feedback", feedback, PostDataParamType.Field));

                    var reqResult = HttpUtils.SendPostRequest(url, postData);

                    Dispatcher.BeginInvoke(() =>
                                {
                                    if (reqResult.OperateResult == OperateResultEnum.Success)
                                    {
                                        Toast.Prompt("已发送反馈意见！");
                                    }
                                    else
                                    {
                                        LoggerFactory.GetLogger().Error("发送反馈意见失败：" + reqResult.ErrorMsg);
                                        Toast.Prompt("额，发送反馈意见失败，请稍后重试！");
                                    }
                                });
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().Error("发送反馈意见失败", ex);
                    Toast.Prompt("额，发送反馈意见失败，请稍后重试！");
                }
                finally
                {
                    Dispatcher.BeginInvoke(() =>
                        {
                            progressBar.Visibility = Visibility.Collapsed;
                            btnSendFeedback.IsEnabled = true;
                            btnGoHome.IsEnabled = true;
                        });
                }
            });
        }

        private void tbFeedback_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbFeedback.Text.Trim())) return;
            tbFeedback.Text = "请在此写下您的反馈意见……";
            tbFeedback.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void tbEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text.Trim() != "选填") return;
            tbEmail.Text = "";
            tbEmail.FontStyle = FontStyles.Normal;
            tbEmail.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void tbEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbEmail.Text.Trim())) return;
            tbEmail.Text = "选填";
            tbEmail.FontStyle = FontStyles.Italic;
            tbEmail.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void tbQq_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbQq.Text.Trim() != "选填") return;
            tbQq.Text = "";
            tbQq.FontStyle = FontStyles.Normal;
            tbQq.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void tbQq_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbQq.Text.Trim())) return;
            tbQq.Text = "选填";
            tbQq.FontStyle = FontStyles.Italic;
            tbQq.Foreground = new SolidColorBrush(Colors.Gray);
        }
    }
}