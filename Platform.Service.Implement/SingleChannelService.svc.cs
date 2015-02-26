using Platform.Service.Contracts;

namespace Platform.Service.Implement
{
    public partial class ServiceImpl : ISingleChannelService
    {
        public byte Heartbeat(byte b)
        {
            return SingleChannelService.Default.Heartbeat(b);
        }


        System.Collections.Generic.List<string> ISingleChannelService.GetClients()
        {
            return SingleChannelService.Default.GetClients();
        }


        public bool Exists(string mac)
        {
            return SingleChannelService.Default.Exists(mac);
        }
    }
}