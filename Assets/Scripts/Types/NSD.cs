using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class NSD
{
    const int MAX_NSD_SIZE = 128_000;

    const int 
        HASHTABLE_COUNT = 256, 
        PROPERTY_SIZE   = 4,
        COMP_CHNK_COUNT = 64, 
        E_EID_MAP_COUNT = 64,
        HASH_OFFSET_POS = 0x000,
        CHUNK_COUNT_POS = 0x400,
        ENTRY_COUNT_POS = 0x404,
        OBJECT_TYPE_POS = 0x408,
        UNKNOWN_1       = 0x40C,
        UNKNOWN_2       = 0x410,
        UNKNOWN_3       = 0x414,
        UNCOM_CHUNK_POS = 0x418,
        COMPCHUNK_COUNT = 0x41C,
        COMP_CHUNKS_POS = 0x420,
        ENTRY_HASH_TABL = 0x520;
    

    public int[] hashtableOffsets       = new int[HASHTABLE_COUNT * PROPERTY_SIZE];
    public int[] compressedChunkOffsets = new int[COMPCHUNK_COUNT * PROPERTY_SIZE];
    public int[] execEIDmap             = new int[E_EID_MAP_COUNT * PROPERTY_SIZE];
    public int[] entryHashTable; 

    private byte[] raw;
    private int idxAfterHashTable;
    private int idxOfEIDmap;

    public int chunkCount              => ConvertBits.FromInt32(raw, CHUNK_COUNT_POS); 
    public int entryCount              => ConvertBits.FromInt32(raw, ENTRY_COUNT_POS);  
    public int objectEID               => ConvertBits.FromInt32(raw, OBJECT_TYPE_POS);
    public int UNK_1                   => ConvertBits.FromInt32(raw, UNKNOWN_1);
    public int UNK_2                   => ConvertBits.FromInt32(raw, UNKNOWN_2);
    public int UNK_3                   => ConvertBits.FromInt32(raw, UNKNOWN_3);
    public int levelHeaderMagic        => ConvertBits.FromInt32(raw, idxAfterHashTable + 0);
    public int levelID                 => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE);
    public int levelStartZoneEID       => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE * 2);
    public int levelStartCamPath       => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE * 3);
    public int UNK_4                   => ConvertBits.FromInt32(raw, idxAfterHashTable + PROPERTY_SIZE * 4);
    


    public NSD(string streamPath)
    {
        using FileStream fs = new FileStream(streamPath, FileMode.Open, FileAccess.Read);
        {
            raw = new byte[fs.Length];
            fs.Read(raw, 0, raw.Length);
        }
           
        idxAfterHashTable = ENTRY_HASH_TABL + (entryCount * PROPERTY_SIZE * 2);
        idxOfEIDmap = idxAfterHashTable + PROPERTY_SIZE * 5;


    }

#region NSDLink
    public NSDLink GetNSDLink(int idx)
    {
        const int STRIDE = sizeof(Int32) * 2;

        if(idx > HASHTABLE_COUNT) throw new IndexOutOfRangeException();

        int offset = HASH_OFFSET_POS + (idx * STRIDE);

        return new NSDLink(
            _chunkid: ConvertBits.FromInt32(raw, offset                ),
            _entryid: ConvertBits.FromInt32(raw, offset + sizeof(Int32))
        );
    }

    public struct NSDLink
    {
        public int chunkID;
        public int entryID;

        public NSDLink(int _chunkid, int _entryid)
        {
            chunkID = _chunkid;
            entryID = _entryid;
        }
    }
#endregion
#region Hashtable Offset
    public int GetHashtableOffset(int idx)
    {
        const int STRIDE = sizeof(Int32);

        if(idx > HASHTABLE_COUNT) throw new IndexOutOfRangeException();

        int offset = HASH_OFFSET_POS + (idx * STRIDE);

        return ConvertBits.FromInt32(raw, offset);
    }
#endregion
#region Compressed Chunk Offset
    public int GetCompressedChunkOffsets(int idx)
    {
        const int STRIDE = sizeof(Int32);

        if(idx > COMPCHUNK_COUNT) throw new IndexOutOfRangeException();

        int offset = COMP_CHUNKS_POS + (idx * STRIDE);

        return ConvertBits.FromInt32(raw, offset);
    }
#endregion
#region Entry EID Map
    public int GetEIDfromMap(int idx)
    {
        const int STRIDE = sizeof(Int32);

        if(idx > E_EID_MAP_COUNT) throw new IndexOutOfRangeException();

        int offset = idxOfEIDmap + (idx * STRIDE);

        return ConvertBits.FromInt32(raw, offset);
    }
#endregion



}
