using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavebankChunk : Chunk
{

    private byte[] raw;

    public WavebankChunk(Chunk _chunk) : base(_chunk.data)
    {
        raw = _chunk.data;

    }
}
