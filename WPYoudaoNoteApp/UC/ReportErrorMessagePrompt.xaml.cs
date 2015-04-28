

namespace WPYoudaoNoteApp.UC
{
    using System.Windows;

    public partial class ReportErrorMessagePrompt
    {
        public ReportErrorMessagePrompt()
        {
            InitializeComponent();
        }

        public string UserEmail
        {
            get
            {
                return tbEmail.Text.Trim() == "选填" ? "" : tbEmail.Text.Trim();
            }
        }

        public string UserQq
        {
            get
            {
                return tbQq.Text.Trim() == "选填" ? "" : tbQq.Text.Trim();
            }
        }

        private void tbEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text.Trim() != "选填") return;
            tbEmail.Text = "";
            tbEmail.FontStyle = FontStyles.Normal;
        }

        private void tbEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbEmail.Text.Trim())) return;
            tbEmail.Text = "选填";
            tbEmail.FontStyle = FontStyles.Italic;
        }

        private void tbQq_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbQq.Text.Trim() != "选填") return;
            tbQq.Text = "";
            tbQq.FontStyle = FontStyles.Normal;
        }

        private void tbQq_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbQq.Text.Trim())) return;
            tbQq.Text = "选填";
            tbQq.FontStyle = FontStyles.Italic;
        }
    }
}
