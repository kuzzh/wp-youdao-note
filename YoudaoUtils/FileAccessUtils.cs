
using System.Linq;

namespace YoudaoNoteUtils
{
    using System.Collections.Generic;
    using System.IO;

    public class FileAccessUtils
    {

        //储存所有文件夹名
        private readonly List<string> _dirs;

        public FileAccessUtils()
        {
            _dirs = new List<string>();
        }

        //获取所有文件名
        private static IEnumerable<string> GetFileName(string dirPath)
        {
            var list = new List<string>();

            if (Directory.Exists(dirPath))
            {
                list.AddRange(Directory.GetFiles(dirPath));
            }
            return list;
        }

        //获取所有文件夹及子文件夹
        private void GetDirs(string dirPath)
        {
            if (Directory.GetDirectories(dirPath).Length <= 0) return;
            foreach (var path in Directory.GetDirectories(dirPath))
            {
                _dirs.Add(path);
                GetDirs(path);
            }
        }

        /// <summary>
        /// 获取给出文件夹及其子文件夹下的所有文件名
        /// （文件名为路径加文件名及后缀,
        /// 使用的时候GetAllFileName().ToArray()方法可以转换成object数组
        /// 之后再ToString()分别得到文件名）
        /// </summary>
        /// <param name="rootPath">文件夹根目录</param>
        /// <returns></returns>
        public List<string> GetAllFileName(string rootPath)
        {
            _dirs.Add(rootPath);
            GetDirs(rootPath);
            object[] allDir = _dirs.ToArray();

            var list = new List<string>();

            foreach (var o in allDir)
            {
                list.AddRange(GetFileName(o.ToString()));
            }

            return list;
        }

        /// <summary>
        /// 如果上个方法不知道怎么用，那就调用这个方法吧
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public List<string> FileName(string rootPath)
        {
            return (from object o in GetAllFileName(rootPath).ToArray() select o.ToString()).ToList();
        }
    }
}
