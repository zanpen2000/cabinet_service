using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Platform.Model.Interfaces;

namespace Platform.Model
{
    public delegate void ItemCollectionItemAddEventHandler<in T>(T t);
    public delegate void ItemCollectionItemRemoveEventHandler<in T>(T t);

    public class ItemCollection<T> : IEnumerable<T>
    {
        public event ItemCollectionItemAddEventHandler<T> OnItemAdd = delegate { };
        public event ItemCollectionItemRemoveEventHandler<T> OnItemRemove = delegate { };

        private readonly List<T> _items;

        private static object _lock = new object();

        public ItemCollection(string name)
        {
            Name = name;
            _items = new List<T>();
        }

        public string Name { get; set; }

        public int Count
        {
            get { return _items.Count; }
        }

        public void Add(T t)
        {
            lock (_lock)
            {
                if (_items.Contains(t)) return;
                _items.Add(t);
                OnItemAdd(t);
            }
        }

        public IEnumerable<T> TakeWhile(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                var lst = new List<T>(_items.TakeWhile(predicate));
                foreach (var l in lst)
                {
                    if (_items.Remove(l))
                        OnItemRemove(l);
                }

                return lst;
            }
        }

        public T Take()
        {
            lock (_lock)
            {
                var t = _items.Take(1).FirstOrDefault();
                if (_items.Remove(t))
                    OnItemRemove(t);
                return t;
            }
        }

        public T Get()
        {
            lock (_lock)
            {
                return _items.Take(1).FirstOrDefault();
            }
        }

        public IEnumerable<T> GetWhile(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                return _items.Where(predicate);
            }
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
