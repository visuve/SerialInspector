using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SerialInspector.Model
{
    internal class ObservableSet<T> : ObservableCollection<T>
    {
        private EqualityComparer<T> equalityComparer;

        internal ObservableSet(EqualityComparer<T> equalityComparer)
        {
            this.equalityComparer = equalityComparer;
        }

        protected override void InsertItem(int index, T item)
        {
            int previousIndex = Array.FindIndex(Items.ToArray(), (x) => equalityComparer.Equals(x, item));

            if (previousIndex >= 0)
            {
                base.SetItem(previousIndex, item);
                return;
            }

            base.InsertItem(index, item);
        }
    }
}
