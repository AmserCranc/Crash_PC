using UnityEngine;
unsafe static public class ConvertBits
{
    public static int FromInt32(ref byte[] bits, int offset)
    {
        if (bits == null)
            Debug.LogError("int32 is null");
        if (offset < 0)
            Debug.LogError("offset too low");
        if (offset + 4 > bits.Length)
            Debug.LogError("out of range");
            
        int result = 0;
        result |= bits[offset + 0] << 8 * 0;
        result |= bits[offset + 1] << 8 * 1;
        result |= bits[offset + 2] << 8 * 2;
        result |= bits[offset + 3] << 8 * 3;

        return result;
    }

    public static short FromInt16(ref byte[] bits, int offset)
    {
        if (bits == null)
            Debug.LogError("int32 is null");
        if (offset < 0)
            Debug.LogError("offset too low");
        if (offset + 2 > bits.Length)
            Debug.LogError("out of range");
            
        int result = 0;
        result |= bits[offset + 0] << 8 * 0;
        result |= bits[offset + 1] << 8 * 1;

        return (short)result;
    }
}
