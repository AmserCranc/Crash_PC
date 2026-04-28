using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGEO_tri
{
    private byte[] rawA;
    private byte[] rawB;
//                   Extract word                     | Extract vertex
    public int v1 => (ConvertBits.FromInt32(ref rawA, 0) >> 8 ) & 0xFFF;
    public int v2 => (ConvertBits.FromInt32(ref rawA, 0) >> 20) & 0xFFF;
    public int v3 => (ConvertBits.FromInt16(ref rawB, 0) >> 4 ) & 0xFFF;
    
    public byte worda => (byte)ConvertBits.FromInt32(ref rawA, 0);
    public byte wordb => (byte)(ConvertBits.FromInt32(ref rawB, 0) & 0xF);

    public WGEO_tri(byte[] _dataA, byte[] _dataB)
    {
        rawA = _dataA;
        rawB = _dataB;
    }
}
