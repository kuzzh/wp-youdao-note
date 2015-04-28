using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace YoudaoNoteDatabaseCache
{
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public SortableObservableCollection()
        {
        }

        public SortableObservableCollection(List<T> list)
            : base(list)
        {
        }

        public SortableObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, System.ComponentModel.ListSortDirection direction)
        {
            switch (direction)
            {
                case System.ComponentModel.ListSortDirection.Ascending:
                {
                    applySort(Items.OrderBy(keySelector));
                    break;
                }
                case System.ComponentModel.ListSortDirection.Descending:
                {
                    applySort(Items.OrderByDescending(keySelector));
                    break;
                }
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            applySort(Items.OrderBy(keySelector, comparer));
        }

        private void applySort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }
    }
}