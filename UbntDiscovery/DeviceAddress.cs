using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace UbntDiscovery
{
    public class DeviceAddress
    {
        public IPAddress IpAddress { get; set; }
        public PhysicalAddress MacAddress { get; set; }

        public DeviceAddress(IPAddress ipAddress, PhysicalAddress macAddress )
        {
            IpAddress = ipAddress;
            MacAddress = macAddress;
        }

        public override string ToString()
        {
            return IpAddress.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is DeviceAddress)
            {
                DeviceAddress b = obj as DeviceAddress;
                return this.IpAddress.Equals(b.IpAddress) && this.MacAddress.Equals(b.MacAddress);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
