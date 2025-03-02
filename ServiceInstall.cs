using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Audi_Auto_Switch
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "AudioAutoSwitch",
                DisplayName = "Audio Auto Switch Service",
                Description = "Synchronise le périphérique de communication avec le périphérique multimédia par défaut",
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}