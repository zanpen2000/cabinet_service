using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Platform.Model;
using Platform.Service.Contracts;

namespace Platform.Service.Implement
{

    internal class DuplexChannelService : IDuplexChannelService
    {
        #region 单例

        private static readonly object SyncLock = new object();
        private static DuplexChannelService _instance;

        public static DuplexChannelService Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DuplexChannelService();
                        }
                    }
                }
                return _instance;
            }
        }

        private DuplexChannelService() { }

        #endregion

        public void Online(string name, string mac)
        {
            var rempEndpointMessageProperty =
                OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as
                    RemoteEndpointMessageProperty;
            IDuplexChannelCallback callback = OperationContext.Current.GetCallbackChannel<IDuplexChannelCallback>();

            if (rempEndpointMessageProperty != null)
            {
                Subscriber subscriber = new Subscriber(name, mac, rempEndpointMessageProperty.Address,
                    rempEndpointMessageProperty.Port, callback, false);
                OperationContext.Current.Channel.Closing += (sender, e) =>
                {
                    SubscriberCollection.Default.TakeWhile(s => s.Mac == mac);
                    
                };

                SubscriberCollection.Default.Add(subscriber);
            }

            callback.OnlineStateChanged(mac, OnlineState.Online);
        }

        public void Offline(string mac)
        {
            var remote =
                OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as
                    RemoteEndpointMessageProperty;

            IDuplexChannelCallback callback = OperationContext.Current.GetCallbackChannel<IDuplexChannelCallback>();

            SubscriberCollection.Default.TakeWhile(s => s.Mac == mac);
            callback.OnlineStateChanged(mac, OnlineState.Offline);
        }

        public void Broadcast(string msg)
        {
            SubscriberCollection.Default.Boardcast(msg);
        }

        public void Broadcast(string clientMac, string msg)
        {
            SubscriberCollection.Default.Boardcast(clientMac, msg);
        }

        public void Broadcast(IEnumerable<string> clientMacs, string msg)
        {
            SubscriberCollection.Default.Boardcast(clientMacs, msg);
        }

        public void GetClients()
        {
            var lst = from n in SubscriberCollection.Default.Subscribers select n.Mac;
            var callback = OperationContext.Current.GetCallbackChannel<IDuplexChannelCallback>();
            callback.ReturnClients(lst);
        }
    }
}
