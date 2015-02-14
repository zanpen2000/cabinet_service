using System.Collections.Generic;
using System.ServiceModel;

namespace Platform.Service.Contracts
{
    [ServiceContract(CallbackContract = typeof(IDuplexChannelCallback))]
    public interface IDuplexChannelService
    {
        /// <summary>
        /// 订阅者订阅
        /// </summary>
        /// <param name="mac"></param>
        [OperationContract(IsOneWay = true)]
        void Online(string name, string mac);

        /// <summary>
        /// 订阅者取消订阅
        /// </summary>
        /// <param name="mac"></param>
        [OperationContract(IsOneWay = true)]
        void Offline(string mac);

        /// <summary>
        /// 广播到所有在线客户端
        /// </summary>
        /// <param name="msg"></param>
        [OperationContract(Name = "BroadcastAllClient", IsOneWay = true)]
        void Broadcast(string msg);

        /// <summary>
        /// 广播到指定客户端
        /// </summary>
        /// <param name="clientMac"></param>
        /// <param name="msg"></param>

        [OperationContract(Name = "BroadcastToClient", IsOneWay = true)]
        void Broadcast(string clientMac, string msg);


        /// <summary>
        /// 广播到指定客户端
        /// </summary>
        /// <param name="clientMacs"></param>
        /// <param name="msg"></param>
        [OperationContract(Name = "BroadcastToClients", IsOneWay = true)]
        void Broadcast(IEnumerable<string> clientMacs, string msg);

        /// <summary>
        /// 获取所有订阅者
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void GetClients();
    }
}
