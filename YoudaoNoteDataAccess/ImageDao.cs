

namespace YoudaoNoteDataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public static class ImageDao
    {
        public static void InsertIfNotExist(ImageEntity imgEntity)
        {
            if (null == imgEntity)
            {
                throw new ArgumentNullException("imgEntity");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.ImageTable.FirstOrDefault(i => i.Id == imgEntity.Id);
                if (null != entity) return;
                dc.ImageTable.InsertOnSubmit(imgEntity);
                dc.SafeSubmitChanges();
            }
        }

        private static void DeleteIfExist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.ImageTable.FirstOrDefault(p => p.Id == id);
                if (null == entity) return;
                dc.ImageTable.DeleteOnSubmit(entity);
                dc.SafeSubmitChanges();
            }
        }

        public static void ModifyIfExist(ImageEntity entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException("entity");
            }
            using (var dc = new MyDataContext())
            {
                var pic = dc.ImageTable.FirstOrDefault(p => p.Id == entity.Id);
                if (null == pic) return;
                pic.ImgLocalSavePath = entity.ImgLocalSavePath;
                pic.BelongedNotePath = entity.BelongedNotePath;
                pic.ImgRemoteUrl = entity.ImgRemoteUrl;

                dc.SafeSubmitChanges();
            }
        }

        public static ImageEntity GetImageById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.ImageTable.FirstOrDefault(p => p.Id == id);
                return entity;
            }
        }

        public static ImageEntity GetImageByLocalSavePath(string localSavePath)
        {
            if (string.IsNullOrEmpty(localSavePath))
            {
                throw new ArgumentNullException("localSavePath");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.ImageTable.FirstOrDefault(p => p.ImgLocalSavePath == localSavePath);
                return entity;
            }
        }

        /// <summary>
        /// 删除悬空图片（没有笔记引用的图片）。
        /// </summary>
        public static void DeleteNoRootImages()
        {
            using (var dc = new MyDataContext())
            {
                if (!dc.DatabaseExists()) return;

                foreach (var img in dc.ImageTable)
                {
                    if (NoteDao.Inst.NoteExist((img.BelongedNotePath))) continue;
                    DeleteIfExist(img.Id);
                    Debug.WriteLine("删除本地悬空图片：" + img.ImgLocalSavePath);
                }
            }
        }

        public static void DeleteImageByBelongedNotePath(string belongedNotePath)
        {
            if (string.IsNullOrEmpty(belongedNotePath))
            {
                throw new ArgumentNullException("belongedNotePath");
            }
            using (var dc = new MyDataContext())
            {
                var entities = from image in dc.ImageTable
                    where image.BelongedNotePath.Equals(belongedNotePath)
                    select image;
                foreach (var entity in entities)
                {
                    dc.ImageTable.DeleteOnSubmit(entity);
                }

                dc.SafeSubmitChanges();
            }
        }

        public static bool ImageExists(string imgNameWithoutExt)
        {
            if (string.IsNullOrEmpty(imgNameWithoutExt))
            {
                throw new ArgumentNullException("imgNameWithoutExt");
            }
            using (var dc = new MyDataContext())
            {
                var entity = dc.ImageTable.First(p => p.ImgNameWithoutExt == imgNameWithoutExt);
                return (entity != null);
            }
        }

        public static bool ImageExists(string imgNameWithoutExt, out ImageEntity img)
        {
            if (string.IsNullOrEmpty(imgNameWithoutExt))
            {
                throw new ArgumentNullException("imgNameWithoutExt");
            }
            using (var dc = new MyDataContext())
            {
                img = dc.ImageTable.FirstOrDefault(p => p.ImgNameWithoutExt == imgNameWithoutExt);
                return (img != null);
            }
        }

        public static List<string> GetCachedNotePathList()
        {
            using (var dc = new MyDataContext())
            {
                return (from img in dc.ImageTable select img.BelongedNotePath).Distinct().ToList();
            }
        }
    }
}
