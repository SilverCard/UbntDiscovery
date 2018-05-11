using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UbntDiscovery
{
    public class MainWindowModel
    {
        public ObservableCollection <Device> Devices { get; private set; }
        public DeviceDiscovery DeviceDiscovery { get; private set; }
        public MainWindow MainWindow { get; private set; }
        private CancellationTokenSource _cts;

        public MainWindowModel(MainWindow mainWindow)
        {
            Devices = new ObservableCollection<Device>();
            MainWindow = mainWindow;
            DeviceDiscovery = new DeviceDiscovery();
            DeviceDiscovery.DeviceDiscovered += DeviceDiscovery_DeviceDiscovered;
            _cts = new CancellationTokenSource();
        }

        private void DeviceDiscovery_DeviceDiscovered(Device device)
        {
            AddDevice(device);
        }

        public async Task ScanAsync()
        {
            _cts.Cancel();
            Devices.Clear();

            try
            {
                var cts = new CancellationTokenSource();
                _cts = cts;

                String strHostName = Dns.GetHostName();
                // Find host by name
                IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

                // Enumerate IP addresses
                List<Task> tasks = new List<Task>();
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Task discoveryTask = DeviceDiscovery.DiscoveryAsync(cts.Token, ipaddress);
                        tasks.Add(discoveryTask);
                    }
                }
                foreach (Task discoveryTask in tasks)
                {
                    await discoveryTask;
                }
                cts.Dispose();
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
                foreach (Device existingDevice in Devices)
                {
                    if (existingDevice.Equals(device))
                    {
                        return;
                    }
                }
                Devices.Add(device);
            });
        }
    }
}
