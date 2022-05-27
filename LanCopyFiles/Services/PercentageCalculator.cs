using System;

namespace LanCopyFiles.Services;

public class PercentageCalculator
{
    // Nguon: https://stackoverflow.com/a/6897213/7182661

    public static int DivideRoundingUp(int x, int y)
    {
        // TODO: Define behaviour for negative numbers
        int remainder;
        int quotient = Math.DivRem(x, y, out remainder);
        return remainder == 0 ? quotient : quotient + 1;
    }
}