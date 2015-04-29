

namespace WPYoudaoNoteApp.Utils
{
    using C1.Phone;
    using Microsoft.Phone.Info;
    using Microsoft.Phone.Net.NetworkInformation;
    using Microsoft.Phone.Shell;
    using System;
    using System.IO.IsolatedStorage;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using YoudaoNoteDataAccess;
    using YoudaoNoteLog;
    using YoudaoNoteUtils;

    internal static class Util
    {
        internal static ApplicationBarIconButton AddApplicationBarButton(IApplicationBar bar, string imageUrl, string buttonName, EventHandler action = null)
        {
            var applicationBarIconButton = new ApplicationBarIconButton(new Uri(imageUrl, UriKind.Relative))
            {
                Text = buttonName
            };
            if (action != null)
                applicationBarIconButton.Click += action;
            bar.Buttons.Add(applicationBarIconButton);
            return applicationBarIconButton;
        }

        internal static ApplicationBarMenuItem AddApplicationBarMenuItem(IApplicationBar bar, string menuText, EventHandler action = null)
        {
            var applicationBarMenuItem = new ApplicationBarMenuItem(menuText);
            if (action != null)
                applicationBarMenuItem.Click += action;
            bar.MenuItems.Add(applicationBarMenuItem);
            return applicationBarMenuItem;
        }



        internal static NotebookEntity ReadLastSelectedNotebook()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool) return null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(ConstantPool.LastSelectedNotebookKey))
            {
                return IsolatedStorageSettings.ApplicationSettings[ConstantPool.LastSelectedNotebookKey] as NotebookEntity;
            }

            return null;
        }
        internal static void SaveLastSelectedNotebook(NotebookEntity notebookEntity)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool) return;
            IsolatedStorageSettings.ApplicationSettings[ConstantPool.LastSelectedNotebookKey] = notebookEntity;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        internal static bool IsNetworkAvailable()
        {
            return DeviceNetworkInformation.IsNetworkAvailable;
        }

        internal static long GetCacheSize()
        {
            return IsoStoreUtil.GetFolderSize("Images\\*");
        }

        internal static void ClearImageCache()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.DirectoryExists("Images\\"))
                {
                    IsoStoreUtil.DeleteFileFromFolder("Images\\*");
                }
            }
        }

        internal static void SendErrorReportAsync()
        {
            Task.Factory.StartNew(() =>
            {
                // 如果有错误并且有网络，提示用户报告错误
                if (!IsNetworkAvailable()) return;
                var logContent = LoggerFactory.GetLogger().GetLogContent();
                if (string.IsNullOrEmpty(logContent) || logContent.Length <= 0) return;
                var content = new StringBuilder();
                content.AppendLine("设备名称：" + DeviceStatus.DeviceName);
                content.AppendLine("设备制造商：" + DeviceStatus.DeviceManufacturer);
                content.AppendLine("设备最大内存：" + (DeviceStatus.DeviceTotalMemory / 1024.0 / 1024.0).ToString("0.00") + " MB");
                content.AppendLine("设备固件版本：" + DeviceStatus.DeviceFirmwareVersion);
                content.AppendLine("设备硬件版本：" + DeviceStatus.DeviceHardwareVersion);
                content.AppendLine("版本：" + ConstantPool.AppVersion);
                content.AppendLine();
                content.AppendLine(logContent);

                ReportErrorAsync(content.ToString());
            });
        }

        internal static void ReportErrorAsync(string body)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        const string url = "填写错误日志接收页面网址";
                        var postData = new PostData();
                        postData.Params.Add(new PostDataParam("body", body, PostDataParamType.Field));

                        var reqResult = HttpUtils.SendPostRequest(url, postData);

                        if (reqResult.OperateResult == OperateResultEnum.Success)
                        {
                            LoggerFactory.GetLogger().SafeDeleteFile();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                });
        }

    }
}
