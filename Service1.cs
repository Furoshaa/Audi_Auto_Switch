using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using NAudio.CoreAudioApi;

namespace Audi_Auto_Switch
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private MMDeviceEnumerator deviceEnumerator;

        public Service1()
        {
            InitializeComponent();
            deviceEnumerator = new MMDeviceEnumerator();
        }

        protected override void OnStart(string[] args)
        {
            // Vérifier toutes les 2 secondes
            timer = new Timer(SynchronizeAudioDevices, null, 0, 2000);
        }

        protected override void OnStop()
        {
            timer?.Dispose();
            deviceEnumerator?.Dispose();
        }

        private void SynchronizeAudioDevices(object state)
        {
            try
            {
                // Obtenir le périphérique de lecture par défaut
                var defaultPlaybackDevice = deviceEnumerator.GetDefaultAudioEndpoint(
                    DataFlow.Render, Role.Multimedia);

                // Obtenir le périphérique de communication actuel
                var currentCommunicationDevice = deviceEnumerator.GetDefaultAudioEndpoint(
                    DataFlow.Render, Role.Communications);

                // Si les périphériques sont différents, synchroniser
                if (defaultPlaybackDevice.ID != currentCommunicationDevice.ID)
                {
                    // Définir le périphérique de communication par défaut
                    using (var policyConfig = new PolicyConfigClient())
                    {
                        policyConfig.SetDefaultEndpoint(defaultPlaybackDevice.ID, Role.Communications);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Audio Device Sync Service",
                    $"Erreur lors de la synchronisation: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }
    }

    // Interface pour la configuration des périphériques audio
    [Guid("870af99c-171d-4f9e-af0d-e63df40c2bc9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPolicyConfig
    {
        int SetDefaultEndpoint(string deviceId, Role role);
    }

    internal class PolicyConfigClient
    {
        private static readonly Guid CLSID_PolicyConfig = Guid.Parse("870af99c-171d-4f9e-af0d-e63df40c2bc9");
        private readonly IPolicyConfig _policyConfig;

        public PolicyConfigClient()
        {
            _policyConfig = (IPolicyConfig)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_PolicyConfig));
        }

        public void SetDefaultEndpoint(string deviceId, Role role)
        {
            _policyConfig.SetDefaultEndpoint(deviceId, role);
        }
    }
}