using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Platform.Service.Contracts;
using Platform.Service.Implement;
using Platform.Layer;
using Platform.Model.Tests;

namespace Platform.Service.Implement.Tests
{
    [TestClass()]
    public class ServiceTest : IDuplexChannelCallback
    {
        private IDuplexChannelService DuplexService;

        private int ClientCount;

        public ServiceTest()
        {
            ClientCount = 0;
            i = 0;
            DuplexService = ProxyFactory.GetProxy<IDuplexChannelService, IDuplexChannelCallback>(this);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod()]
        public void OnOffLineTest()
        {
            i = 0;
            DuplexService.Online("c1", "c1_mac");
            waitCallback();

            i = 0;
            DuplexService.GetClients();
            waitCallback();

            Assert.AreEqual(1, ClientCount);

            i = 0;
            DuplexService.Offline("c1_mac");
            waitCallback();

            i = 0;
            DuplexService.GetClients();
            waitCallback();


            Assert.AreEqual(0, ClientCount);

        }

        public void OnlineStateChanged(string mac, OnlineState state)
        {
            TestContext.WriteLine("客户端：{0} 状态: {1}", mac, state.ToString());
            ++i;
        }

        public void NotifyMessage(string msg)
        {
            TestContext.WriteLine(msg);
            ++i;
        }

        public void ReturnClients(System.Collections.Generic.IEnumerable<string> clientMacs)
        {
            TestContext.WriteLine("当前在线客户端：");

            ClientCount = 0;

            foreach (var mac in clientMacs)
            {
                TestContext.WriteLine("\t" + mac);
                ClientCount += 1;
            }
            ++i;
        }

        private int i = 0;

        private void waitCallback()
        {
            while (i == 0)
            {
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
