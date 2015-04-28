
using System.Linq;

namespace YoudaoNoteUtils
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Text;
    using System.Windows;

    public static class IsoStoreUtil
    {   

        public static void SaveFilesToIsoStore(string rootDir, bool cover = false)
        {
            //These files must match what is included in the application package,
            //or BinaryStream.Dispose below will throw an exception.
            //string[] files = Directory.GetFiles("Assets/ueditor");
            var fileAccess = new FileAccessUtils();
            var files = fileAccess.GetAllFileName(rootDir);

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (var file in files)
                {
                    if (cover)
                    {
                        performSave(file);
                    }
                    else
                    {
                        if (!isoStore.FileExists(file))
                        {
                            performSave(file);
                        }
                    }
                }
            }
        }

        private static void performSave(string file)
        {
            var sr = Application.GetResourceStream(new Uri(file, UriKind.Relative));
            using (var br = new BinaryReader(sr.Stream))
            {
                var data = br.ReadBytes((int)sr.Stream.Length);
                SaveToIsoStore(file, data);
            }
        }

        public static string ReadFileAsString(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(filePath))
                {
                    using (var fs = isoStore.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fs, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException("File not found:" + filePath);
                }
            }
        }

        public static byte[] ReadFileAsByte(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(filePath))
                {
                    using (var fs = isoStore.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(fs, Encoding.UTF8))
                        {
                            var buffer = new byte[fs.Length];
                            return reader.ReadBytes((int)fs.Length);
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException("File not found:" + filePath);
                }
            }
        }

        public static long GetFolderSize(string folderPattern)
        {
            long total = 0;
            var folder = Path.GetDirectoryName(folderPattern);
            if (string.IsNullOrEmpty((folder)))
            {
                return 0;
            }
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists(folder))
                {
                    return 0;
                }
                var fileNames = isoStore.GetFileNames(folderPattern);
                foreach (var fileName in fileNames)
                {
                    using (var file = isoStore.OpenFile(Path.Combine(folder, fileName), FileMode.Open))
                    {
                        total += file.Length;
                    }
                }
            }
            return total;
        }

        public static void DeleteFileFromFolder(string folderPattern)
        {
            if (string.IsNullOrEmpty(folderPattern))
            {
                throw new ArgumentNullException("folderPattern");
            }
            var folder = Path.GetDirectoryName(folderPattern);
            if (string.IsNullOrEmpty((folder)))
            {
                return;
            }
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (var filePath in isoStore.GetFileNames(folderPattern).Select(fileName => Path.Combine(folder, fileName)))
                {
                    DeleteFileFormIsoStore(filePath);
                }
            }
        }

        private static void DeleteFileFormIsoStore(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }
            //Get the IsoStore.
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //Remove the existing file.
                if (!isoStore.FileExists(filePath)) return;
                isoStore.DeleteFile(filePath);
                Debug.WriteLine("删除本地图片：" + filePath);
            }
        }

        public static string SaveToIsoStore(string fileName, byte[] data, bool cover = true)
        {
            var strBaseDir = string.Empty;
            const string delimStr = "\\";
            var delimiter = delimStr.ToCharArray();
            fileName = fileName.Replace('/', '\\');
            var dirsPath = fileName.Split(delimiter);

            //Get the IsoStore.
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //Re-create the directory structure.
                for (var i = 0; i < dirsPath.Length - 1; i++)
                {
                    strBaseDir = Path.Combine(strBaseDir, dirsPath[i]);
                    isoStore.CreateDirectory(strBaseDir);
                }

                //Remove the existing file.
                if (isoStore.FileExists(fileName))
                {
                    if (cover)
                    {
                        isoStore.DeleteFile(fileName);
                    }
                    else
                    {
                        using (var fs = isoStore.OpenFile(fileName, FileMode.Open))
                        {
                            return fs.Name;
                        }
                    }
                }

                //Write the file.
                using (var fs = isoStore.CreateFile(fileName))
                {
                    using (var bw = new BinaryWriter(fs))
                    {
                        bw.Write(data);
                    }

                    return fs.Name;
                }
            }
        }

        public static bool FileExists(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isoStore.FileExists(filename);
            }
        }

        public static string GetFileAbsolutePath(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(filename))
                {
                    using (var fs = isoStore.OpenFile(filename, FileMode.Open))
                    {
                        return fs.Name;
                    }
                }
            }
            throw new FileNotFoundException("File not found: " + filename);
        }
    }
}
