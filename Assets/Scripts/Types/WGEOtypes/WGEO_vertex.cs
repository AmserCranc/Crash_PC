using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

public class WGEO_vertex
{
    private byte[] rawData;

    short x     => (short)(ConvertBits.FromInt16(rawData, 4) & 0xFFF8);
    short y     => (short)(ConvertBits.FromInt16(rawData, 6) & 0xFFF8);
    int   zHigh =>         rawData[6] & 7;
    int   zMid  =>        (rawData[4] & 6) >> 1;
    int   zLow  =>         rawData[3];
    

    public WGEO_vertex(byte[] _data)
    {
        rawData = _data;
    }

}
