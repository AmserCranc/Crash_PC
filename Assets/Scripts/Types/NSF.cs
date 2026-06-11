using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NSF
{
    public List<Chunk> chunks;
    public NSD nsd;

    public NSF(string streamPath)
    {
        Debug.Log("New NSF");

        GLOBAL.texEIDMap = new();
        GLOBAL.WGEO_material = new Material(Shader.Find("Custom/WGEO_PS1"));
        GLOBAL.HUD = new Material(Shader.Find("Custom/WGEO_PS1"));
        GLOBAL.DEMO = new Material(Shader.Find("Custom/WGEO_PS1"));


        byte[] raw;
        using (FileStream fs = new FileStream(streamPath, FileMode.Open, FileAccess.Read))
        {
            raw = new byte[fs.Length];
            fs.Read(raw, 0, raw.Length);
        }
#region Process Chunks
        List<Chunk> mainChunks = new();
        int offset = 0;

        int? firstChunkId = null;
        bool inPrelude = false;
        List<Chunk> prelude = null;

        while (offset < raw.Length)
        {
            bool isCompressed;
            byte[] chunkData = EnsureDecompressedChunk(raw, ref offset, out isCompressed);
            UnprocessedChunk chunk = new(chunkData);

            if (firstChunkId == null)
            {
                firstChunkId = chunk.id;
            }
            else if (!inPrelude && chunk.id == firstChunkId.Value)
            {
                inPrelude = true;
                prelude = new List<Chunk>(mainChunks);
                mainChunks.Clear();
            }

            mainChunks.Add(chunk);
        }

        chunks = new();

        foreach (Chunk chunk in mainChunks)
        {
            switch (chunk.type)
            {
                case Chunk.Type.NORM:
                    chunks.Add(new NormalChunk(chunk));
                    break;

                case Chunk.Type.TEXT:
                    chunks.Add(new TextureChunk(chunk));
                    break;

                case Chunk.Type.OSND:
                    chunks.Add(new OldSoundChunk(chunk));
                    break;

                case Chunk.Type.NSND:
                    chunks.Add(new SoundChunk(chunk));
                    break;

                case Chunk.Type.WBNK:
                    chunks.Add(new WavebankChunk(chunk));
                    break;

                case Chunk.Type.SPCH:
                    chunks.Add(new SpeechChunk(chunk));
                    break;
                    
            }
        }
#endregion
#region Process Textures
        int pageWidth = 512;
        int pageHeight = 128;
        int pageCount = chunks.Count(c => c is TextureChunk);

        Texture2D atlas = new Texture2D(pageWidth, pageHeight * pageCount, TextureFormat.R8, false, true);
        atlas.filterMode = FilterMode.Point;
        atlas.wrapMode = TextureWrapMode.Clamp;

        var rawtex = atlas.GetRawTextureData<byte>();

        int texPageIndex = 0;

        foreach (Chunk chunk in chunks)
        {
            if (chunk is not TextureChunk tchunk)
                continue;

            byte[] data = tchunk.data;

            int baseY = texPageIndex * pageHeight;

            for (int y = 0; y < pageHeight; y++)
            {
                int srcRow = y * pageWidth;
                int dstRow = (baseY + y) * pageWidth;

                for (int x = 0; x < pageWidth; x++)
                    rawtex[dstRow + x] = data[srcRow + x];
            }

            
            GLOBAL.texEIDMap.Add(tchunk.eid, texPageIndex);
//Debug.Log($"Chunk {tchunk.EIDname} with ID: {tchunk.eid} placed at index {texPageIndex}");
            texPageIndex++;
        }
//Debug.Log($"{chunks.Count} chunks in NSF {streamPath}");

        atlas.Apply(false, false);
        GLOBAL.WGEO_material.SetTexture("_WGEO_Atlas", atlas);
        GLOBAL.WGEO_material.SetFloat("_PageHeight", pageHeight);
        GLOBAL.WGEO_material.SetFloat("_PageWidth", pageWidth);
        GLOBAL.WGEO_material.SetFloat("_PageCount", pageCount);
        GLOBAL.WGEO_material.SetInt("_Page0_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(0));
        GLOBAL.WGEO_material.SetInt("_Page1_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(1));
        GLOBAL.WGEO_material.SetInt("_Page2_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(2));
        GLOBAL.WGEO_material.SetInt("_Page3_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(3));
        GLOBAL.WGEO_material.SetInt("_Page4_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(4));
        GLOBAL.WGEO_material.SetInt("_Page5_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(5));
        GLOBAL.WGEO_material.SetInt("_Page6_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(6));
        GLOBAL.WGEO_material.SetInt("_Page7_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(7));
        GLOBAL.WGEO_material.SetInt("_Page8_EID", GLOBAL.texEIDMap.Keys.ElementAtOrDefault(8));
#endregion




    }



#region Helpers
    void ExtractTexPage(Material mat, TextureChunk tchunk)
    {
        byte[] data = tchunk.data;

        Texture2D page = new Texture2D(512, 128, TextureFormat.R8, false, true);
        page.filterMode = FilterMode.Point;
        page.wrapMode = TextureWrapMode.Clamp;
        page.ignoreMipmapLimit = true;
        var rawtex = page.GetRawTextureData<byte>();

        for (int y = 0; y < 128; y++)
        {
            int row = y * 512;

            for (int x = 0; x < 512; x++)
                rawtex[row + x] = data[row + x];
        }

        page.Apply(false, false);
        mat.SetTexture ("_WGEO_Atlas", page);
        mat.SetFloat   ("_PageHeight", 128);
        mat.SetFloat   ("_PageWidth",  512);
        mat.SetFloat   ("_PageCount",  1);
    }

    byte[] EnsureDecompressedChunk(byte[] nsfData, ref int offset, out bool isCompressed)
    {
        if (nsfData is null)
            throw new ArgumentNullException(nameof(nsfData));

        if (offset < 0 || offset > nsfData.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));

        isCompressed = false;

        byte[] extractedRaw = new byte[Chunk.LENGTH];
        short magic = ConvertBits.FromInt16(nsfData, offset);

        if (magic == Chunk.MAGIC)
        {
            isCompressed = false;
            Array.Copy(nsfData, offset, extractedRaw, 0, Chunk.LENGTH);
            offset += Chunk.LENGTH;
            return extractedRaw;
        }

        if (magic == Chunk.COMP_MAGIC)
        {
            isCompressed = true;

            int pos = 0;

            short zero = ConvertBits.FromInt16(nsfData, offset + 2);
            int length = ConvertBits.FromInt32(nsfData, offset + 4);
            int skip = ConvertBits.FromInt32(nsfData, offset + 8);

            offset += 12;

            while (pos < length)
            {
                byte prefix = nsfData[offset++];

                if ((prefix & 0x80) != 0)
                {
                    prefix &= 0x7F;

                    int seekByte = nsfData[offset++];

                    int span = seekByte & 7;
                    int seek = seekByte >> 3;
                    seek |= prefix << 5;

                    if (span == 7) span = 64;
                    else span += 3;

                    for (int i = 0; i < span; i++)
                        extractedRaw[pos + i] = extractedRaw[pos - seek + i];

                    pos += span;
                }
                else
                {
                    int len = prefix;

                    Array.Copy(nsfData, offset, extractedRaw, pos, len);
                    offset += len;
                    pos += len;
                }
            }

            offset += skip;

            Array.Copy(nsfData, offset, extractedRaw, pos, Chunk.LENGTH - length);
            offset += (Chunk.LENGTH - length);

            return extractedRaw;
        }

        throw new Exception("Unknown magic at " + offset);
    }


#endregion

}
