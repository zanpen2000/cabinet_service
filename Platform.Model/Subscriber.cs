using Platform.Model.Interfaces;
using Platform.Service.Contracts;

namespace Platform.Model
{
    public class Subscriber : ISubscriber
    {
        public bool IsManager { get; private set; }
        public string Mac { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public string Name { get; private set; }


        public Subscriber(string name, string cMac, string cIp, int cPort,IDuplexChannelCallback callback, bool isManager = false)
        {
            Name = name; Mac = cMac; IP = cIp; Port = cPort; IsManager = isManager;
            Callback = callback;
        }


        public void Notify(string msg)
        {
            Callback.NotifyMessage(msg);
        }

        public IDuplexChannelCallback Callback { get; private set; }
    }
}
