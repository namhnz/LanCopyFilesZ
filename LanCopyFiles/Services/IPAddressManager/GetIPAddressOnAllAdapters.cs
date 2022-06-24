using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace LanCopyFiles.Services.IPAddressManager;

public class GetIPAddressOnAllAdapters
{
    public static IEnumerable<string> GetAllIPv4()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                Console.WriteLine(ni.Name);
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        yield return ip.Address.ToString();
                    }
                }
            }
        }
    }
}