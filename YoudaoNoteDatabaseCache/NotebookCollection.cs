using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoudaoNoteDataAccess;

namespace YoudaoNoteDatabaseCache
{
    public class NotebookCollection : SortableObservableCollection<NotebookEntity>
    {
         public NotebookCollection()
        {
            
        }

         public NotebookCollection(List<NotebookEntity> list)
            : base(list)
        {
        }

         public void AddRange(IEnumerable<NotebookEntity> entities)
         {
             if (null == entities)
             {
                 return;
             }
             foreach (var entity in entities)
             {
                 Add(entity);
             }
         }
    }
}
