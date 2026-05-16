using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Unity.VisualScripting;

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

    public List<WGEO_polygon> polygons;
    public List<WGEO_vertex> verts;
    //public List<WGEO_struct> structs;


    public WGEO(Entry _entry) : base(_entry.data)
    {
        rawData = _entry.ExtractAllItems();

        polygons = new();
        for(int index = 0; index < polyCount; index++)
            polygons.Add(new WGEO_polygon(rawPolys, index * WGEO_polygon.DATA_LENGTH));

        verts = new();
        for(int index = 0; index < vertCount; index++)
            verts.Add(new WGEO_vertex(rawVerts, index * WGEO_vertex.DATA_LENGTH));
        


//DEBUGGING
        PlaceTerrain();
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

    public struct WGEO_polygon
    {
        public const int
            DATA_LENGTH = 8;

        private int offset;
        private byte[] rawData;

        public int wordA       => ConvertBits.FromInt32(rawData, offset + 0);
        public int wordB       => ConvertBits.FromInt32(rawData, offset + 4);
        public int vertA       => (      wordA >> 20) & 0b1111_1111_1111;
        public int vertB       => (      wordB >> 8 ) & 0b1111_1111_1111;
        public int vertC       => (      wordB >> 20) & 0b1111_1111_1111;
        public int modelstruct => (      wordA >> 8 ) & 0b1111_1111_1111;
        public byte page       => (byte)(wordA >> 5   & 0b0111);
        public byte anim0      => (byte)(wordA        & 0b0001_1111);
        public byte unknown    => (byte)(wordB        & 0b1111_1111);


        public WGEO_polygon(byte[] _polyData, int _index)
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
        public int  UVindex     =>        (texInfo >> 22) & 0b0011_1111_1111;
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

    public struct WGEO_face
    {
        public uint2x3 UV;
        public float3x3 verts;
        public Color32 RGBA_0;
        public Color32 RGBA_1;
        public Color32 RGBA_2;
        public Vector4 meta; //CLUTX, CLUTY, Texpage, blendmode
        public Vector4 meta1;//ColourMode, BLANK, BLANK, BLANK
    }

    public void PlaceTerrain()
    {
        List<WGEO_face> faces = new();

        foreach (WGEO_polygon p in polygons)
        {
            WGEO_struct str = GetModelStruct(p.modelstruct);

            if (str is WGEO_textured texFace)
            {
                WGEO_face f = new();

                f.verts.c0 = new float3(
                    (float)(XOff + verts[p.vertA].x) * -1 / WGEO_SCALE,
                    (float)(YOff + verts[p.vertA].y) / WGEO_SCALE,
                    (float)(ZOff + verts[p.vertA].z) / WGEO_SCALE);

                f.verts.c1 = new float3(
                    (float)(XOff + verts[p.vertB].x) * -1 / WGEO_SCALE,
                    (float)(YOff + verts[p.vertB].y) / WGEO_SCALE,
                    (float)(ZOff + verts[p.vertB].z) / WGEO_SCALE);

                f.verts.c2 = new float3(
                    (float)(XOff + verts[p.vertC].x) * -1 / WGEO_SCALE,
                    (float)(YOff + verts[p.vertC].y) / WGEO_SCALE,
                    (float)(ZOff + verts[p.vertC].z) / WGEO_SCALE);

                f.UV.c2 = new uint2((uint)texFace.U1, (uint)texFace.V1 ); // divide x512 y128
                f.UV.c1 = new uint2((uint)texFace.U2, (uint)texFace.V2 ); // divide x512 y128
                f.UV.c0 = new uint2((uint)texFace.U3, (uint)texFace.V3 ); // divide x512 y128

                f.RGBA_0 = new Color32(
                    verts[p.vertA].red,
                    verts[p.vertA].green,
                    verts[p.vertA].blue,
                    1);

                f.RGBA_1 = new Color32(
                    verts[p.vertB].red,
                    verts[p.vertB].green,
                    verts[p.vertB].blue,
                    1);

                f.RGBA_2 = new Color32(
                    verts[p.vertC].red,
                    verts[p.vertC].green,
                    verts[p.vertC].blue,
                    1);

                f.meta.x = texFace.ClutX;
                f.meta.y = texFace.ClutY;
                f.meta.z = GetTPage(p.page);
                f.meta.w = texFace.blendMode;
                f.meta1.x = texFace.colourMode;
                Debug.Log(texFace.colourMode);

                faces.Add(f);
            }
        }

        GameObject go = new GameObject("WGEO");

        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        mf.mesh = BuildMesh(faces);

        mr.material = GLOBAL.WGEO_material;
    }

    public static Mesh BuildMesh(List<WGEO_face> faces)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new();
        List<Vector2> uvs = new();
        List<Vector4> uvMeta = new();
        List<Vector4> uvMeta1 = new();
        List<Color32> colors = new();
        List<int> triangles = new();

        int index = 0;

        foreach (WGEO_face face in faces)
        {
            //
            // VERTICES
            //

            vertices.Add(new Vector3(
                face.verts.c0.x,
                face.verts.c0.y,
                face.verts.c0.z));

            vertices.Add(new Vector3(
                face.verts.c1.x,
                face.verts.c1.y,
                face.verts.c1.z));

            vertices.Add(new Vector3(
                face.verts.c2.x,
                face.verts.c2.y,
                face.verts.c2.z));

            //
            // UVs
            //

            uvs.Add(new Vector2(
                face.UV.c0.x,
                face.UV.c0.y));

            uvs.Add(new Vector2(
                face.UV.c1.x,
                face.UV.c1.y));

            uvs.Add(new Vector2(
                face.UV.c2.x,
                face.UV.c2.y));

            //
            // META
            //

            uvMeta.Add(face.meta);
            uvMeta.Add(face.meta);
            uvMeta.Add(face.meta);

            uvMeta1.Add(face.meta1);
            uvMeta1.Add(face.meta1);
            uvMeta1.Add(face.meta1);

            //
            // COLOURS
            //

            colors.Add(face.RGBA_0);
            colors.Add(face.RGBA_1);
            colors.Add(face.RGBA_2);

            //
            // TRIANGLES
            //

            triangles.Add(index + 0);
            triangles.Add(index + 1);
            triangles.Add(index + 2);

            index += 3;
        }

        mesh.SetVertices(vertices);

        mesh.SetUVs(0, uvs);
        mesh.SetUVs(1, uvMeta);
        mesh.SetUVs(2, uvMeta1);

        mesh.SetColors(colors);

        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
} 
// #region later
//     public struct WGEO_ModelTri
//     {
//         public const byte NullPtr = 0x57;

//         // || Little Endian
//         // || YYSSFTLL PPPPPPPP CCCCCCCA XXXXXXXX
//         const int
//             pTEX_IDX    = 0x0,
//             pANIMATED   = 0x8,
//             pCOLOUR     = 0x9,
//             pKEY        = 0x10,
//             pUNKNOWN    = 0x18,
//             pIDX_TYPE   = 0x1A,
//             pFLAG       = 0x1B,
//             pTRI_TYPE   = 0x1C;

//         private uint raw;

//         public byte texture   => (byte) raw;
//         public bool animated  =>       (raw >> pANIMATED & 0b0000_0001) != 0;
//         public byte colour    => (byte)(raw >> pCOLOUR   & 0b0001_1111);
//         public byte key       => (byte)(raw >> pKEY);
//         public byte unknown   => (byte)(raw >> pUNKNOWN  & 0b0000_0011);
//         public byte indexType => (byte)(raw >> pIDX_TYPE & 0b0000_0001);
//         public bool flag      =>       (raw >> pFLAG     & 0b0000_0001) != 0;
//         public byte triType   => (byte)(raw >> pTRI_TYPE);


//         public WGEO_ModelTri(uint _data)
//         {
//             raw = _data;
//         }
//     }

//     public struct WGEO_ModelTriColour
//     {
//         private uint raw;

//         public byte colour1 => (byte)(raw >> 2 & 0b0001_1111);
//         public byte colour2 => (byte)(raw >> 9 & 0b0001_1111);

//         public WGEO_ModelTriColour(uint _data)
//         {
//             raw = _data;
//         }
//     }
// #endregion

