using System.Collections.Generic;
using Platform.Service.Contracts;

namespace Platform.Service.Implement
{
    public partial class ServiceImpl : IDuplexChannelService
    {
        public void Online(string name, string mac)
        {
            DuplexChannelService.Default.Online(name, mac);
        }

        public void Offline(string mac)
        {
            DuplexChannelService.Default.Offline(mac);
        }

        public void Broadcast(string msg)
        {
            DuplexChannelService.Default.Broadcast(msg);
        }

        public void Broadcast(string clientMac, string msg)
        {
            DuplexChannelService.Default.Broadcast(clientMac, msg);
        }

        public void Broadcast(IEnumerable<string> clientMacs, string msg)
        {
            DuplexChannelService.Default.Broadcast(clientMacs, msg);
        }

        public void GetClients()
        {
            DuplexChannelService.Default.GetClients();
        }

    }
}
