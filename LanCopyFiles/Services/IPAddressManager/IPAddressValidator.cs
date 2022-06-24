using System;
using System.Linq;
using System.Net.Sockets;
using log4net;

namespace LanCopyFiles.Services.IPAddressManager;

public class IPAddressValidator
{
    // public static bool ValidateIPv4(string ipAddressString)
    // {
    //     if (CheckIfValidFormatIPv4Only(ipAddressString))
    //     {
    //         return false;
    //     }
    //
    //     string[] splitValues = ipAddressString.Split('.');
    //     if (splitValues.Length != 4)
    //     {
    //         return false;
    //     }
    //
    //     byte tempForParsing;
    //
    //     return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    // }

    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static bool CheckIfValidFormatIPv4Only(string ipAddressString)
    {
        // Kiem tra dia chi IP
        if (string.IsNullOrEmpty(ipAddressString.Trim()))
        {
            return false;
        }

        var parts = ipAddressString.Split('.');
        bool isValid = parts.Length == 4
                       && !parts.Any(
                           x =>
                           {
                               int y;
                               return Int32.TryParse((string?)x, out y) && y > 255 || y < 1;
                           });
        return isValid;
    }

    private static bool PingHost(string hostUri, int portNumber)
    {
        // Cho phep may host ping may VM trong Hyper-V: https://serverfault.com/a/623254

        // Nguon: https://stackoverflow.com/a/22903941/7182661
        try
        {
            using (var client = new TcpClient(hostUri, portNumber))
                return true;
        }
        catch (SocketException ex)
        {
            Log.Error("Error pinging host:'" + hostUri + ":" + portNumber.ToString() + "'");
            return false;
        }
    }

    public static bool TestConnectionUsingPingHost(string destinationPCIPAddress)
    {
        return CheckIfValidFormatIPv4Only(destinationPCIPAddress) && PingHost(destinationPCIPAddress, APP_DEFAULT_PORT);
    }

    public static readonly int APP_DEFAULT_PORT = 8085;
}