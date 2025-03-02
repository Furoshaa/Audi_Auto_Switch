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
        private MMDeviceEnumerator deviceEnumerator;
        private MMDeviceNotificationClient notificationClient;

        public Service1()
        {
            InitializeComponent();
            deviceEnumerator = new MMDeviceEnumerator();
            notificationClient = new MMDeviceNotificationClient(SynchronizeAudioDevices);
        }

        protected override void OnStart(string[] args)
        {
            deviceEnumerator.RegisterEndpointNotificationCallback(notificationClient);
            // Synchroniser une première fois au démarrage
            SynchronizeAudioDevices();
        }

        protected override void OnStop()
        {
            deviceEnumerator.UnregisterEndpointNotificationCallback(notificationClient);
            deviceEnumerator?.Dispose();
        }

        private void SynchronizeAudioDevices()
        {
            // Même code que précédemment, mais sans le paramètre object state
            try
            {
                var defaultPlaybackDevice = deviceEnumerator.GetDefaultAudioEndpoint(
                    DataFlow.Render, Role.Multimedia);
                var currentCommunicationDevice = deviceEnumerator.GetDefaultAudioEndpoint(
                    DataFlow.Render, Role.Communications);

                if (defaultPlaybackDevice.ID != currentCommunicationDevice.ID)
                {
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

    internal class PolicyConfigClient : IDisposable
    {
        private static readonly Guid CLSID_PolicyConfig = Guid.Parse("870af99c-171d-4f9e-af0d-e63df40c2bc9");
        private readonly IPolicyConfig _policyConfig;
        private bool _disposed = false;

        public PolicyConfigClient()
        {
            _policyConfig = (IPolicyConfig)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_PolicyConfig));
        }

        public void SetDefaultEndpoint(string deviceId, Role role)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PolicyConfigClient));
                
            _policyConfig.SetDefaultEndpoint(deviceId, role);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_policyConfig != null && Marshal.IsComObject(_policyConfig))
                {
                    Marshal.ReleaseComObject(_policyConfig);
                }
                _disposed = true;
            }
        }
    }

    internal class MMDeviceNotificationClient : NAudio.CoreAudioApi.Interfaces.IMMNotificationClient
    {
        private readonly Action _onDeviceChanged;

        public MMDeviceNotificationClient(Action onDeviceChanged)
        {
            _onDeviceChanged = onDeviceChanged;
        }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == DataFlow.Render && role == Role.Multimedia)
            {
                _onDeviceChanged();
            }
        }

        public void OnDeviceAdded(string pwstrDeviceId) { }
        public void OnDeviceRemoved(string deviceId) { }
        public void OnDeviceStateChanged(string deviceId, DeviceState newState) { }
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }
    }
}