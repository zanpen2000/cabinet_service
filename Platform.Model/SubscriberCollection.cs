using System;
using System.Collections.Generic;
using System.Linq;
using Logger;
using Platform.Model.Interfaces;
using Platform.Service.Contracts;

namespace Platform.Model
{
    public delegate void BoardcastEventHandler(ISubscriber subscriber, Exception ex);

    public delegate void CollectionChangedEventHandler(ISubscriber subscriber, OnlineState state);
    public class SubscriberCollection
    {
        #region 单例

        private static readonly object SyncLock = new object();
        private static SubscriberCollection _instance;

        public static SubscriberCollection Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SubscriberCollection();
                        }
                    }
                }
                return _instance;
            }
        }

        private readonly ItemCollection<ISubscriber> _subscribers;

        private SubscriberCollection()
        {
            _subscribers = new ItemCollection<ISubscriber>("subscribers");
            _subscribers.OnItemAdd += _subscribers_OnItemAdd;
            _subscribers.OnItemRemove += _subscribers_OnItemRemove;
        }

        #endregion

        public ItemCollection<ISubscriber> Subscribers { get { return _subscribers; } }

        private void _subscribers_OnItemRemove(ISubscriber t)
        {
            Log.AppendInfo(string.Format("客户端:{0}已移除({1}{2}{3})", t.Name, t.Mac, t.IP, t.Port));
        }

        private void _subscribers_OnItemAdd(ISubscriber t)
        {
            Log.AppendInfo(string.Format("客户端:{0}已添加({1}{2}{3})", t.Name, t.Mac, t.IP, t.Port));
        }

        public void Remove(Func<ISubscriber, bool> func)
        {
            if (_subscribers.Count(func) <= 0) return;
            var subs = _subscribers.Take(func);

            foreach (var subscriber in subs)
            {
                OnCollectionChanged(subscriber, OnlineState.Offline);
            }
        }

        public void Add(ISubscriber subscriber)
        {
            var count = _subscribers.Count(sub => sub.Mac == subscriber.Mac);
            switch (count)
            {
                case 1:
                    _subscribers.Take(sub => sub.Mac == subscriber.Mac);
                    OnCollectionChanged(subscriber, OnlineState.Offline);
                    _subscribers.Add(subscriber);
                    OnCollectionChanged(subscriber, OnlineState.Online);
                    break;
                case 0:
                    _subscribers.Add(subscriber);
                    OnCollectionChanged(subscriber, OnlineState.Online);
                    break;
                default:
                    throw new Exception("不是唯一的订阅者");
            }
        }

        public void Boardcast(string message)
        {
            foreach (var sub in _subscribers)
            {
                try
                {
                    sub.Notify(message);
                }
                catch (Exception ex)
                {
                    OnBoardcastError(sub, ex);
                }
            }
        }

        public void Boardcast(string mac, string message)
        {
            foreach (var sub in _subscribers.Where(s => s.Mac == mac))
            {
                try
                {
                    sub.Notify(message);
                }
                catch (Exception ex)
                {
                    OnBoardcastError(sub, ex);
                }
            }
        }

        public void Boardcast(IEnumerable<string> macs, string message)
        {
            foreach (var sub in from sub in _subscribers
                                let enumerable = (IList<string>)(macs as IList<string> ?? macs.ToList())
                                where enumerable.Contains(sub.Mac)
                                select sub)
            {
                try
                {
                    sub.Notify(message);
                }
                catch (Exception ex)
                {
                    OnBoardcastError(sub, ex);
                }
            }
        }

        public event BoardcastEventHandler OnBoardcastError = delegate { };
        public event CollectionChangedEventHandler OnCollectionChanged = delegate { };

        private void BoardcastError(ISubscriber subscriber, Exception ex)
        {
            OnBoardcastError(subscriber, ex);
        }
    }
}