using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Title
{
    static public UVInfo    uvInfo;
    static public TileInfo  tileInfo;
    static public TImgInfo  titleImageInfo;
    static public Title_s   titleStruct;


    static public int TitleUpdate()
    {
        throw new NotImplementedException();
    }

    public struct UVInfo
    {
        public uint UIndex;
        public uint VIndex;
        public uint Mode;

        public UVInfo(byte[] raw, int offset)
        {
            UIndex = (uint)ConvertBits.FromInt32(raw, offset + 0);
            VIndex = (uint)ConvertBits.FromInt32(raw, offset + 4);
            Mode   = (uint)ConvertBits.FromInt32(raw, offset + 8);
        }
    }

    public class TileInfo
    {
        private uint word0;
        private uint word1;
        private uint word2;
        private uint word3;

        public byte UIndex => (byte)(word0 & 0xF);
        public byte VIndex => (byte)((word0 >> 4) & 0xF);
        public ushort XIndex => (ushort)((word0 >> 8) & 0x1FF);
        public byte YIndex => (byte)((word0 >> 17) & 0xFF);
        public byte Mode => (byte)((word0 >> 25) & 0x3);
        public ushort ClutIndex => (ushort)(word1 & 0x1FF);
        public ushort X => (ushort)((word1 >> 9) & 0x3FFF);
        public ushort Y => (ushort)(word2 & 0x3FFF);
    }

    public class TImgInfo
    {
        public UVInfo[] UVInfos;
        public TileInfo[] TileInfos;

        public TImgInfo()
        {
            UVInfos = new UVInfo[33 * 16];
            TileInfos = new TileInfo[33 * 16];
        }
    }

    public class Title_s
    {
        public uint Mdat;

        public uint Width;
        public uint Height;

        public uint WidthTiles;
        public uint HeightTiles;

        public uint XOffset;
        public uint YOffset;

        public TImgInfo ImageInfo = new();

        public ushort[] ClutIds = new ushort[480];

        public ushort[] TPageIds = new ushort[4];

        public uint[,] DrModes = new uint[4, 3];

        public int TransitionState;
        public int AtTitle;

        public uint Unk7;
        public uint Unk8;
        public uint Unk9;
        public uint Unk10;
        public uint Unk11;
        public uint Unk12;
        public uint Unk13;
        public uint Unk14;
        public uint Unk15;
        public uint Unk16;
        public uint Unk17;
        public uint Unk18;

        public int State;
        public int NextState;
    }
}
