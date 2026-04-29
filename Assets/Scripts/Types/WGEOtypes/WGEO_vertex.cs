using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGEO_vertex
{
    private byte[] rawXY;
    private byte[] rawZ;

    public short x        => (short)(ConvertBits.FromInt32(rawXY, 0)       >> 4);
    public short y        => (short)(ConvertBits.FromInt32(rawXY, 0) >> 16 >> 4);
    public short z        => (short)(ConvertBits.FromInt16(rawZ, 0)        >> 4);
    public int unkX       => x & 0xF;
    public int unkY       => y & 0xF;
    public int unkZ       => z & 0xF;




    public WGEO_vertex(byte[] _dataXY, byte[] _dataZ)
    {
        rawXY = _dataXY;
        rawZ  = _dataZ;
    }

}
