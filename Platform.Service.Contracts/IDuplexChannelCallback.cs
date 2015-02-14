using System.Collections.Generic;
using System.ServiceModel;

namespace Platform.Service.Contracts
{
    public interface IDuplexChannelCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnlineStateChanged(string mac, OnlineState state);

        [OperationContract(IsOneWay = true)]
        void NotifyMessage(string msg);

        [OperationContract(IsOneWay = true)]
        void ReturnClients(IEnumerable<string> clientMacs);
    }
}
