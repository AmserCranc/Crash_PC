using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

unsafe public class Entry
{
    public const int MAGIC      = 0x100FFFF;
    public const int //data field offsets
        pMAGIC      = 0x0,
        pID         = 0x4,
        pTYPE       = 0x8,
        pITEM_COUNT = 0xC,
        pITEM_OFFS  = 0x10;

    public enum Type : short
    {
        SVTX =  1, //Object model animation keyframes
        TGEO =  2, //used by T1-type objects
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


    private byte[] raw;
    //private int idx;

    public byte[] data          => raw;
    public int magic            => ConvertBits.FromInt32(raw, pMAGIC);
    public int id               => ConvertBits.FromInt32(raw, pID);
    public Type type            => (Type)ConvertBits.FromInt16(raw, pTYPE);
    public int itemCount        => ConvertBits.FromInt32(raw, pITEM_COUNT);

    readonly public int size;
  

    public Entry(byte[] _data)
    {
        raw = _data;

        if(magic != MAGIC) throw new Exception("Entry has incorrect magic");
    }

    public Entry Classify()
    {
        switch(type)
        {
            case Type.SVTX: return new SVTX(this);
            case Type.TGEO: return new TGEO(this);
            case Type.WGEO: return new WGEO(this);
            case Type.SLST: return new SLST(this);
            case Type.TPAG: return new TPAG(this);
            case Type.LDAT: return new LDAT(this);
            case Type.ZDAT: return new ZDAT(this);
            case Type.GOOL: return new GOOL(this);
            case Type.ADIO: return new ADIO(this);
            case Type.MIDI: return new MIDI(this);
            case Type.INST: return new INST(this);
            case Type.IMAG: return new IMAG(this);
            case Type.MDAT: return new MDAT(this);
            case Type.IPAL: return new IPAL(this);
            case Type.PBAK: return new PBAK(this);
            case Type.CVTX: return new CVTX(this);

            default: throw new Exception($"Entry type not recognised {type}");
        }
    }

    public byte[] ExtractItem(int number)
    {
        if(number > itemCount) throw new Exception("tried to access out-of-range item");
        if(number < 0)         throw new Exception("tries to access <0 index item");

        int rootIDX     = ConvertBits.FromInt32(raw, pITEM_OFFS + ( number       * sizeof(Int32)));
        int finalIDX    = ConvertBits.FromInt32(raw, pITEM_OFFS + ((number + 1)  * sizeof(Int32)));
        int length      = finalIDX - rootIDX;

        byte[] itemData = new byte[length];
        Array.Copy(raw, rootIDX, itemData, 0, length);
        return itemData;
    }
}
