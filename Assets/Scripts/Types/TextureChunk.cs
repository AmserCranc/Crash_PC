using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChunk : Chunk
{
    const int
        pEID    = 0x4;

    private byte[] rawData;

    public int eid  => ConvertBits.FromInt32(rawData, pEID);

    public TextureChunk(Chunk _chunk) : base(_chunk.data)
    {
        rawData = _chunk.data;
    }
}
