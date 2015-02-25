using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Platform.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Platform.Model.Interfaces;
using Platform.Service.Contracts;

namespace Platform.Model.Tests
{
    [TestClass()]
    public class SubscriberCollectionTests
    {
        [TestMethod()]
        public void AddAndRemoveTest()
        {
            CallbackTest cbtest = new CallbackTest();
            ISubscriber sub = new Subscriber("client1", "client1_mac", "client1_ip", 5000, cbtest);

            SubscriberCollection.Default.Add(sub);

            Assert.AreEqual(1, SubscriberCollection.Default.Count);

            //执行两次确定不会重复添加
            SubscriberCollection.Default.Add(sub);
            Assert.AreEqual(1, SubscriberCollection.Default.Count);

            SubscriberCollection.Default.Take();
            Assert.AreEqual(0, SubscriberCollection.Default.Count);

            SubscriberCollection.Default.TakeWhile(t => t.Mac == "client1_mac");
            Assert.AreEqual(0, SubscriberCollection.Default.Count);

            SubscriberCollection.Default.Add(sub);

            var s1 = SubscriberCollection.Default.Get();
            Assert.AreEqual(1, SubscriberCollection.Default.Count);
            Assert.AreEqual("client1", s1.Name);

            var s2 = SubscriberCollection.Default.GetWhile(x => x.Mac == "client1_mac");
            Assert.AreEqual(1, SubscriberCollection.Default.Count);
            Assert.AreEqual(s2.First().Name, "client1");

            //移除指定元素
            SubscriberCollection.Default.TakeWhile(t => t.Mac == "client1_mac");
            Assert.AreEqual(0, SubscriberCollection.Default.Count);
        }
    }

    public class CallbackTest : IDuplexChannelCallback
    {
        public void OnlineStateChanged(string mac, OnlineState state)
        {
            Debug.WriteLine("客户端：{0} 状态: {1}", mac, state.ToString());
        }

        public void NotifyMessage(string msg)
        {
            Debug.WriteLine(msg);
        }

        public void ReturnClients(IEnumerable<string> clientMacs)
        {
            Debug.WriteLine("当前在线客户端:");
            foreach (var clientMac in clientMacs)
            {
                Debug.WriteLine("\t" + clientMac);
            }
        }
    }
}
