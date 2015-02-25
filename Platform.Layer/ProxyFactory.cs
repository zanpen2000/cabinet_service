using System;
using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Platform.Layer
{
    public class ProxyFactory : IDisposable
    {
        private static readonly object SyncLock = new object();
        private static readonly Hashtable Channels = new Hashtable();

        public void Dispose()
        {
            var iter = Channels.GetEnumerator();
            while (iter.MoveNext())
            {
                ((ICommunicationObject)iter.Value).Abort();
            }
        }

        public static TSvc GetProxy<TSvc, TCallback>(TCallback callback)
        {
            var context = new InstanceContext(callback);
            var factory = new DuplexChannelFactory<TSvc>(context, __getCustomBinding());


            return (TSvc)__getChannel(factory);
        }

        public static TSvc GetProxy<TSvc, TCallback>(TCallback callback, CustomBinding binding)
        {
            var context = new InstanceContext(callback);
            var factory = new DuplexChannelFactory<TSvc>(context, binding);
            return (TSvc)__getChannel(factory);
        }

        public static TSvc GetProxy<TSvc>()
        {
            var factory = new ChannelFactory<TSvc>(__getCustomBinding());
            return (TSvc)__getChannel(factory);
        }

        private static ICommunicationObject __getChannel<TSvc>(IChannelFactory<TSvc> factory)
        {
            lock (SyncLock)
            {
                var obj = (ICommunicationObject)factory.CreateChannel(new EndpointAddress(__getAddress<TSvc>()));
                if (Channels.ContainsKey(typeof(TSvc)))
                {
                    if (!Channels[typeof(TSvc)].Equals(obj))
                        Channels[typeof(TSvc)] = obj;
                    return (ICommunicationObject)Channels[typeof(TSvc)];
                }
                Channels.Add(typeof(TSvc), obj);
                return obj;
            }
        }

        private static string __getAddress<TSvc>()
        {
            var baseAddr = AppSettings.Get("tcpServiceAddress");

            if (string.IsNullOrEmpty(baseAddr))
            {
                throw new ArgumentNullException("baseAddr");
            }

            var svrName = typeof(TSvc).ToString().Split('.')[3].Substring(1);

            if (string.IsNullOrEmpty(svrName))
            {
                throw new ArgumentNullException("svrName");
            }

            return "net.tcp://" + baseAddr + "/" + svrName;
        }

        private static CustomBinding __getCustomBinding()
        {
            var binding = new CustomBinding
            {
                CloseTimeout = new TimeSpan(0, 30, 0),
                SendTimeout = new TimeSpan(0, 1, 0),
                ReceiveTimeout = new TimeSpan(0, 30, 0),
                OpenTimeout = new TimeSpan(0, 1, 0)
            };

            binding.Elements.Add(new TransactionFlowBindingElement(TransactionProtocol.Default));
            binding.Elements.Add(new BinaryMessageEncodingBindingElement());

            var reliable = new ReliableSessionBindingElement(true) { MaxPendingChannels = 20 };
            binding.Elements.Add(reliable);

            var tcp = new TcpTransportBindingElement
            {
                ListenBacklog = 400,
                MaxPendingConnections = 1000,
                MaxPendingAccepts = 10
            };

            binding.Elements.Add(tcp);

            return binding;
        }

        public static CustomBinding DefaultBinding()
        {
            return __getCustomBinding();
        }
    }
}
