using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGEO_quad
{
    private byte[] raw;
//                   Extract word                     | Extract vertex
    public int v1 => (ConvertBits.FromInt32(raw, 0) >> 8 ) & 0xFFF;
    public int v2 => (ConvertBits.FromInt32(raw, 0) >> 20) & 0xFFF;
    public int v3 => (ConvertBits.FromInt32(raw, 4) >> 8 ) & 0xFFF;
    public int v4 => (ConvertBits.FromInt32(raw, 4) >> 20) & 0xFFF;

    public byte worda => (byte)ConvertBits.FromInt32(raw, 0);
    public byte wordb => (byte)ConvertBits.FromInt32(raw, 4);

    public WGEO_quad(byte[] _data)
    {
        raw = _data;
    }
}
