using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChunk : Chunk
{
    public List<Entry> entries;

    public NormalChunk(byte[] _data) : base(_data)
    {
        byte[] raw = _data;

        entries = new();
        int offset = 0;
        while(offset < raw.Length)
        {
            var entry = Entry.ClassifyRaw(raw, offset).Invoke(raw, offset);
            offset += entry.length;





            //byte[] chunkData = Array.Copy(raw, offset, )
        }
        
        
    }
}

