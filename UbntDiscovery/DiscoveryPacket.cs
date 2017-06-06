using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace UbntDiscovery
{
    public class DiscoveryPacket
    {
        public byte[] PacketData { get; private set; }
        public byte Version { get; private set; }


        public ushort Length { get; private set; }
        private int _Index;
        private ushort _ValueLength;

        public DiscoveryPacket(byte[] data)
        {
            PacketData = data;
            Version = PacketData[0];

            Length = Utils.UInt16FromBytes(PacketData, 2);

            if (Length + 4 != PacketData.Length)
            {
                throw new Exception("Invalid packet size.");
            }
        }

        private String DecodeString()
        {
            return Encoding.UTF8.GetString(PacketData, _Index, _ValueLength);
        }

        private DeviceAddress DecodeAddresses()
        {
            if (_ValueLength != 10)
            {
                throw new Exception("Invalid length.");
            }

            byte[] ipbytes = new byte[4];
            byte[] macbytes = new byte[6];

            Array.Copy(PacketData, _Index, macbytes, 0, macbytes.Length);
            Array.Copy(PacketData, _Index + macbytes.Length, ipbytes, 0, ipbytes.Length);

            return new DeviceAddress(new IPAddress(ipbytes), new PhysicalAddress(macbytes));
        }

        private TimeSpan DecodeTime()
        {
            if (_ValueLength != 4)
            {
                throw new Exception("Invalid length.");
            }

            int seconds = Utils.Int32FromBytes(PacketData, _Index);
            return new TimeSpan(0, 0, seconds);
        }

        public Device DecodePacket()
        {
            Device device = new Device();

            _Index = 4;

            //byte uk1 = packet[1];

            while (_Index < PacketData.Length)
            {
                byte i = PacketData[_Index];
                _ValueLength = Utils.UInt16FromBytes(PacketData, _Index + 1);
                _Index += 3;

                switch (i)
                {
                    case 1: //Binary Data
                        break;

                    case 2: //Address 
                        device.Addresses.Add(DecodeAddresses());
                        break;
                    
                    case 3: //Firmware Version
                        device.Firmware = DecodeString();
                        break;

                    case 10: // Uptime
                        device.Uptime = DecodeTime();
                        break;

                    case 11: // Hostname
                        device.Hostname = DecodeString();
                        break;

                    case 12: // Platform
                        device.Platform = DecodeString();
                        break;
                    
                    case 13: // SSID
                        device.SSID = DecodeString();
                        break;

                    case 14: // Wireless Mode  
                        if (_ValueLength != 1)
                            throw new Exception("Invalid Length for Wireless Mode.");

                        device.WirelessMode = PacketData[_Index];
                        break;

                    // Binary Data
                    case 16:
                        break;

                    default:
                        break;
                }

                _Index += _ValueLength;

            }

            return device;
        }
    }
}
