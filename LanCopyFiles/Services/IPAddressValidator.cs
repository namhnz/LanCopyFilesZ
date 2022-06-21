using System;
using System.Linq;

namespace LanCopyFiles.Services;

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
                               return Int32.TryParse(x, out y) && y > 255 || y < 1;
                           });
        return isValid;
    }

    public static readonly int APP_DEFAULT_PORT = 8085;
}