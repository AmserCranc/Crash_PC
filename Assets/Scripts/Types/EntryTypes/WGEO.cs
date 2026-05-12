using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class WGEO : Entry
{
    // 0x0  || XOFFSET  | YOFFSET   | ZOFFSET   | POLY_COUNT|
    // 0x10 || VER_COUNT| STR_COUNT | TPAG_COUNT| IS_SKY    |
    // 0x20 || TEX_PAGES-           -           -           -
    const int
        pXOFFSET       = 0x0,
        pYOFFSET       = 0x4,
        pZOFFSET       = 0x8,
        pPOLY_COUNT    = 0xC,
        pVERT_COUNT    = 0x10,
        pSTRU_COUNT    = 0x14,
        pTPAG_COUNT    = 0x18,
        pIS_SKY        = 0x1C,
        pTEX_PAGES     = 0x20,
        pSTRUCTS       = 0x40;

    const int WGEO_SCALE = 400;

    private byte[][] rawData;

    private byte[] rawHeader => rawData[0];
    private byte[] rawPolys  => rawData[1];
    private byte[] rawVerts  => rawData[2];

    public int XOff          => ConvertBits.FromInt32(rawHeader, pXOFFSET);
    public int YOff          => ConvertBits.FromInt32(rawHeader, pYOFFSET);
    public int ZOff          => ConvertBits.FromInt32(rawHeader, pZOFFSET);
    public int TPageCount    => ConvertBits.FromInt32(rawHeader, pTPAG_COUNT);
    public int IsSky         => ConvertBits.FromInt32(rawHeader, pIS_SKY);
    public int polyCount     => ConvertBits.FromInt32(rawHeader, pPOLY_COUNT);
    public int vertCount     => ConvertBits.FromInt32(rawHeader, pVERT_COUNT);
    public int structcount   => ConvertBits.FromInt32(rawHeader, pSTRU_COUNT);

    public int GetTPage(int idx) => ConvertBits.FromInt32(rawHeader, pTEX_PAGES + sizeof(Int32) * idx);

    public ConcurrentBag<WGEO_Polygon> polygons;


    public WGEO(Entry _entry) : base(_entry.data)
    {
        rawData = _entry.ExtractAllItems();

        polygons = new();
        for(int index = 0; index < rawPolys.Length; index++)
            polygons.Add(new WGEO_Polygon(rawPolys, index * WGEO_Polygon.DATA_LEGNTH));
    }

    public WGEO_struct GetModelStruct(int _index)
    {
        const int pIS_TEXTURED = 0b1000_0000;

        int offset = pSTRUCTS + (_index * 4);
        bool isTextured = (rawHeader[offset + 3] & pIS_TEXTURED) != 0;
        int size = isTextured ? 8 : 4;

        if((offset + size) > rawHeader.Length) return null;

        byte[] data = new byte[size];
        Array.Copy(rawHeader, offset, data, 0 , size);
        if(isTextured)
            return new WGEO_textured(data);
        else
            return new WGEO_coloured(data);

    }

    public struct WGEO_Polygon
    {
        public const int
            DATA_LEGNTH = 8;

        private int offset;
        private byte[] rawData;

        public int wordA       => ConvertBits.FromInt32(rawData, offset + 0);
        public int wordB       => ConvertBits.FromInt32(rawData, offset + 4);
        public int vertA       => (      wordA >> 20) & 0b1111_1111_1111;
        public int vertB       => (      wordB >> 8 ) & 0b1111_1111_1111;
        public int vertC       => (      wordB >> 20) & 0b1111_1111_1111;
        public int modelstruct => (      wordA >> 8 ) & 0b1111_1111_1111;
        public byte page       => (byte)(wordA >> 5   & 0b0111);
        public byte anim0      => (byte)(wordA        & 0b0001_0000);
        public byte unknown    => (byte)(wordB        & 0b1111_1111);


        public WGEO_Polygon(byte[] _polyData, int _index)
        {
            rawData = _polyData;
            offset = _index;
        } 
    }

    public class WGEO_struct {}

    public class WGEO_textured : WGEO_struct
    {
        private byte[] raw;

        public int  texInfo     => ConvertBits.FromInt32(raw, 4);
        public int  UVindex     =>        (texInfo >> 22) & 0b0011_0000_0000;
        public byte colourRed   =>         raw[0];
        public byte colourBlue  =>         raw[1];
        public byte colourGreen =>         raw[2];
        public byte blendMode   => (byte)((raw[3]  >> 5)  & 0b0011);
        public byte ClutX       => (byte) (raw[3]         & 0b1111);
        public byte ClutY       => (byte) (texInfo >> 6   & 0b0111_1111);
        public byte colourMode  => (byte) (texInfo >> 20  & 0b0011);
        public byte segment     => (byte) (texInfo >> 18  & 0b0011);
        public byte xOffsetU    => (byte) (texInfo >> 13  & 0b0001_1111);
        public byte yOffsetU    => (byte) (texInfo        & 0b0001_1111);

        public int  w           => 4 <<  (UVindex % 5);
        public int  h           => 4 << ((UVindex / 5) % 5);
        public int  xOff        => ((64 << (2 - colourMode)) * segment) + ((2 << (2 - colourMode)) * xOffsetU);
        public int  yOff        => yOffsetU * 4;
        public int  winding     => UVindex /25;
        public int  U1          => w * ((0x30FF0C >> winding) & 1) + xOff;
        public int  U2          => w * ((0x8799E1 >> winding) & 1) + xOff;
        public int  U3          => w * ((0x4B66D2 >> winding) & 1) + xOff;
        public int  V1          => h * ((0xF3CC30 >> winding) & 1) + yOff;
        public int  V2          => h * ((0x9E7186 >> winding) & 1) + yOff;
        public int  V3          => h * ((0x6DB249 >> winding) & 1) + yOff;



        public WGEO_textured(byte[] _data)
        {
            raw = _data;
        }
    }

    public class WGEO_coloured : WGEO_struct
    {
        private byte[] raw;

        public byte colourRed    => raw[0];
        public byte colourGreen  => raw[1];
        public byte colourBlue   => raw[2];
        public bool n            => (raw[3] & 0b0001_0000) != 0;


        public WGEO_coloured(byte[] _data)
        {
            raw = _data;
        }
    }

    private struct WGEO_face
    {
        public uint2x3 UV;
        public int3x3 verts;
        public Color32 RGBA_0;
        public Color32 RGBA_1;
        public Color32 RGBA_2;
    }

    public void PlaceTerrain()
    {
        const uint pageWidth = 512;

        ConcurrentBag<WGEO_face> faces = new();
        Parallel.ForEach(polygons, (WGEO_Polygon p) =>
        {
            WGEO_struct str = GetModelStruct(p.modelstruct);

            if(str is WGEO_textured texFace)
            {
                WGEO_face f = new();
                f.UV.c2 = new ((uint)(texFace.U1 / pageWidth * -1), (uint)texFace.V1 / 128);
                f.UV.c1 = new ((uint)(texFace.U2 / pageWidth * -1), (uint)texFace.V2 / 128);
                f.UV.c0 = new ((uint)(texFace.U3 / pageWidth * -1), (uint)texFace.V3 / 128);

                f.verts.c0 = new float3((XOff + GetVert(p.vertA).X) *-1 / WGEO_SCALE, (YOff + GetVert(p.vertA).Y) / WGEO_SCALE, (ZOff + GetVert(p.vertA).Z) / WGEO_SCALE);
                f.verts.c0 = new float3((XOff + GetVert(p.vertB).X) *-1 / WGEO_SCALE, (YOff + GetVert(p.vertB).Y) / WGEO_SCALE, (ZOff + GetVert(p.vertB).Z) / WGEO_SCALE);
                f.verts.c0 = new float3((XOff + GetVert(p.vertC).X) *-1 / WGEO_SCALE, (YOff + GetVert(p.vertC).Y) / WGEO_SCALE, (ZOff + GetVert(p.vertC).Z) / WGEO_SCALE);

                                    
                f.RGBA_0 = new Color32(rawVerts[p.vertA].colourRed, rawVerts[p.vertA].colourGreen, rawVerts[p.vertA].colourBlue, 1);
                f.RGBA_1 = new Color32(rawVerts[p.vertB].colourRed, rawVerts[p.vertB].colourGreen, rawVerts[p.vertB].colourBlue, 1); 
                f.RGBA_2 = new Color32(rawVerts[p.vertC].colourRed, rawVerts[p.vertC].colourGreen, rawVerts[p.vertC].colourBlue, 1); 
            }

        });

        Mesh m = new();

        
    }
#region later
    public struct WGEO_ModelTri
    {
        public const byte NullPtr = 0x57;

        // || Little Endian
        // || YYSSFTLL PPPPPPPP CCCCCCCA XXXXXXXX
        const int
            pTEX_IDX    = 0x0,
            pANIMATED   = 0x8,
            pCOLOUR     = 0x9,
            pKEY        = 0x10,
            pUNKNOWN    = 0x18,
            pIDX_TYPE   = 0x1A,
            pFLAG       = 0x1B,
            pTRI_TYPE   = 0x1C;

        private uint raw;

        public byte texture   => (byte) raw;
        public bool animated  =>       (raw >> pANIMATED & 0b0000_0001) != 0;
        public byte colour    => (byte)(raw >> pCOLOUR   & 0b0001_1111);
        public byte key       => (byte)(raw >> pKEY);
        public byte unknown   => (byte)(raw >> pUNKNOWN  & 0b0000_0011);
        public byte indexType => (byte)(raw >> pIDX_TYPE & 0b0000_0001);
        public bool flag      =>       (raw >> pFLAG     & 0b0000_0001) != 0;
        public byte triType   => (byte)(raw >> pTRI_TYPE);


        public WGEO_ModelTri(uint _data)
        {
            raw = _data;
        }
    }

    public struct WGEO_ModelTriColour
    {
        private uint raw;

        public byte colour1 => (byte)(raw >> 2 & 0b0001_1111);
        public byte colour2 => (byte)(raw >> 9 & 0b0001_1111);

        public WGEO_ModelTriColour(uint _data)
        {
            raw = _data;
        }
    }
#endregion
}
