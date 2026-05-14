using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChunk : Chunk
{
    const int
        pEID    = 0x4;

    public byte[] rawData;

    public int eid  => ConvertBits.FromInt32(rawData, pEID);
    public string EIDname => EIDToEName(eid);

    public TextureChunk(Chunk _chunk) : base(_chunk.data)
    {
        rawData = _chunk.data;
    }

    public static string EIDToEName(int eid)
    {
        const string ENameCharacterSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_!";
        char[] str = new char[5];
        eid >>= 1;
        for (int i = 0; i < 5; i++)
        {
            str[4 - i] = ENameCharacterSet[eid & 0x3F];
            eid >>= 6;
        }
        return new string(str);
    }
}
