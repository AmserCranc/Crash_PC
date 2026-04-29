using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

unsafe public class Entry
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

    public delegate Entry EntryFactory(byte[] data, int offset);

    public static Dictionary<Type, EntryFactory> entryTypes = new Dictionary<Type, EntryFactory>
    {
        {Type.SVTX, (data, offset) => new SVTX(data, offset)},
        {Type.TGEO, (data, offset) => new TGEO(data, offset)},
        {Type.WGEO, (data, offset) => new WGEO(data, offset)},
        {Type.SLST, (data, offset) => new SLST(data, offset)},
        {Type.TPAG, (data, offset) => new TPAG(data, offset)},
        {Type.LDAT, (data, offset) => new LDAT(data, offset)},
        {Type.ZDAT, (data, offset) => new ZDAT(data, offset)},
        {Type.GOOL, (data, offset) => new GOOL(data, offset)},
        {Type.ADIO, (data, offset) => new ADIO(data, offset)},
        {Type.MIDI, (data, offset) => new MIDI(data, offset)},
        {Type.INST, (data, offset) => new INST(data, offset)},
        {Type.IMAG, (data, offset) => new IMAG(data, offset)},
        {Type.MDAT, (data, offset) => new MDAT(data, offset)},
        {Type.IPAL, (data, offset) => new IPAL(data, offset)},
        {Type.PBAK, (data, offset) => new PBAK(data, offset)},
        {Type.CVTX, (data, offset) => new CVTX(data, offset)},
    };



    public int[] itemOffsets;
    public int length;

    private byte[] raw;

    public byte[] data  => raw;
    public int magic    => ConvertBits.FromInt32(raw, pMAGIC);
    public int id       => ConvertBits.FromInt32(raw, pID);

    public static EntryFactory ClassifyRaw(byte[] data, int offset)
    {
        return entryTypes[(Type)ConvertBits.FromInt16(data, offset + pTYPE)];
    }


}
