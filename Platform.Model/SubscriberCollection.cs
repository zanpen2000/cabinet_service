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
            this.OnBoardcastError += SubscriberCollection_OnBoardcastError;
            _subscribers = new ItemCollection<ISubscriber>("subscribers");
            _subscribers.OnItemAdd += _subscribers_OnItemAdd;
            _subscribers.OnItemRemove += _subscribers_OnItemRemove;
        }

        private void SubscriberCollection_OnBoardcastError(ISubscriber subscriber, Exception ex)
        {
            var msg = string.Format("客户端{0}({1})广播异常", subscriber.Name, subscriber.Mac);
            Log.AppendErrorInfo(msg, ex);
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

        public IEnumerable<ISubscriber> Take(Func<ISubscriber, bool> func)
        {
            if (_subscribers.Count(func) <= 0) return null;
            return _subscribers.TakeWhile(func);
        }

        /// <summary>
        /// 移除并返回一个元素
        /// </summary>
        /// <returns></returns>
        public ISubscriber Take()
        {
            return _subscribers.Take();
        }

        /// <summary>
        /// 移除并返回满足条件的元素
        /// </summary>
        /// <param name="preFunc"></param>
        /// <returns></returns>
        public IEnumerable<ISubscriber> TakeWhile(Func<ISubscriber, bool> preFunc)
        {
            return _subscribers.TakeWhile(preFunc);
        }

        /// <summary>
        /// 获取第一个元素（不移除）
        /// </summary>
        /// <returns></returns>
        public ISubscriber Get()
        {
            return _subscribers.Get();
        }

        /// <summary>
        /// 获取满足条件的元素（不移除）
        /// </summary>
        /// <param name="preFunc"></param>
        /// <returns></returns>
        public IEnumerable<ISubscriber> GetWhile(Func<ISubscriber, bool> preFunc)
        {
            return _subscribers.GetWhile(preFunc);
        }

        public void Add(ISubscriber subscriber)
        {
            var count = _subscribers.Count(sub => sub.Mac == subscriber.Mac);
            switch (count)
            {
                case 1:
                    _subscribers.TakeWhile(sub => sub.Mac == subscriber.Mac);
                    _subscribers_OnItemRemove(subscriber);
                    _subscribers.Add(subscriber);
                    _subscribers_OnItemAdd(subscriber);
                    break;
                case 0:
                    _subscribers.Add(subscriber);
                    _subscribers_OnItemAdd(subscriber);
                    break;
                default:
                    Log.AppendInfo(string.Format("客户端:{0}不是唯一({1}{2}{3})", subscriber.Name, subscriber.Mac, subscriber.IP, subscriber.Port));
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

        private void BoardcastError(ISubscriber subscriber, Exception ex)
        {
            OnBoardcastError(subscriber, ex);
        }

        public long Count
        {
            get { return _subscribers.LongCount(); }
        }
    }
}