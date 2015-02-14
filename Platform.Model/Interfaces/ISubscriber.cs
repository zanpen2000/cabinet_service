using Platform.Service.Contracts;

namespace Platform.Model.Interfaces
{
    /// <summary>
    /// 订阅者
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// 是否作为管理者
        /// </summary>
        bool IsManager { get; }
        /// <summary>
        /// Mac地址
        /// </summary>
        string Mac { get; }
        /// <summary>
        /// ip地址
        /// </summary>
        string IP { get; }
        /// <summary>
        /// 端口
        /// </summary>
        int Port { get; }
        /// <summary>
        /// 名字（客户端描述）
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="msg"></param>
        void Notify(string msg);

        IDuplexChannelCallback Callback { get; }
    }
}
