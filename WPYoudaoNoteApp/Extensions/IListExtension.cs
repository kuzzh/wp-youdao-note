using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YoudaoNoteDataAccess;

namespace WPYoudaoNoteApp.Extensions
{
    public static class IListExtension
    {
        public static void AddRange(this IList list, IEnumerable<NoteEntity> values)
        {
            if (null == values)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count() <= 0) return;

            foreach (var item in values)
            {
                list.Add(item);
            }
        }
    }
}
