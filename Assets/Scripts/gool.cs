using System;
using EID = System.UInt32;
using LID = System.UInt32;
using PEID = System.UInt32;

using static Level;
using static GLOBAL;
using static geom;
using static zoneData;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class gool
{
    public const int EID_NONE = 0x6396347F;
#region gool flags
/* events */
    public const uint EVENT_JUMPED_ON       = 0;
    public const uint EVENT_HIT             = 0x300;
    public const uint EVENT_SPIN_HIT        = 0x400;
    public const uint EVENT_TRIGGERED       = 0x800;
    public const uint EVENT_FALL_KILL       = 0x900;
    public const uint EVENT_BOX_STACK_BREAK = 0x900;
    public const uint EVENT_HIT_INVINCIBLE  = 0xA00;
    public const uint EVENT_STATUS          = 0xF00;
    public const uint EVENT_COMBO           = 0x1000;
    public const uint EVENT_RESPAWN         = 0x1300;
    public const uint EVENT_EAT             = 0x1400;
    public const uint EVENT_BOUNCE          = 0x1500;
    public const uint EVENT_WARP            = 0x1600;
    public const uint EVENT_SQUASH          = 0x1900;
    public const uint EVENT_TERMINATE       = 0x1A00;
    public const uint EVENT_WIN_BOSS        = 0x1D00;
    public const uint EVENT_FLING           = 0x1D00;
    public const uint EVENT_EXPLODE         = 0x1E00;
    public const uint EVENT_BURN            = 0x1F00;
    public const uint EVENT_DROWN           = 0x2100;
    public const uint EVENT_SHOCK           = 0x2300;
    public const uint EVENT_HIT_FENCE       = 0x2300;
    public const uint EVENT_BOULDER_SQUASH  = 0x2500;
    public const uint EVENT_LEVEL_END       = 0x2900;
    public const uint EVENT_PLAYER_DAMAGE   = 0x2B00;

/* status a flags */
    public const uint FLAG_GROUNDLAND        = 0x1;
    public const uint FLAG_TOWARD_GOAL       = 0x4;
    public const uint FLAG_TROT_DIR          = 0x8;
    public const uint FLAG_CHANGE_PATH_DIR   = 0x10;
    public const uint FLAG_FIRST_FRAME       = 0x20;
    public const uint FLAG_HIT_AT_BOTTOM     = 0x80;
    public const uint FLAG_INVALID_PATH      = 0x200;
    public const uint FLAG_DYING             = 0x400;
    public const uint FLAG_REACHED_TROT      = 0x800;
    public const uint FLAG_PLAYER_D_COLLIDER = 0x1000;
    public const uint FLAG_LBOUND_INVALID    = 0x8000;
    public const uint FLAG_KEEP_EVENT_STACK  = 0x20000;
    public const uint FLAG_ON_FLOOR          = 0x40000;
    public const uint FLAG_ATOP_OBJECT       = 0x200000;

/* status b flags */
    public const uint FLAG_ROT_Y             = 0x1;
    public const uint FLAG_TRACK_PATH_ROT    = 0x2;
    public const uint FLAG_TRACK_PATH_SIGN   = 0x4;
    public const uint FLAG_STOPPED_BY_SOLID  = 0x8;
    public const uint FLAG_COLLIDABLE        = 0x10;
    public const uint FLAG_COLLIDEABLE       = 0x10;
    public const uint FLAG_GRAVITY           = 0x20;
    public const uint FLAG_TRANS_MOTION      = 0x40;
    public const uint FLAG_DPAD_CONTROL      = 0x80;
    public const uint FLAG_INVISIBLE         = 0x100;
    public const uint FLAG_2D                = 0x200;
    public const uint FLAG_ROT_X             = 0x2000;
    public const uint FLAG_SOLID_GROUND      = 0x4000;
    public const uint FLAG_ORIENT_ON_PATH    = 0x8000;
    public const uint FLAG_SOLID_SIDES       = 0x10000;
    public const uint FLAG_SOLID_TOP         = 0x20000;
    public const uint FLAG_ROT_Y2            = 0x80000;
    public const uint FLAG_FORCE_UPDATE      = 0x2000000;
    public const uint FLAG_HAS_SHADOW        = 0x4000000;
    public const uint FLAG_STRING_CENTER     = 0x4000000;
    public const uint FLAG_SOLID_BOTTOM      = 0x8000000;
    public const uint FLAG_STALL             = 0x10000000;
    public const uint FLAG_PAUSED            = 0x20000000;  /* GOOL_DEBUG extension only */
    public const uint FLAG_STEP              = 0x40000000;  /* GOOL_DEBUG extension only */

/* state/status c flags */
    public const uint FLAG_GROUND_STATE      = 0x4;
    public const uint FLAG_AIR_STATE         = 0x8;
    public const uint FLAG_FLING_STATE       = 0x10;  /* rotate state? */
    public const uint FLAG_COLOVERRIDE_STATE = 0x800;
    public const uint FLAG_MENUTEXT_STATE    = 0x20000;
    public const uint FLAG_NOTERM_STATE      = 0x40000;
    public const uint FLAG_EXPENDABLE_STATE  = 0x80000;

/* spawn flags */
    public const uint FLAG_SPAWNED           = 0x1;
    public const uint FLAG_ACTIVE_SPAWN      = 0x8;

/* display flags */
    public const uint FLAG_DISPLAY_WORLDS    = 0x1;
    public const uint FLAG_CAM_UPDATE        = 0x2;
    public const uint FLAG_DISPLAY           = 0x4;
    public const uint FLAG_ANIMATE           = 0x8;
    public const uint FLAG_DISPLAY_C1        = 0x10;
    public const uint FLAG_ANIMATE_C1        = 0x20;
    public const uint FLAG_DISPLAY_C356      = 0x40;
    public const uint FLAG_ANIMATE_C356      = 0x80;
    public const uint FLAG_ANIMATE_C2        = 0x100;
    public const uint FLAG_DISPLAY_C2        = 0x200;
    public const uint FLAG_ANIMATE_C4        = 0x400;
    public const uint FLAG_DISPLAY_C4        = 0x800;
    public const uint FLAG_COUNT_DRAWS       = 0x1000;
    public const uint FLAG_DISPLAY_SOLID_BGC = 0x2000;
    public const uint FLAG_FORCE_DISP_MENUS  = 0x4000;
    public const uint FLAG_FORCE_ANIM_MENUS  = 0x8000;
    public const uint FLAG_SPIN_DEATH        = 0x10000;
    public const uint FLAG_DISPLAY_IMAGES    = 0x20000;
    public const uint FLAG_SPIN_ACCEL        = 0x40000;
    public const uint FLAG_DISPLAY_UNK       = 0x200000;

    public const uint FLAG_DISPANIM_ALL      = 0x3C00F;
    public const uint FLAG_DISPANIM_OBJECTS  = 0xFFFC;


/* interpreter flags */
    public const uint FLAG_SUSPEND_ON_RET    = 0x1;
    public const uint FLAG_SUSPEND_ON_RETLNK = 0x2;
    public const uint FLAG_SUSPEND_ON_ANIM   = 0x4;
    public const uint FLAG_EVENT_SERVICE     = 0x8;
    public const uint FLAG_STATUS_PRESERVE   = 0x10;
    public const uint FLAG_RETURN_EVENT      = 0x20;

/* constants */
    public const uint OBJECT_COUNT           = 96;
    public const uint LEVEL_SPAWN_COUNT      = 3592;
    public const uint SPAWN_COUNT            = 304;

/* game state */
    public const int GAME_STATE_CUTSCENE     = 0;
    public const int GAME_STATE_PLAYING      = 0x100;
    public const int GAME_STATE_GAMEOVER     = 0x200;
    public const int GAME_STATE_CONTINUE     = 0x300;
    public const int GAME_STATE_NEW_GEM      = 0x500;
    public const int GAME_STATE_TITLE        = 0x600;
#endregion
#region types
    public struct move_state
    {
        public int dir, angle, speed_scale;
    }
    public struct accel_state
    {
        public int accel, max_speed, decel;
        public uint unk;
    }
    public struct const_buf
    {
        public uint[] buf;
        public int idx;
    }
    public struct gool_bound
    {
        public bound bound;
        public gool_object obj;
    }
    [StructLayout(LayoutKind.Explicit, Size = 48, Pack = 2)]
    public struct colours
    {
        [FieldOffset(0)]
        public Vector3 LightMat;
        [FieldOffset(0)]
        public Vector3 Color;
        [FieldOffset(0)]
        public unsafe fixed ushort L[12];

        [FieldOffset(24)]
        public Vector3 ColorMat;
        [FieldOffset(24)]
        public Vector3 Intensity;
        [FieldOffset(24)]
        public unsafe fixed ushort vertColorsRaw[12];
        [FieldOffset(24)]
        public unsafe fixed ushort c[12];

        [FieldOffset(0)]
        public unsafe fixed ushort a[24];
    }
    public class gool_object : MonoBehaviour
    {
        public handle       handle;
        public bound        bounds;
        public Entry        global;
        public Entry        external;
        public Entry        zone;
        public uint         state;
        public colours      colours;
        public gool_process process;
        public vectors      vectors;

        public gool_object()
        {
            handle = new handle();
            bounds = new bound();
            process = new gool_process();
            vectors = new vectors();

        }

    }
    public class handle
    {
        public int type;

        public gool_object self;
        public int subtype;
    }
    public class vectors
    {
        public Vector3 trans, rot, scale, velocity, misc_a, misc_b, misc_c;
        public int move_flags_a, mode_flags_b, mode_flags_c;
        public gool_link box_link;
    }
    public class gool_link
    {
        public gool_object prev, next;
    }
    public class gool_process
    {
        public gool_links links = new gool_links();

        public Vector3[] vectors    = new Vector3[6];
        public Vector3[] angles     = new Vector3[6];

        public uint 
            status_a, status_b, status_c, subtype, pid_flags, sp, pc, fp, tp, ep, once_p;

        public uint misc_flag;
        public gool_object misc_child;
        public uint misc_node;
        public Entry misc_entry;
        public uint misc_memcard;

        public uint 
            ack, anim_stamp, state_stamp, anim_counter, path_length, 
            floor_y, state_flags, invincibility_state, invincibility_stamp, 
            floor_impact_stamp, size, gool_event, voice_id, _150, anim_frame, node;

        public uint[] memory = new uint[64];

        public gool_anim anim_seq;
        public zone_entity entity;

        public int path_progress, speed, floor_impact_velocity, cam_zoom, ang_velocity_y, hotspot_size, _154;

    }
    public class gool_links
    {
        public gool_object self;
        public gool_object parent;
        public gool_object sibling;
        public gool_object creator;
        public gool_object player;
        public gool_object collider;
        public gool_object interrupter;
        public gool_object children;
    }
    public class header
    {
        public uint type, category, unk_0x8, init_sp, subtype_map_idx, unk_0x14;

        public header(byte[] raw)
        {
            Debug.Log($"new Gool Header was created with {raw.Length} bytes. Header contains 6 uints for {sizeof(uint) * 6} bytes. Check this for parity.");
            type            = (uint)ConvertBits.FromInt32(raw, sizeof(uint) * 0);
            category        = (uint)ConvertBits.FromInt32(raw, sizeof(uint) * 1);
            unk_0x8         = (uint)ConvertBits.FromInt32(raw, sizeof(uint) * 2);
            init_sp         = (uint)ConvertBits.FromInt32(raw, sizeof(uint) * 3);
            subtype_map_idx = (uint)ConvertBits.FromInt32(raw, sizeof(uint) * 4);
            unk_0x14        = (uint)ConvertBits.FromInt32(raw, sizeof(uint) * 5);
        }
    }
    public class state_maps
    {
        public ushort[] map;

        public state_maps(byte[] data)
        {
            map = new ushort[data.Length / 2];

            for (int i = 0; i < map.Length; i++)
                map[i] = (ushort)ConvertBits.FromInt16(data, i * 2);
        }
    }
    public class gool_anim
    {
//NOT IMPLEMENTED
    }
#endregion
#region variables
    static public readonly move_state[] moveStates = new move_state[16]
    {
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x100 },
        new move_state{ dir = 0, angle = 0x800, speed_scale = 0x100 },
        new move_state{ dir = 2, angle = 0x400, speed_scale = 0x100 },
        new move_state{ dir = 1, angle = 0x600, speed_scale = 0x147 },
        new move_state{ dir = 4, angle = 0x000, speed_scale = 0x100 },
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x000 },
        new move_state{ dir = 3, angle = 0x200, speed_scale = 0x147 },
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x000 },
        new move_state{ dir = 6, angle = 0xC00, speed_scale = 0x100 },
        new move_state{ dir = 7, angle = 0xA00, speed_scale = 0x147 },
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x000 },
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x000 },
        new move_state{ dir = 5, angle = 0xE00, speed_scale = 0x147 },
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x000 },
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x000 },
        new move_state{ dir = 8, angle = 0x000, speed_scale = 0x000 }
    };
    static public readonly accel_state[] accel_states  = new accel_state[7]
    {
        new accel_state{ accel = 0x000000, max_speed = 0x7D000, unk = 0x00, decel = 0x000000 },
        new accel_state{ accel = 0x271000, max_speed = 0x96000, unk = 0x1E, decel = 0x271000 },
        new accel_state{ accel = 0x138800, max_speed = 0x96000, unk = 0x1E, decel = 0x09C400 },
        new accel_state{ accel = 0x190000, max_speed = 0xAAE60, unk = 0x0F, decel = 0x190000 },
        new accel_state{ accel = 0x271000, max_speed = 0xC8000, unk = 0x1E, decel = 0x271000 },
        new accel_state{ accel = 0x1A0AAA, max_speed = 0x64000, unk = 0x1E, decel = 0x271000 },
        new accel_state{ accel = 0x0D0555, max_speed = 0x64000, unk = 0x1E, decel = 0x09C400 }
    };
    static public uint[]        consts         = new uint[2];
    static public const_buf     in_consts;
    static public const_buf     out_consts;
    static public EID           crash_eid      = 0;
    static public gool_object   crash          = null;
    static public ushort[]      level_spawns   = new ushort[LEVEL_SPAWN_COUNT];      
    static public uint[]        spawns         = new uint[SPAWN_COUNT];
    static public gool_object[] objects;        
    static public gool_object   player;
    //static public handle[]      handles        = new handle[8];
    //static public handle        free_objects;
    static public gool_object   cur_obj;
    static public uint          frames_elapsed;
    static public gool_bound[]  object_bounds  = new gool_bound[28];
    static public int           object_bound_count;
    //static public globals       globals;


#endregion

    public gool()
    {
        in_consts = new const_buf { buf = consts, idx = 0 };
        out_consts = new const_buf { buf = consts, idx = 0 };
    }

    static public GameObject ObjectCreate(gool_object parent, int exec, int subtype, int argc, uint argv, int flag)
    {
        GameObject newObject = new();
        gool_object child = newObject.AddComponent<gool_object>();;
        Entry zone;
        zone_header header;

        if (exec == 0 && subtype == 0)
        {
            crash = player;
            child = crash;
        }
        else
        {
            child = new gool_object();
        }

        ObjectAddChild(parent, child);
        ObjectInit(child, exec, subtype, argc, (int)argv);

        zone = child.zone != null ? child.zone : Level.cur_zone;
        header = new zone_header(zone.ExtractItem(0));

        child.colours = (child == crash)
            ? header.gfx.player_colours
            : header.gfx.object_colours;

        return newObject;
    }
    static public void ObjectAddChild(gool_object parent, gool_object child)
    {
        if (parent == null || child == null)
            return;

        child.process.links.parent = parent;
        child.process.links.sibling = parent.process.links.children;
        parent.process.links.children = child;
    }
    static public void ObjectInit(gool_object obj, int exec, int subtype, int argc, int argv)
    {
        gool_object parent;
        vectors vectors;
        header header;
        state_maps maps;
        Entry global;
        EID p_eid;
        int idx_states, state;

        parent = obj.process.links.parent;
        obj.process.node = 0xFFFF;
        obj.process.pid_flags = 0;
        obj.process.entity = null;
        obj.process.path_progress = 0;
        obj.process.path_length = 0;
        obj.handle.type = 1;
        obj.process.vectors[0].x = 0;
        obj.process.vectors[0].y = 0;
        obj.process.vectors[0].z = 0;
        obj.process.vectors[1].y = 0;
        obj.process.vectors[1].x = 0;
        obj.process.vectors[1].z = 0;
        obj.process.vectors[2].x = 0;
        obj.process.vectors[2].y = 0;
        obj.process.vectors[2].z = 0;
        obj.process.speed = 0;
        obj.process.anim_frame = 0;
        obj.process.ack = 0;
        obj.process.status_a = 0;
        obj.process.status_c = 0;
        obj.process.status_b = 0;
        obj.process.invincibility_state = 0;
        obj.process.size = 0;
        obj.process.floor_impact_stamp = 0;
        obj.process.hotspot_size = 0;
        obj.process.voice_id = unchecked((uint)-2);

        if(parent is not null && parent.handle.type == 1)
        {
            obj.zone  = parent.zone;
            obj.vectors.trans = parent.vectors.trans;
            obj.vectors.rot   = parent.vectors.rot;
            obj.vectors.scale = parent.vectors.scale;
        }
        else
        {
            vectors = obj.vectors;
            obj.zone = null;
            obj.vectors.rot.y = 0;
            obj.vectors.rot.x = 0;
            obj.vectors.rot.z = 0;
            obj.vectors.scale.x = 0x1000;
            obj.vectors.scale.y = 0x1000;
            obj.vectors.scale.z = 0x1000;
        }
        p_eid = (uint)GLOBAL.nsd.GetEIDfromMap(exec);
        if (exec == 4 || exec == 5 || exec == 29) { obj.zone = null; }
        if (exec == 0)                            { obj.process.cam_zoom = 0; }
        global = GLOBAL.nsf.entryTable[(int)p_eid];
        obj.global = global;
        obj.process.links.self          = obj;
        obj.process.links.collider      = null;
        obj.process.links.creator       = null;
        obj.process.links.interrupter   = null;
        global = obj.global;
        obj.handle.subtype = subtype;
        obj.process.anim_seq = null;
        obj.process.once_p   = 0;
        obj.process.links.player = player;
        header = new header(obj.global.ExtractItem(0));
        maps = new state_maps(obj.global.ExtractItem(3));
        idx_states = (int)header.subtype_map_idx;
        state = maps.map[idx_states + subtype];

        if(state == 0xFF)
            Debug.LogWarning($"On ObjectInit {subtype}, invalid state");

    }


    static public void SendEvent(gool_object sender, gool_object recipient, uint evnt, int argc, uint argv)
    {
        throw new NotImplementedException();
    }

    static public void SendToColliders(gool_object sender, uint evnt, int type, int argc, uint argv)
    {
        throw new NotImplementedException();
    }

    static public void UpdateObjects(int flags)
    {
        throw new NotImplementedException();
    }

    static public void InitLevelSpawns()
    {
        ushort[] levelSpawn =  new ushort[gool.SPAWN_COUNT];
        levelSpawn = level_spawns;

        for (int i = 0; levelSpawn[i] != 0; i++)
        {
            ushort value = levelSpawn[i];

            int packedLevel = value >> 9;
            int spawnIndex = value & 0x1FF;

            if (packedLevel == GLOBAL.nsd.levelID)
            {
                spawns[spawnIndex] |= 8;
            }
        }
    }
}

