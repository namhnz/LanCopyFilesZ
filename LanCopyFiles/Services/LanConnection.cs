using System.Net.Sockets;

namespace LanCopyFiles.Services;

public class LanConnection
{
    // Cho phep may host ping may VM trong Hyper-V: https://serverfault.com/a/623254

    // Nguon: https://stackoverflow.com/a/22903941/7182661
    public static bool PingHost(string hostUri, int portNumber)
    {
        try
        {
            using (var client = new TcpClient(hostUri, portNumber))
                return true;
        }
        catch (SocketException ex)
        {
            // MessageBox.Show("Error pinging host:'" + hostUri + ":" + portNumber.ToString() + "'");
            return false;
        }
    }
}