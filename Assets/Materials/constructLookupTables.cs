using UnityEngine;

public class PixelConv : MonoBehaviour
{
    internal static byte[] colorTable;

    internal static byte[] colorTableInverse;

    void Start()
    {
        colorTable = new byte[256];
        colorTableInverse = new byte[32];
        for (int i = 0; i < 256; i++)
        {
            colorTable[i] = (byte)(i * 31 / 255);
        }

        for (int j = 0; j < 32; j++)
        {
            for (int k = 0; k < 256; k++)
            {
                if (colorTable[k] == j)
                {
                    colorTableInverse[j] = (byte)k;
                    break;
                }
            }
        }

        string output = null;

        foreach(byte b in colorTableInverse)
        {
            output = output + $",{b} ";
        }
        Debug.Log(output);
    }


    public static int Convert5551_8888(short p, int mode)
    {
        byte b  = colorTableInverse[ p        & 0x1F];
        byte b2 = colorTableInverse[(p >> 5)  & 0x1F];
        byte b3 = colorTableInverse[(p >> 10) & 0x1F];
        byte b4 = (byte)((p >> 15) & 1);
        switch (mode)
        {
            case 0:
                b4 = (byte)((b4 == 1) ? 127 : ((b + b2 + b3 != 0) ? byte.MaxValue : 0));
                break;
            case 1:
                b4 = (byte)((b4 == 1) ? byte.MaxValue : ((b + b2 + b3 != 0) ? byte.MaxValue : 0));
                break;
            case 2:
                b4 = (byte)((b4 == 1) ? byte.MaxValue : ((b + b2 + b3 != 0) ? byte.MaxValue : 0));
                break;
            case 3:
                b4 = (byte)((b4 == 1) ? byte.MaxValue : ((b + b2 + b3 != 0) ? byte.MaxValue : 0));
                break;
        }

        return (b4 << 24) | (b << 16) | (b2 << 8) | b3;
    }
}