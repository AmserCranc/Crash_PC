using System;
using System.Collections.Generic;
using UnityEngine;
using EID = System.UInt32;

public class NormalChunk : Chunk
{
    public Dictionary<EID, Entry> entries;

    public NormalChunk(Chunk _chunk) : base(_chunk.data)
    {
        entries = new Dictionary<EID, Entry>();
    
        List<Entry> unprocessedEntries = new List<Entry>();
        for(int entry = 0; entry < entryCount; entry++)
        {
            Vector2 entryBounds = GetEntryBounds(entry);
            byte[] entryData = new byte[(int)entryBounds.y - (int)entryBounds.x]; 
            Array.Copy(_chunk.data, (int)entryBounds.x, entryData, 0, (int)entryBounds.y - (int)entryBounds.x);
            unprocessedEntries.Add(new Entry(entryData));
        }

        foreach(Entry e in unprocessedEntries)
            entries.Add(e.id, e.Classify());
    }
}
