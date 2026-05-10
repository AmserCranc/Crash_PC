using System;
using System.Collections.Generic;
using UnityEngine;

unsafe public class Chunk
{
    public const int   LENGTH     = 65536;
    public const short MAGIC      = 0x1234;
    public const short COMP_MAGIC = 0x1235;

    public enum Type : short
    {
        NORM   = 0,
        TEXT   = 1,
        OSND   = 2,
        NSND   = 3,
        WBNK   = 4,
        SPCH   = 5
    }

    Dictionary<Type, Color> displayColour = new Dictionary<Type, Color>
    {
        {Type.NORM, Color.blue},
        {Type.TEXT, Color.green},
        {Type.OSND, Color.magenta},
        {Type.NSND, Color.blue},
        {Type.WBNK, Color.cyan},
        {Type.SPCH, Color.black},

    };

    public const int 
        pMAGIC       = 0x0,
        pTYPE        = 0x2,
        pID          = 0x4,
        pENTRY_COUNT = 0x8,
        pCHECKSUM    = 0xC,
        pENT_OFFSETS = 0x10;

    private byte[] raw;

    public byte[] data       => raw;
    public int    id         => ConvertBits.FromInt32(raw, pID);
    public Type   type       => (Type)ConvertBits.FromInt16(raw, pTYPE);
    public Color  disp       => displayColour[type];
    public int    entryCount => ConvertBits.FromInt32(raw, pENTRY_COUNT);
    public int    magic      => ConvertBits.FromInt16(raw, pMAGIC);

    public Chunk(byte[] _data)
    {
        if (_data is null)
            throw new ArgumentNullException("data");
        if (_data.Length != LENGTH)
            throw new ArgumentException($"Data must be {LENGTH} bytes long.");
        this.raw = _data;

        if(magic != MAGIC && magic != COMP_MAGIC) throw new Exception("Chunk has incorrect magic");
    }

    public Vector2 GetEntryBounds(int _i)
    {
        if(_i > entryCount) throw new Exception("tried to get nonexistent entry");
        if(_i < 0)          throw new Exception("asked for entry of index <= 0");

        int entryOffsetPos = pENT_OFFSETS + (_i * sizeof(Int32));
        int entryOffsetEnd = pENT_OFFSETS + ((_i + 1) * sizeof(Int32));

        return new Vector2(
            ConvertBits.FromInt32(raw, entryOffsetPos),
            ConvertBits.FromInt32(raw, entryOffsetEnd));


    }
}