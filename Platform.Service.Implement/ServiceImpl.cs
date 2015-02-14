using System.ServiceModel;

namespace Platform.Service.Implement
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class ServiceImpl
    {
    }
}
