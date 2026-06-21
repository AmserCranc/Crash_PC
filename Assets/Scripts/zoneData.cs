using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EID = System.UInt32;
using static gool;
using System.Runtime.InteropServices;

public class zoneData
{
    public enum ZoneFlags : uint
    {
        UP_DOWN               = 0x1,
        SOLID_BOTTOM          = 0x2,
        HAS_WATER             = 0x4,
        LOC_BASED_WAIT        = 0x8,
        FOG                   = 0x10,
        AUTO_CAM_ZOFFSET      = 0x80,
        RIPPLE                = 0x100,
        LIGHTNING             = 0x200,
        DARK2                 = 0x400,
        CAM_BOUNCE            = 0x1000,
        NO_SAVE               = 0x2000,
        SIDE_SCROLL           = 0x4000,
        BACKWARD              = 0x8000,
        SOLID_TOP             = 0x20000,
        DISCARD_BELOW_PATHS   = 0x40000,
        AUTO_CAM_SKIP_DISABLE = 0x80000,
        SOLID_SIDES           = 0x100000
    }

    public class zone_world
    {
        public EID eid;
        public Vector3Int trans;
        public WGEO geometry;
        public List<TPAG> tpages = new();
    }

    public class zone_gfx
    {
        public uint     vram_fill_height;
        public uint     unknown_a;
        public uint     visibility_depth;
        public uint     unknown_b;
        public uint     unknown_c;
        public uint     unknown_d;
        public uint     unknown_e;
        public uint     flags;
        public int      water_y;
        public EID      midi;
        public uint     unknown_g;
        public Color    unknown_h;
        public byte     unused_a;
        public Vector3  vram_fill;
        public byte     unused_b;
        public Vector3  far_color;
        public uint     unused_c;
        public colours  object_colours;
        public colours  player_colours;
    }
    public class zone_header
    {
        private byte[]      headerData;

        public const int
            pWORLD_COUNT    = 0x0,
            pHEAD_COUNT     = 0x204,
            pZONE_COUNT     = 0x210; 

        public uint         world_count         => (uint)ConvertBits.FromInt32(headerData, pWORLD_COUNT);
        public List<WGEO>   worlds; //This will need to be populated properly
        public uint         paths_idx;
        public uint         path_count;
        public uint         entity_count;
        public uint         neighbour_count;
        public EID[]        neighbours;
        //public ns_loadlist  loadlist;
        public uint         display_flags;
        public zone_gfx     gfx;

        public zone_header(byte[] _headerData)
        {
            headerData = new byte[_headerData.Length];
            neighbours = new EID[4];
            gfx = new zone_gfx();

        }
    }

    public class zone_octree
    {
        public ushort root;

        public ushort max_depth_x;
        public ushort max_depth_y;
        public ushort max_depth_z;

        public ushort[] nodes;
    }

    public class zone_rect
    {
        public int x;
        public int y;
        public int z;

        public uint w;
        public uint h;
        public uint d;

        public uint unk;

        public zone_octree octree;
    }

    public class zone_neighbor_path
    {
        public byte relation;
        public byte neighbor_zone_idx;
        public byte path_idx;
        public byte goal;

        public zone_neighbor_path(byte[] raw, int offset, int idx)
        {
            relation            = raw[offset + (idx * 4) + 0];
            neighbor_zone_idx   = raw[offset + (idx * 4) + 1];
            path_idx            = raw[offset + (idx * 4) + 2];
            goal                = raw[offset + (idx * 4) + 3];

        }
    }

    public class zone_path_point
    {
        public short x;
        public short y;
        public short z;

        public short rot_y;
        public short rot_x;
        public short rot_z;
    }

    public class zone_path
    {
        private byte[] raw;

        const int 
            pSLST           = 0x00,
            pPARENT_Z       = 0x04,
            pNEIGH_COUNT    = 0x08,
            pNEIGH_PATHS    = 0x0C,
            pENTRANCE_IDX   = 0x1C,
            pEXIT_IDX       = 0x1D,
            pLENGTH         = 0x1E,
            pCAM_MODE       = 0x20,
            pAVG_DISTANCE   = 0x22,
            pZOOM           = 0x24,
            pUNK_1          = 0x26,
            pUNK_2          = 0x28,
            pUNK_3          = 0x2A,
            pX_DIR          = 0x2C,
            pY_DIR          = 0x2E,
            pZ_DIR          = 0x30;

        public EID      slst                => (uint)ConvertBits.FromInt32(raw, pSLST);
        public EID      parent_zone         => (uint)ConvertBits.FromInt32(raw, pPARENT_Z);
        public uint     neighbor_path_count => (uint)ConvertBits.FromInt32(raw, pNEIGH_COUNT);
        public zone_neighbor_path[] neighbor_paths = new zone_neighbor_path[4];
        public byte     entrance_index      => raw[pENTRANCE_IDX];
        public byte     exit_index          => raw[pEXIT_IDX];
        public short    length              => ConvertBits.FromInt16(raw, pLENGTH);
        public short    cam_mode            => ConvertBits.FromInt16(raw, pCAM_MODE);
        public short    avg_node_dist       => ConvertBits.FromInt16(raw, pAVG_DISTANCE);
        public short    cam_zoom            => ConvertBits.FromInt16(raw, pZOOM);
        public short    unknown_a           => ConvertBits.FromInt16(raw, pUNK_1);
        public short    unknown_b           => ConvertBits.FromInt16(raw, pUNK_2);
        public short    unknown_c           => ConvertBits.FromInt16(raw, pUNK_3);
        public short    direction_x         => ConvertBits.FromInt16(raw, pX_DIR);
        public short    direction_y         => ConvertBits.FromInt16(raw, pY_DIR);
        public short    direction_z         => ConvertBits.FromInt16(raw, pZ_DIR);

        public List<zone_path_point> points = new();

        public zone_path(byte[] rawData)
        {
            raw = rawData;
        }

        public zone_neighbor_path GetNeighbourPath(int idx)
        {
            return new zone_neighbor_path(raw, pNEIGH_PATHS, idx);
        }
    }

    public class zone_entity_path_point
    {
        public short x;
        public short y;
        public short z;
    }

    public class zone_entity
    {
        public ZDAT parent_zone;

        public ushort spawn_flags;

        public ushort group;

        public ushort id;

        public ushort path_length;

        // union
        public short init_flags_a;
        public short init_flags_b;
        public short init_flags_c;

        public Vector3Int rotation;

        public byte type;

        public byte subtype;

        public List<zone_entity_path_point> path_points = new();
    }
        
}
