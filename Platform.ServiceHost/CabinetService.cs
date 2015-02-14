using System;
using System.ServiceProcess;
using Logger;
using Platform.Model;
using Platform.Model.Interfaces;
using Platform.Service.Contracts;
using Platform.Service.Implement;

namespace Platform.ServiceHost
{
    public class CabinetService : ServiceBase
    {
        private System.ServiceModel.ServiceHost serviceHost = null;

        public CabinetService()
        {
            ServiceName = "TSDYKJ_CabinetService";
        }

        public static void Main()
        {
            Run(new CabinetService());
        }

        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            serviceHost = new System.ServiceModel.ServiceHost(typeof(ServiceImpl));

            serviceHost.Opened += serviceHost_Opened;
            serviceHost.Closed += serviceHost_Closed;
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Open();
        }

        void serviceHost_Closing(object sender, EventArgs e)
        {
            Log.AppendInfo(ServiceName + " 正在停止...");
        }

        void serviceHost_Opening(object sender, EventArgs e)
        {
            Log.AppendInfo(ServiceName + " 正在启动...");
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            Log.AppendInfo(ServiceName + " 已停止");
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            Log.AppendInfo(ServiceName + " 已启动");
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
