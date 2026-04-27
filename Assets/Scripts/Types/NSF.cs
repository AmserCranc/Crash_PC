using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NSF
{
    public List<Chunk> chunks;

    public NSF(string streamPath)
    {
//Load the NSF file
        byte[] raw;
        using FileStream fs = new FileStream(streamPath, FileMode.Open, FileAccess.Read);
        {
            raw = new byte[fs.Length];
            fs.Read(raw, 0, raw.Length);
        }

//Decompress the chunks, where needed
        List<Chunk> unprocessedChunks = new();
        int offset = 0;
        while(offset < raw.Length)
        {
            bool isCompressed;
            byte[] chunkData = EnsureDecompressedChunk(ref raw, ref offset, out isCompressed);
            UnprocessedChunk chunk = new(chunkData);
            unprocessedChunks.Add(chunk);
        }

//Classify chunks
        chunks = new();
        foreach(Chunk chunk in unprocessedChunks)
        {
            switch(chunk.type)
            {
                case Chunk.Type.NORM:
                    chunks.Add(new NormalChunk(chunk.data)); break;
                    
                case Chunk.Type.TEXT:
                    chunks.Add(new TextureChunk(chunk.data)); break;
                    
                case Chunk.Type.OSND:
                    chunks.Add(new OldSoundChunk(chunk.data)); break;
                    
                case Chunk.Type.NSND:
                    chunks.Add(new SoundChunk(chunk.data)); break;
                    
                case Chunk.Type.WBNK:
                    chunks.Add(new WavebankChunk(chunk.data)); break;

                case Chunk.Type.SPCH:
                    chunks.Add(new SpeechChunk(chunk.data)); break;    
            }
        }
    }

    byte[] EnsureDecompressedChunk(ref byte[] nsfData, ref int offset, out bool isCompressed)
    {
        if(nsfData is null)                       throw new ArgumentNullException("nsfData");
        if(offset < 0 || offset > nsfData.Length) throw new ArgumentOutOfRangeException("offset");
        if(nsfData.Length < offset + 2)           Debug.LogError("NSF.ReadChunk: Data is too short");

        isCompressed = false;
        byte[] extractedRaw = new byte[Chunk.LENGTH];
        short magic = ConvertBits.FromInt16(ref nsfData, offset); //First field of chunk
        if(magic == Chunk.MAGIC)
        {
            isCompressed = false;
            Array.Copy(nsfData, offset, extractedRaw, 0, Chunk.LENGTH);
            offset += Chunk.LENGTH;
        }
        else if (magic == Chunk.COMP_MAGIC) //Taken decompression from CrashEdit project, placing good faith
        {
            isCompressed = true;
            int pos = 0;
#region DEBUG
if (nsfData.Length < offset + 12) Debug.LogError($"Chunk begining at {offset}, of NSF, is too short");
#endregion
            short zero = ConvertBits.FromInt16(ref nsfData, offset + 2);
            int length = ConvertBits.FromInt32(ref nsfData, offset + 4);
            int skip   = ConvertBits.FromInt32(ref nsfData, offset + 8);
#region DEBUG
if (zero != 0) Debug.Log("NSF.ReadChunk: Zero value is wrong");
if (length < 0 || length > Chunk.LENGTH) Debug.LogError("NSF.ReadChunk: Length field is invalid");
if (skip < 0) Debug.LogError("NSF.ReadChunk: Skip value is negative");
#endregion
            offset += 12;
            while (pos < length)
            {
#region DEBUG
if (nsfData.Length < offset + 1) Debug.LogError("NSF.ReadChunk: Data is too short");
#endregion
                byte prefix = nsfData[offset];
                offset++;
                if ((prefix & 0x80) != 0)
                {
                    prefix &= 0x7F;
#region DEBUG
if (nsfData.Length < offset + 1) Debug.LogError("NSF.ReadChunk: Data is too short");
#endregion
                    int seek = nsfData[offset];
                    offset++;
                    int span = seek & 7;
                    seek >>= 3;
                    seek |= prefix << 5;
                    if (span == 7)
                        span = 64;
                    else
                        span += 3;
#region DEBUG
if (pos - seek < 0) Debug.LogError("NSF.ReadChunk: Repeat begins out of bounds");
if (pos + span > Chunk.LENGTH) Debug.LogError("NSF.ReadChunk: Repeat ends out of bounds");
#endregion
                    // Do NOT use Array.Copy as
                    // overlap is possible i.e. span
                    // may be greater than seek
                    for (int i = 0;i < span;i++)
                        extractedRaw[pos + i] = extractedRaw[pos - seek + i];
                    pos += span;
                }
                else
                {
#region DEBUG
if (nsfData.Length < offset + prefix) Debug.LogError("NSF.ReadChunk: Data is too short");
#endregion
                    Array.Copy(nsfData, offset, extractedRaw, pos, prefix);
                    offset += prefix;
                    pos += prefix;
                }
            }
#region DEBUG
if (nsfData.Length < offset + skip) Debug.LogError("NSF.ReadChunk: Data is too short");
#endregion
            offset += skip;
#region DEBUG
if (nsfData.Length < offset + (Chunk.LENGTH - length)) Debug.LogError("NSF.ReadChunk: Data is too short");
#endregion
            Array.Copy(nsfData, offset, extractedRaw, pos, Chunk.LENGTH - length);
            offset += (Chunk.LENGTH - length);
        }
#region DEBUG
        else
Debug.Log("NSF.ReadChunk: Unknown magic number");
#endregion

        return extractedRaw;
    }

    
}
