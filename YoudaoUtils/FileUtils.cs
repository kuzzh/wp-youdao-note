

namespace YoudaoNoteUtils
{
    using System.IO;

    public static class FileUtils
    {
        public static string GetFileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }

        //internal static string GetFileContentType(string filename)
        //{
        //    var array = filename.Split('.');
        //    var result = string.Empty;
        //    var suffix = "." + array[array.Length - 1];
        //    var rg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(suffix);
        //    var obj = rg.GetValue("Content Type");
        //    result = obj != null ? obj.ToString() : null;
        //    rg.Close();
        //    return result;
        //}
    }
}
