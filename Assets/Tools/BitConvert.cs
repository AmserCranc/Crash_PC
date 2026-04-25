using UnityEngine;
unsafe static public class ConvertBits
{
    public static int FromInt32(ref byte[] str, int offset, int streamSize)
    {
        if (str == null)
            Debug.LogError("int32 is null");
        if (offset < 0)
            Debug.LogError("offset too low");
        if (offset + 4 > streamSize)
            Debug.LogError("out of range");
            
        int result = 0;
        result |= str[offset + 0] << 8 * 0;
        result |= str[offset + 1] << 8 * 1;
        result |= str[offset + 2] << 8 * 2;
        result |= str[offset + 3] << 8 * 3;
        return result;
    }
}
