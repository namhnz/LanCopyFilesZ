using System;
using System.Net;
using System.Net.Sockets;

namespace LanCopyFiles.Services.IPAddressManager;

public class GetCurrentConnectionIPAddress
{
    public static string GetIPv4()
    {
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.1.20", 1337); // doesnt matter what it connects to
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString(); //ipv4
            }
        }
        catch (Exception)
        {
            return "Failed when getting current connection IP Address"; // If no connection is found
        }
    }
}