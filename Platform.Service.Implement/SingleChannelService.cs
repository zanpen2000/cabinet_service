using Platform.Service.Contracts;

namespace Platform.Service.Implement
{
    internal class SingleChannelService : ISingleChannelService
    {
        private static readonly object SyncLock = new object();
        private static SingleChannelService _instance;

        public static SingleChannelService Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SingleChannelService();
                        }
                    }
                }
                return _instance;
            }
        }

        private SingleChannelService() { }

        public byte Heartbeat(byte b)
        {
            return b;
        }
    }
}
