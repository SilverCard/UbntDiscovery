using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace UbntDiscovery
{
    public class DeviceDiscovery
    {
        public Action<Device> DiscoveryCallback { get; set; }
        public Boolean IsScanning { get; set; }

        private UdpClient _UdpClient;

        /// <summary>
        /// Endpoint to listen
        /// </summary>
        private IPEndPoint _EP1;

        /// <summary>
        /// Endpoint to broadcast
        /// </summary>
        private IPEndPoint _EP2;

        private HashSet<ulong> _PacketHash;

        /// <summary>
        /// Packet data to broadcast
        /// </summary>
        private readonly byte[] _Datagram = { 1, 0, 0, 0 };

        public DeviceDiscovery()
        {
            Init();
        }

        public DeviceDiscovery(Action<Device> discoveryCallback)
        {
            Init();
            DiscoveryCallback = discoveryCallback;            
        }

        private void Init()
        {
            DiscoveryCallback = null;
            IsScanning = false;
            _PacketHash = new HashSet<ulong>();
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            if (!IsScanning) return;

            Byte[] receiveBytes = _UdpClient.EndReceive(ar, ref _EP1);

            DiscoveryPacket dpack = new DiscoveryPacket(receiveBytes);
            Device device = dpack.DecodePacket();

            Boolean result = _PacketHash.Add(Utils.CalculateHash(receiveBytes));

            if (DiscoveryCallback != null && result)
            {
                DiscoveryCallback(device);
            }

            // ReceiveNext
            ReceivePacket();
        }

        public void ReceivePacket()
        {
            _UdpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
        }

        public void BeginDiscoverDevices()
        {

            _EP1 = new IPEndPoint(IPAddress.Any, 2048);
            _EP2 = new IPEndPoint(IPAddress.Broadcast, 10001);
            _UdpClient = new UdpClient(_EP1);
            _UdpClient.EnableBroadcast = true;
            _UdpClient.Send(_Datagram, _Datagram.Length, _EP2);
            _UdpClient.EnableBroadcast = false;

            ReceivePacket();
            IsScanning = true;
        }

        public void EndDiscoverDevices()
        {
            if (!IsScanning) return;

            IsScanning = false;

            _PacketHash.Clear();
            _UdpClient.Close();            

        }
    }
}
