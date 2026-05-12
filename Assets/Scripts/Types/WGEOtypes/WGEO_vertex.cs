using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

public readonly struct WGEO_vertex
{
    public const int
        DATA_LENGTH = 8;

    private readonly byte[] rawData;
    private readonly int offset;

    public short x     => (short)(ConvertBits.FromInt16(rawData, offset + 4) & 0xFFF8);
    public short y     => (short)(ConvertBits.FromInt16(rawData, offset + 6) & 0xFFF8);
    public int   zHigh =>         rawData[offset + 6] & 7;
    public int   zMid  =>        (rawData[offset + 4] & 6) >> 1;
    public int   zLow  =>         rawData[offset + 3];
    public short z     => (short)(zHigh << 13 | zMid << 11 | zLow << 3);
    public byte  red   => rawData[offset + 0]; 
    public byte  green => rawData[offset + 1];
    public byte  blue  => rawData[offset + 2];
    public bool  fx    => (rawData[offset + 4] & 1) != 0;


    public WGEO_vertex(byte[] _data, int _index)
    {
        rawData = _data;
        offset = _index;
    }

}
