



namespace WPYoudaoNoteApp.Extensions
{
    using System;
    using System.Windows.Controls;

    public static class ControlExtensions
    {
        public static void SafeInvoke<T>(this T control, Action action) where T : Control
        {
            if (control.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                control.Dispatcher.BeginInvoke(action);
            }
        }
    }
}
