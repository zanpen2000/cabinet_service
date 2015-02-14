using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Platform.Model
{
    public delegate void ItemCollectionItemAddEventHandler<in T>(T t);
    public delegate void ItemCollectionItemRemoveEventHandler<in T>(T t);

    public class ItemCollection<T> : IEnumerable<T>
    {
        public event ItemCollectionItemAddEventHandler<T> OnItemAdd = delegate { };
        public event ItemCollectionItemRemoveEventHandler<T> OnItemRemove = delegate { };

        private readonly BlockingCollection<T> _items;
        public ItemCollection(string name)
        {
            this.Name = name;
            _items = new BlockingCollection<T>();
        }

        public string Name { get; set; }

        public int Count
        {
            get { return _items.Count; }
        }

        public void Add(T t)
        {
            if (!_items.Contains(t))
            {
                _items.Add(t);
                OnItemAdd(t);
            }
        }

        public void Remove(Func<T, bool> predicate)
        {
            var obj = (T)_items.TakeWhile(predicate);
            OnItemRemove(obj);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.TakeWhile(item => item != null).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.TakeWhile(item => item != null).GetEnumerator();
        }
    }
}
