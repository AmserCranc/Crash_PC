using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChunk : Chunk
{
    public NormalChunk(byte[] _data) : base(_data)
    {
        byte[] raw = _data;

        int offset = 0;
        while(offset < raw.Length)
        {
            
            





            //byte[] chunkData = Array.Copy(raw, offset, )
        }
        
        
    }
}
