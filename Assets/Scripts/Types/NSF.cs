using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NSF
{
    public List<UnprocessedChunk> chunks;
    public List<UnprocessedChunk> compressedChunks;

    public NSF(string streamPath)
    {
        byte[] raw;
        using FileStream fs = new FileStream(streamPath, FileMode.Open, FileAccess.Read);
        {
            raw = new byte[fs.Length];
            fs.Read(raw, 0, raw.Length);
        }

        chunks = new();
        int offset = 0;
        while(offset < raw.Length)
        {
            bool isCompressed;
            byte[] chunkData = ReadChunk(raw, ref offset, out isCompressed);
            UnprocessedChunk chunk = new(chunkData);
            if(isCompressed)
                compressedChunks.Add(chunk);
            else
                chunks.Add(chunk);
        }
    }

    byte[] ReadChunk(byte[] nsfData, ref int offset, out bool isCompressed)
    {
        if(nsfData is null)                       throw new ArgumentNullException("nsfData");
        if(offset < 0 || offset > nsfData.Length) throw new ArgumentOutOfRangeException("offset");
        if(nsfData.Length < offset + 2)           Debug.LogError("NSF.ReadChunk: Data is too short");

        isCompressed = false;
        byte[] extractedRaw = new byte[UnprocessedChunk.LENGTH];
        short magic = ConvertBits.FromInt16(ref nsfData, offset); //First field of chunk
        if(magic == UnprocessedChunk.MAGIC)
        {
            isCompressed = false;
            Array.Copy(nsfData, offset, extractedRaw, 0, UnprocessedChunk.LENGTH);
        }
        else if (magic == UnprocessedChunk.COMP_MAGIC) //Taken from CrashEdit project, placing good faith
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
if (length < 0 || length > UnprocessedChunk.LENGTH) Debug.LogError("NSF.ReadChunk: Length field is invalid");
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
if (pos + span > UnprocessedChunk.LENGTH) Debug.LogError("NSF.ReadChunk: Repeat ends out of bounds");
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
if (nsfData.Length < offset + (UnprocessedChunk.LENGTH - length)) Debug.LogError("NSF.ReadChunk: Data is too short");
#endregion
            Array.Copy(nsfData, offset, extractedRaw, pos, UnprocessedChunk.LENGTH - length);
            offset += (UnprocessedChunk.LENGTH - length);
        }
#region DEBUG
        else
Debug.Log("NSF.ReadChunk: Unknown magic number");
#endregion

        return extractedRaw;
    }
}
