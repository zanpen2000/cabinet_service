using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Platform.ServiceHost
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var process = new ServiceProcessInstaller {Account = ServiceAccount.LocalSystem};
            var service = new ServiceInstaller {ServiceName = "TSDYKJ_CabinetService", Description = "唐山市达意科技快递业务服务"};
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
