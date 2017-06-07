using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UbntDiscovery
{
    public class MainWindowModel
    {
        public ObservableCollection<Device> Devices { get; private set; }
        public DeviceDiscovery DeviceDiscovery { get; private set; }
        public MainWindow MainWindow { get; private set; }
        private CancellationTokenSource _cts;
        private Task discoveryTask;

        public MainWindowModel(MainWindow mainWindow)
        {
            Devices = new ObservableCollection<Device>();
            MainWindow = mainWindow;
            DeviceDiscovery = new DeviceDiscovery();
            DeviceDiscovery.DeviceDiscovered += DeviceDiscovery_DeviceDiscovered;        
        }

        private void DeviceDiscovery_DeviceDiscovered(Device device)
        {
            AddDevice(device);
        }

        public async Task ScanAsync()
        {
            if (discoveryTask != null && (discoveryTask.Status == TaskStatus.WaitingForActivation || discoveryTask.Status == TaskStatus.Running) )
            {
                _cts.Cancel();
                Devices.Clear();
            }

            try
            {
                var cts = new CancellationTokenSource();
                _cts = cts;
                discoveryTask = DeviceDiscovery.DiscoveryAsync(_cts.Token).ContinueWith((t) => cts.Dispose());

                await discoveryTask;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        public void ClearDevices()
        {
            if (_cts != null) _cts.Cancel();
            Devices.Clear();
        }

        public void AddDevice(Device device)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                Devices.Add(device);

            });
        }




    }
}
