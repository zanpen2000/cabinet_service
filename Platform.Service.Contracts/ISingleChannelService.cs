using System.Collections.Generic;
using System.ServiceModel;

namespace Platform.Service.Contracts
{

    [ServiceContract]
    public interface ISingleChannelService
    {
        [OperationContract]
        byte Heartbeat(byte b);


        [OperationContract]
        List<string> GetClients();
    }
}
