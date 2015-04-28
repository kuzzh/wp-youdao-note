

namespace WPYoudaoNoteApp.Utils
{
    using Coding4Fun.Toolkit.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal static class Toast
    {
        internal static void Prompt(string msg)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                var toast = CreateToastPrompt(msg);

                toast.Show();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var toast = CreateToastPrompt(msg);

                    toast.Show();
                });
            }
        }

        private static ToastPrompt CreateToastPrompt(string msg)
        {
            var toast = new ToastPrompt
            {
                Title = "Sunrise",
                Message = msg,
                TextOrientation = Orientation.Vertical,
                Background = new SolidColorBrush(ConstantPool.AppForeColor),
                MillisecondsUntilHidden = 2000
            };
            return toast;
        }
    }
}
