using System;

unsafe public abstract class Chunk
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

    public const int 
        MAGIC_POS   = 0x0,
        TYPE_POS    = 0x2,
        ID_POS      = 0x4,
        ENTRY_COUNT = 0x8,
        CHECKSUM    = 0xC,
        ENT_OFFSETS = 0x10;

    private byte[] raw;

    public byte[] data => raw;
    public int    id   => ConvertBits.FromInt32(ref raw, ID_POS);
    public Type   type => (Type)ConvertBits.FromInt16(ref raw, TYPE_POS);

    public Chunk(byte[] _data)
    {
        if (_data is null)
            throw new ArgumentNullException("data");
        if (_data.Length != LENGTH)
            throw new ArgumentException($"Data must be {LENGTH} bytes long.");
        this.raw = _data;
    }


}