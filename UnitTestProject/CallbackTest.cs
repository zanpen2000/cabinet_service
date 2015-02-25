using Platform.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Platform.Model.Tests
{
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
