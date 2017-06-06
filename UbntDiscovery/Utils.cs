using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace UbntDiscovery
{
    public static class Utils
    {
        public static UInt16 UInt16FromBytes(byte[] bytes, int index)
        {
            return (UInt16)(bytes[index] << 8 | bytes[index + 1]);
        }

        public static Int32 Int32FromBytes(byte[] bytes, int index)
        {
            byte[] intbytes = new byte[4];
            Array.Copy(bytes, index, intbytes, 0, intbytes.Length);
            Array.Reverse(intbytes);
            return BitConverter.ToInt32(intbytes, 0);
        }

        public static String FormatMacAddress(PhysicalAddress macAddress)
        {
            String str = String.Empty;

            if (macAddress != null)
            {
                str = BitConverter.ToString(macAddress.GetAddressBytes()).Replace('-', ':');
            }

            return str;
        }

        public static ulong CalculateHash(byte[] bytes)
        {
            byte[] hb;

            using(MD5 md5 = MD5.Create())
            {
                hb = md5.ComputeHash(bytes); 
            }

            return BitConverter.ToUInt64(hb, 0);
        }
    }
}
