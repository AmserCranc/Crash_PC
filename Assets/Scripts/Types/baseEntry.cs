using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

unsafe public abstract class Entry
{
    public const int MAGIC      = 0x100FFFF;
    public const int //data field offsets
        pMAGIC      = 0x0,
        pID         = 0x4,
        pTYPE       = 0x8,
        pITEM_COUNT = 0xC;

    public enum Type : short
    {
        SVTX =  1, //Object model animation keyframes
        TGEO =  2, //Geometry used by T1-type objects
        WGEO =  3, //World Geometry
        SLST =  4, //Camera geometry ordering list
        TPAG =  5, //Texture data and CLUT
        LDAT =  6, //Level data
        ZDAT =  7, //Zone data (level fragments)
        GOOL = 11, //GOOL Executable
        ADIO = 12, //SFX audio data
        MIDI = 13, //MIDI sequences and sometimes wavebank header
        INST = 14, //Instrument data
        IMAG = 15, //Image data
        MDAT = 17, //Map data, to load special levels
        IPAL = 18, //CLUTs for fade transitions
        PBAK = 19, //Playback demo file, recorded button-presses
        CVTX = 20, //Cutscene vertex models
    }


    public int[] itemOffsets;

    private byte[] raw;

    public byte[] data  => raw;
    public int magic    => ConvertBits.FromInt32(ref raw, pMAGIC);
    public int id       => ConvertBits.FromInt32(ref raw, pID);
    public Type type    => (Type)ConvertBits.FromInt16(ref raw, pTYPE);


    public Entry(byte[] _data)
    {
        if (_data is null)
            throw new ArgumentNullException("data");
        this.raw = _data;
    }

}
