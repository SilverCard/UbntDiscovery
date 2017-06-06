using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public MainWindowModel(MainWindow mainWindow)
        {
            Devices = new ObservableCollection<Device>();
            MainWindow = mainWindow;
            DeviceDiscovery = new DeviceDiscovery(this.AddDevice);
        }  

        public void Scan()
        {
          

            try
            {
                if (!DeviceDiscovery.IsScanning)
                {           
                    DeviceDiscovery.BeginDiscoverDevices();
                }
                else
                {
                    DeviceDiscovery.EndDiscoverDevices();
                    Thread.Sleep(100);
                    Devices.Clear();
                    DeviceDiscovery.BeginDiscoverDevices();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void ClearDevices()
        {
            DeviceDiscovery.EndDiscoverDevices();
            Devices.Clear();
        }

        public void AddDevice(Device device)
        {
            this.MainWindow.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
            () =>
            {
                Devices.Add(device);

            }));
        }




    }
}
