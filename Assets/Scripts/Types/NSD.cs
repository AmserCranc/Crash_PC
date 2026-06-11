using System;
using System.IO;

public class NSD
{
    const int MAX_NSD_SIZE = 128_000;

    const int
        HASHTABLE_COUNT = 256,
        PROPERTY_SIZE   = 4,
        COMP_CHNK_COUNT = 64,
        E_EID_MAP_COUNT  = 64,

        HASH_OFFSET_POS  = 0x000,
        CHUNK_COUNT_POS  = 0x400,
        ENTRY_COUNT_POS  = 0x404,
        OBJECT_TYPE_POS  = 0x408,
        UNKNOWN_1        = 0x40C,
        UNKNOWN_2        = 0x410,
        UNKNOWN_3        = 0x414,
        UNCOM_CHUNK_POS  = 0x418,
        COMPCHUNK_COUNT  = 0x41C,
        COMP_CHUNKS_POS  = 0x420,
        ENTRY_HASH_TABL  = 0x520;

    const int ENTRY_PAIR_SIZE   = 8;
    const int LEVEL_HEADER_SIZE = 0x118;
    const int LDAT_IMAGE_SIZE   = 62720;

    public int[] hashtableOffsets       = new int[HASHTABLE_COUNT * PROPERTY_SIZE];
    public int[] compressedChunkOffsets = new int[COMP_CHNK_COUNT * PROPERTY_SIZE];
    public int[] execEIDmap             = new int[E_EID_MAP_COUNT * PROPERTY_SIZE];

    public int[] entryHashTable;

    private byte[] raw;

    private int idxAfterHashTable;
    private int idxOfEIDmap;

    public NSD(string streamPath)
    {
        using FileStream fs = new FileStream(streamPath, FileMode.Open, FileAccess.Read);
        {
            raw = new byte[fs.Length];
            fs.Read(raw, 0, raw.Length);
        }

        idxAfterHashTable =
            ENTRY_HASH_TABL +
            (entryCount * ENTRY_PAIR_SIZE);

        idxOfEIDmap =
            idxAfterHashTable + (PROPERTY_SIZE * 5);

        GLOBAL.nsd = this;
    }

    #region Basic Fields

    public int chunkCount => ConvertBits.FromInt32(raw, CHUNK_COUNT_POS);
    public int entryCount => ConvertBits.FromInt32(raw, ENTRY_COUNT_POS);
    public int objectEID  => ConvertBits.FromInt32(raw, OBJECT_TYPE_POS);

    public int UNK_1 => ConvertBits.FromInt32(raw, UNKNOWN_1);
    public int UNK_2 => ConvertBits.FromInt32(raw, UNKNOWN_2);
    public int UNK_3 => ConvertBits.FromInt32(raw, UNKNOWN_3);

    #endregion

    #region Level Header

    public int levelHeaderMagic     => ConvertBits.FromInt32(raw, idxAfterHashTable + 0);
    public int levelID              => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE);
    public int levelStartZoneEID    => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE * 2);
    public int levelStartCamPath    => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE * 3);
    public int UNK_4                => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE * 4);

    #endregion

    #region LDAT (Loading / Misc Images)

    public int ldatOffset =>
        ENTRY_HASH_TABL +
        (entryCount * ENTRY_PAIR_SIZE) +
        LEVEL_HEADER_SIZE;

    public int LDATImageCount =>
        (raw.Length - ldatOffset) / LDAT_IMAGE_SIZE;

    public byte[] GetLDATImage(int index)
    {
        if (index < 0 || index >= LDATImageCount)
            throw new IndexOutOfRangeException();

        byte[] img = new byte[LDAT_IMAGE_SIZE];

        Buffer.BlockCopy(
            raw,
            ldatOffset + index * LDAT_IMAGE_SIZE,
            img,
            0,
            LDAT_IMAGE_SIZE
        );

        return img;
    }

    public byte[] LDATFirstImage => GetLDATImage(0);

    #endregion

    #region NSD Link (Hash Table)

    public NSDLink GetNSDLink(int idx)
    {
        const int STRIDE = sizeof(int) * 2;

        if (idx >= HASHTABLE_COUNT)
            throw new IndexOutOfRangeException();

        int offset = HASH_OFFSET_POS + (idx * STRIDE);

        return new NSDLink(
            ConvertBits.FromInt32(raw, offset),
            ConvertBits.FromInt32(raw, offset + sizeof(int))
        );
    }

    public struct NSDLink
    {
        public int chunkID;
        public int entryID;

        public NSDLink(int chunkID, int entryID)
        {
            this.chunkID = chunkID;
            this.entryID = entryID;
        }
    }

    #endregion

    #region Hashtable Offsets

    public int GetHashtableOffset(int idx)
    {
        const int STRIDE = sizeof(int);

        if (idx >= HASHTABLE_COUNT)
            throw new IndexOutOfRangeException();

        return ConvertBits.FromInt32(
            raw,
            HASH_OFFSET_POS + idx * STRIDE
        );
    }

    #endregion

    #region Compressed Chunk Offsets

    public int GetCompressedChunkOffsets(int idx)
    {
        const int STRIDE = sizeof(int);

        if (idx >= COMP_CHNK_COUNT)
            throw new IndexOutOfRangeException();

        return ConvertBits.FromInt32(
            raw,
            COMP_CHUNKS_POS + idx * STRIDE
        );
    }

    #endregion

    #region EID Map

    public int GetEIDfromMap(int idx)
    {
        const int STRIDE = sizeof(int);

        if (idx >= E_EID_MAP_COUNT)
            throw new IndexOutOfRangeException();

        return ConvertBits.FromInt32(
            raw,
            idxOfEIDmap + idx * STRIDE
        );
    }

    #endregion
}