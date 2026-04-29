using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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
    

    public int[] hashtableOffsets       = new int[HASHTABLE_COUNT];
    public int[] compressedChunkOffsets = new int[COMPCHUNK_COUNT];
    public int[] execEIDmap             = new int[E_EID_MAP_COUNT];
    public int[] entryHashTable; 

    private byte[] raw;
    private int idxAfterHashTable;

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

        for(int offset = HASH_OFFSET_POS; offset < HASHTABLE_COUNT * PROPERTY_SIZE; offset += PROPERTY_SIZE)
            hashtableOffsets[offset] = ConvertBits.FromInt32(raw, offset);

        int idx = 0;
        for(int chunk = COMP_CHUNKS_POS; chunk < COMP_CHUNKS_POS + (COMP_CHNK_COUNT * PROPERTY_SIZE); chunk += PROPERTY_SIZE)
        {
            compressedChunkOffsets[idx] = ConvertBits.FromInt32(raw, chunk);
            idx++;
        }
            
        idx = 0;
        entryHashTable = new int[entryCount];
        for(int entry = ENTRY_HASH_TABL; entry < ENTRY_HASH_TABL + (entryCount * PROPERTY_SIZE * 2); entry += PROPERTY_SIZE * 2)
        {
            entryHashTable[idx    ] = ConvertBits.FromInt32(raw, entry);
            entryHashTable[idx + 1] = ConvertBits.FromInt32(raw, entry + PROPERTY_SIZE);
            idx++;
        }

        idxAfterHashTable = ENTRY_HASH_TABL + (entryCount * PROPERTY_SIZE * 2);


        idx = 0;
        int idxOfEIDmap = idxAfterHashTable + PROPERTY_SIZE * 5;
        for(int EID = idxOfEIDmap; EID < idxOfEIDmap + (PROPERTY_SIZE * E_EID_MAP_COUNT); EID += PROPERTY_SIZE)
        {
            execEIDmap[idx] = ConvertBits.FromInt32(raw, EID);
            idx++;
        }
    }
}
