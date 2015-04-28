

namespace WPYoudaoNoteApp.Utils
{
    using System;
    using System.IO.IsolatedStorage;
    using System.Windows;
    using System.Windows.Media;

    internal static class ConstantPool
    {
        private const string AccessTokenKey = "AccessToken";
        private const string AutoSendErrorReportKey = "AutoSendErrorReport";
        internal const string ConsumerKey = "a82cf830df2780731c65783ddb82b207";
        internal const string LastSelectedNotebookKey = "LastSelectedNotebook";
        internal const string ProductId = "{2263ebee-9d8d-41b2-b256-df240fe07ce8}";
        internal static readonly double ScreenWidth = Application.Current.Host.Content.ActualWidth;
        internal static readonly double ScreenHeight = Application.Current.Host.Content.ActualHeight;
        internal static readonly Color AppForeColor = Color.FromArgb(255, 147, 196, 210);

        internal static string AccessToken
        {
            get
            {
                if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                {
                    return IsolatedStorageSettings.ApplicationSettings.Contains(AccessTokenKey) ? IsolatedStorageSettings.ApplicationSettings[AccessTokenKey].ToString() : "";
                }
                return "";
            }
            set
            {
                if (System.ComponentModel.DesignerProperties.IsInDesignTool) return;
                IsolatedStorageSettings.ApplicationSettings[AccessTokenKey] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        internal static bool AutoSendErrorReport
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(AutoSendErrorReportKey))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[AutoSendErrorReportKey];
                }
                return true;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[AutoSendErrorReportKey] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        internal static Version AppVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        internal static bool ShowBulletinBoard
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(AppVersion.ToString()))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[AppVersion.ToString()];
                }
                return true;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[AppVersion.ToString()] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
    }
}
