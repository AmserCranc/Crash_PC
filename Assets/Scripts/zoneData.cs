using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EID = System.UInt32;
using static gool;

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
        public Vector3  unknown_h;
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
        public uint         world_count;
        public List<WGEO>   worlds; //This will need to be populated properly
        public uint         paths_idx;
        public uint         path_count;
        public uint         entity_count;
        public uint         neighbour_count;
        public EID[]        neighbours;
        //public ns_loadlist  loadlist;
        public uint         display_flags;
        public zone_gfx     gfx;

        public zone_header(byte[] data)
        {
            throw new NotImplementedException("zone header data not yet translated");
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
        public EID slst;

        public ZDAT parent_zone;

        public uint neighbor_path_count;

        public zone_neighbor_path[] neighbor_paths = new zone_neighbor_path[4];

        public byte entrance_index;
        public byte exit_index;

        public ushort length;

        public ushort cam_mode;

        public short avg_node_dist;

        public short cam_zoom;

        public ushort unknown_a;
        public ushort unknown_b;
        public ushort unknown_c;

        public short direction_x;
        public short direction_y;
        public short direction_z;

        public List<zone_path_point> points = new();
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
