

using System;
using System.Diagnostics;

namespace YoudaoNoteDataAccess
{
    using System.Data.Linq;

    public class MyDataContext : DataContext
    {
        /// <summary>
        /// 连接字符串。
        /// </summary>
        private const string ConnectStr = "Data Source = 'isostore:/db.sdf';Password='123456';Max Database Size = 512; Max Buffer Size = 1024;";

        public MyDataContext()
            : base(ConnectStr)
        {

        }

        public static void CreatetDatabaseIfNeeded()
        {
            using (var dc = new MyDataContext())
            {
                if (dc.DatabaseExists()) return;
                dc.CreateDatabase();
                Debug.WriteLine(DateTime.Now.ToLongTimeString() + " 数据库创建完成。");
            }
        }

        public static void DeleteDatabaseIfExists()
        {
            using (var dc = new MyDataContext())
            {
                if (dc.DatabaseExists())
                {
                    dc.DeleteDatabase();
                }
                Debug.WriteLine(DateTime.Now.ToLongTimeString() + " 数据库已被删除。");
            }
        }

        public void SafeSubmitChanges()
        {
            try
            {
                SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException)
            {
                // ChangeConflicts.ResolveAll(RefreshMode.KeepCurrentValues); //保持当前的值
                // ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues); //保持原来的更新,放弃了当前的值.
                ChangeConflicts.ResolveAll(RefreshMode.KeepChanges); //保存原来的值 有冲突的话保存当前版本

                // 注意：解决完冲突后还得 SubmitChanges() 一次，不然一样是没有更新到数据库的
                SubmitChanges();
            }
        }

        /// <summary>
        /// 对应数据库中的笔记本表。
        /// </summary>
        public Table<NotebookEntity> NotebookTable;
        /// <summary>
        /// 对应数据库中的笔记表。
        /// </summary>
        public Table<NoteEntity> NoteTable;
        /// <summary>
        /// 对应数据库中的图片表。
        /// </summary>
        public Table<ImageEntity> ImageTable;
    }
}
