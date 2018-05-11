using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UbntDiscovery
{
    public class DeviceDiscovery
    {
        public delegate void DeviceDiscoveredEventHandler(Device device);

        public event DeviceDiscoveredEventHandler DeviceDiscovered;

        protected virtual void OnDeviceDiscovered(Device e)
        {
            if (DeviceDiscovered != null)
                DeviceDiscovered(e);
        }

        public Boolean IsScanning { get; set; } 

        /// <summary>
        /// Endpoint to broadcast
        /// </summary>
        private IPEndPoint _EP2;


        /// <summary>
        /// Packet data to broadcast
        /// </summary>
        private readonly byte[] _Datagram = { 1, 0, 0, 0 };

        public DeviceDiscovery()
        {
            IsScanning = false;
            _EP2 = new IPEndPoint(IPAddress.Broadcast, 10001);
        }

        public async Task DiscoveryAsync(CancellationToken ct, IPAddress interfaceAddress)
        {
            IPEndPoint interfaceEndpoint = new IPEndPoint(interfaceAddress, 2048);
            var udpClient = new BroadcastUdpClient(interfaceEndpoint);
            //udpClient.EnableBroadcast = true;
            await udpClient.SendAsync(_Datagram, _Datagram.Length, _EP2);
            udpClient.EnableBroadcast = false;
            ct.Register(() => udpClient.Close() );
       
            HashSet<ulong> packetHash = new HashSet<ulong>();

            while (!ct.IsCancellationRequested)
            {
                UdpReceiveResult receiveResult;

                try
                {
                    receiveResult = await udpClient.ReceiveAsync();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                DiscoveryPacket dpack = new DiscoveryPacket(receiveResult.Buffer);
                Device device = dpack.DecodePacket();

                Boolean result = packetHash.Add(Utils.CalculateHash(receiveResult.Buffer));

                if (result)
                {
                    OnDeviceDiscovered(device);
                }
            }

            udpClient.Close();
            packetHash.Clear();

        }

        //public void BeginDiscoverDevices()
        //{
        //    IsScanning = true;
        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    DiscoveryTask(cts.Token);
        //}

        //public void EndDiscoverDevices()
        //{
        //    if (!IsScanning) return;
        //    IsScanning = false;
        //}
    }
}
