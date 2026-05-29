using System.Collections.Generic;
using UnityEngine;
using LID = System.UInt32;
using EID = System.UInt32;
using System;

static public class GLOBAL
{
    static public Dictionary<int, int> texEIDMap;
    static public Material HUD;
    static public Material WGEO_material;
    static public Material DEMO;
    static public GameState state;
    static public GameObject crash;

    

#region NS data
    //static public BGM bgm;
    static public LDAT ldat;

#endregion
}

public class BGM
{
    public List<INST> instruments;
    public List<MIDI> sequences;

    public BGM()
    {
        instruments = new();
        sequences = new();
    }
}

public class GameState
{
#region gool flags
    /* events */
    const uint GOOL_EVENT_JUMPED_ON       = 0;
    const uint GOOL_EVENT_HIT             = 0x300;
    const uint GOOL_EVENT_SPIN_HIT        = 0x400;
    const uint GOOL_EVENT_TRIGGERED       = 0x800;
    const uint GOOL_EVENT_FALL_KILL       = 0x900;
    const uint GOOL_EVENT_BOX_STACK_BREAK = 0x900;
    const uint GOOL_EVENT_HIT_INVINCIBLE  = 0xA00;
    const uint GOOL_EVENT_STATUS          = 0xF00;
    const uint GOOL_EVENT_COMBO           = 0x1000;
    const uint GOOL_EVENT_RESPAWN         = 0x1300;
    const uint GOOL_EVENT_EAT             = 0x1400;
    const uint GOOL_EVENT_BOUNCE          = 0x1500;
    const uint GOOL_EVENT_WARP            = 0x1600;
    const uint GOOL_EVENT_SQUASH          = 0x1900;
    const uint GOOL_EVENT_TERMINATE       = 0x1A00;
    const uint GOOL_EVENT_WIN_BOSS        = 0x1D00;
    const uint GOOL_EVENT_FLING           = 0x1D00;
    const uint GOOL_EVENT_EXPLODE         = 0x1E00;
    const uint GOOL_EVENT_BURN            = 0x1F00;
    const uint GOOL_EVENT_DROWN           = 0x2100;
    const uint GOOL_EVENT_SHOCK           = 0x2300;
    const uint GOOL_EVENT_HIT_FENCE       = 0x2300;
    const uint GOOL_EVENT_BOULDER_SQUASH  = 0x2500;
    const uint GOOL_EVENT_LEVEL_END       = 0x2900;
    const uint GOOL_EVENT_PLAYER_DAMAGE   = 0x2B00;

    /* status a flags */
    const uint GOOL_FLAG_GROUNDLAND        = 0x1;
    const uint GOOL_FLAG_TOWARD_GOAL       = 0x4;
    const uint GOOL_FLAG_TROT_DIR          = 0x8;
    const uint GOOL_FLAG_CHANGE_PATH_DIR   = 0x10;
    const uint GOOL_FLAG_FIRST_FRAME       = 0x20;
    const uint GOOL_FLAG_HIT_AT_BOTTOM     = 0x80;
    const uint GOOL_FLAG_INVALID_PATH      = 0x200;
    const uint GOOL_FLAG_DYING             = 0x400;
    const uint GOOL_FLAG_REACHED_TROT      = 0x800;
    const uint GOOL_FLAG_PLAYER_D_COLLIDER = 0x1000;
    const uint GOOL_FLAG_LBOUND_INVALID    = 0x8000;
    const uint GOOL_FLAG_KEEP_EVENT_STACK  = 0x20000;
    const uint GOOL_FLAG_ON_FLOOR          = 0x40000;
    const uint GOOL_FLAG_ATOP_OBJECT       = 0x200000;

    /* status b flags */
    const uint GOOL_FLAG_ROT_Y             = 0x1;
    const uint GOOL_FLAG_TRACK_PATH_ROT    = 0x2;
    const uint GOOL_FLAG_TRACK_PATH_SIGN   = 0x4;
    const uint GOOL_FLAG_STOPPED_BY_SOLID  = 0x8;
    const uint GOOL_FLAG_COLLIDABLE        = 0x10;
    const uint GOOL_FLAG_COLLIDEABLE       = 0x10;
    const uint GOOL_FLAG_GRAVITY           = 0x20;
    const uint GOOL_FLAG_TRANS_MOTION      = 0x40;
    const uint GOOL_FLAG_DPAD_CONTROL      = 0x80;
    const uint GOOL_FLAG_INVISIBLE         = 0x100;
    const uint GOOL_FLAG_2D                = 0x200;
    const uint GOOL_FLAG_ROT_X             = 0x2000;
    const uint GOOL_FLAG_SOLID_GROUND      = 0x4000;
    const uint GOOL_FLAG_ORIENT_ON_PATH    = 0x8000;
    const uint GOOL_FLAG_SOLID_SIDES       = 0x10000;
    const uint GOOL_FLAG_SOLID_TOP         = 0x20000;
    const uint GOOL_FLAG_ROT_Y2            = 0x80000;
    const uint GOOL_FLAG_FORCE_UPDATE      = 0x2000000;
    const uint GOOL_FLAG_HAS_SHADOW        = 0x4000000;
    const uint GOOL_FLAG_STRING_CENTER     = 0x4000000;
    const uint GOOL_FLAG_SOLID_BOTTOM      = 0x8000000;
    const uint GOOL_FLAG_STALL             = 0x10000000;
    const uint GOOL_FLAG_PAUSED            = 0x20000000;  /* GOOL_DEBUG extension only */
    const uint GOOL_FLAG_STEP              = 0x40000000;  /* GOOL_DEBUG extension only */

    /* state/status c flags */
    const uint GOOL_FLAG_GROUND_STATE      = 0x4;
    const uint GOOL_FLAG_AIR_STATE         = 0x8;
    const uint GOOL_FLAG_FLING_STATE       = 0x10;  /* rotate state? */
    const uint GOOL_FLAG_COLOVERRIDE_STATE = 0x800;
    const uint GOOL_FLAG_MENUTEXT_STATE    = 0x20000;
    const uint GOOL_FLAG_NOTERM_STATE      = 0x40000;
    const uint GOOL_FLAG_EXPENDABLE_STATE  = 0x80000;

    /* spawn flags */
    const uint GOOL_FLAG_SPAWNED           = 0x1;
    const uint GOOL_FLAG_ACTIVE_SPAWN      = 0x8;

    /* display flags */
    const uint GOOL_FLAG_DISPLAY_WORLDS    = 0x1;
    const uint GOOL_FLAG_CAM_UPDATE        = 0x2;
    const uint GOOL_FLAG_DISPLAY           = 0x4;
    const uint GOOL_FLAG_ANIMATE           = 0x8;
    const uint GOOL_FLAG_DISPLAY_C1        = 0x10;
    const uint GOOL_FLAG_ANIMATE_C1        = 0x20;
    const uint GOOL_FLAG_DISPLAY_C356      = 0x40;
    const uint GOOL_FLAG_ANIMATE_C356      = 0x80;
    const uint GOOL_FLAG_ANIMATE_C2        = 0x100;
    const uint GOOL_FLAG_DISPLAY_C2        = 0x200;
    const uint GOOL_FLAG_ANIMATE_C4        = 0x400;
    const uint GOOL_FLAG_DISPLAY_C4        = 0x800;
    const uint GOOL_FLAG_COUNT_DRAWS       = 0x1000;
    const uint GOOL_FLAG_DISPLAY_SOLID_BGC = 0x2000;
    const uint GOOL_FLAG_FORCE_DISP_MENUS  = 0x4000;
    const uint GOOL_FLAG_FORCE_ANIM_MENUS  = 0x8000;
    const uint GOOL_FLAG_SPIN_DEATH        = 0x10000;
    const uint GOOL_FLAG_DISPLAY_IMAGES    = 0x20000;
    const uint GOOL_FLAG_SPIN_ACCEL        = 0x40000;
    const uint GOOL_FLAG_DISPLAY_UNK       = 0x200000;

    const uint GOOL_FLAG_DISPANIM_ALL      = 0x3C00F;
    const uint GOOL_FLAG_DISPANIM_OBJECTS  = 0xFFFC;


    /* interpreter flags */
    const uint GOOL_FLAG_SUSPEND_ON_RET    = 0x1;
    const uint GOOL_FLAG_SUSPEND_ON_RETLNK = 0x2;
    const uint GOOL_FLAG_SUSPEND_ON_ANIM   = 0x4;
    const uint GOOL_FLAG_EVENT_SERVICE     = 0x8;
    const uint GOOL_FLAG_STATUS_PRESERVE   = 0x10;
    const uint GOOL_FLAG_RETURN_EVENT      = 0x20;

    /* constants */
    const uint GOOL_OBJECT_COUNT           = 96;
    const uint GOOL_LEVEL_SPAWN_COUNT      = 3592;
    const uint GOOL_SPAWN_COUNT            = 304;
#endregion

    public Level level;
#region globals
    public int          state;
    public LID          current_lid_ro;
    public uint         dword_80061890;
    public int          screen_shake;
    public int          fog_z;
    public uint         next_display_flags;
    public int          respawn_count;
    public GameObject   fruit_hud;
    public GameObject   life_hud;
    public GameObject   ambiance_object;
    public uint         current_display_flags;
    public int          i_death_cam;
    public int          dword_800618B8;
    public GameObject   pause_obj;
    public uint         dword_800618C0;
    public GameObject   pickup_hud;
    public int          cam_rot_xz_ro;
    public GameObject   aku_mask;
    public int          state_flag;
    public int          title_state;
    public int          saved_title_state;
    public int          current_map_level;
    public uint         dword_800618E0;
    public uint         dword_800618E4;
    public uint         dword_800618E8;
    public int          life_count;
    public uint         health;
    public int          fruit_count;
    public int          cortex_count;
    public int          brio_count;
    public int          tawna_count;
    public uint         current_zone_flags_ro;
    public int          init_life_count;
    public uint         dword_8006190C;
    public int          mono;
    public uint         sfx_vol;
    public uint         mus_vol;
    public GameObject   camera_spin_object;
    public Vector3      camera_trans_ro;
    public Vector3      camera_rotation_ro;
    public uint         ticks_current_frame;
    public Vector2      screen_ro;
    public int          level_count;
    public int          levels_unlocked;
    public uint         dword_8006194C;
    public int          camera_spin_object_vert;
    public uint         dword_80061954; 
    public uint         dword_80061958;
    public uint         dword_8006195C;
    public uint         dword_80061960;
    public GameObject   light_source_obj;
    public uint         dword_80061968;
    public int          dcam_zoom_speed;
    public int          dcam_flip_speed;
    public uint         percent_complete;
    public uint         card_flags_ro;
    public int          bonus_round;
    public int          card_part_count_ro;
    public int          box_count;
    public uint         item_pool1;
    public uint         island_cam_rot_x;
    public uint         gem_stamp;
    public int          island_cam_state;
    public uint         is_first_zone;
    public int          debug;
    public int          checkpoint_id;
    public uint         dword_000619A4;
    public uint         dword_800619A8;
    public uint         item_pool2;
    public uint         map_level_links;
    public int          title_pause_state;
    public uint         map_key_links;
    public GameObject   caption_obj;
    public uint         dword_800619C0;
    public uint         dword_800619C4;
    public int          draw_count_ro;
    public char         card_string;
    public Texture2D    card_icon;
    public uint[]       card_partinfos;
    public int          gem_count;
    public int          key_count;
    public uint         dword_80061A18;
    public uint         saved_item_pool1;
    public uint         saved_item_pool2;
    public Vector3      spawn_trans;
    public int          pbak_state;
    public int          fade_counter;
    public int          fade_step;
    public int          death_count;
    public uint         dword_80061A40;
    public uint         dword_80061A44;
    public uint         dword_80061A48;
    public uint         dword_80061A4C;
    public int          saved_level_count;
    public uint         dword_80061A54;
    public int          options_changed;
    public GameObject   previous_box;
    public uint         boxes_y;
    public zone_entity  previous_box_entity;
#endregion

    public class Level
    {
#region level state
        public Vector3 player_trans, player_rot, player_scale;
        public EID zone;
        public int path_idx;
        public uint progress;
        public LID lid;
        public int flag;
        public ushort[] level_spawns;
        public uint[] spawns;
        public int box_count;
#endregion

        public Entry zone_spawn;
        public ZonePath path_spawn;
        public uint progress_spawn;
        public ZoneHeader zone_header;
        int idx, ref_inc;

        public void Init()
        {
            GLOBAL.state.pbak_state = 0;
            GLOBAL.crash = null;
            GLOBAL.state.current_display_flags  = GOOL_FLAG_DISPLAY_WORLDS | GOOL_FLAG_DISPANIM_OBJECTS | GOOL_FLAG_CAM_UPDATE;
            GLOBAL.state.next_display_flags     = GOOL_FLAG_DISPLAY_WORLDS | GOOL_FLAG_DISPANIM_OBJECTS | GOOL_FLAG_CAM_UPDATE;
            GLOBAL.state.level.spawns           = new uint[GOOL_SPAWN_COUNT];
            InitLevelSpawns(GLOBAL.ldat.lid);
            GLOBAL.state.level.progress_spawn   = 0;
            ref_inc = 2;

            if(GLOBAL.ldat.lid == LID_HOGWILD && GLOBAL.ldat.lid == LID_WHOLEHOG)
                ref_inc = 1;
            //GfxInitMatrices();
            if(GLOBAL.ldat.lid == LID_TITLE && GLOBAL.state.state == 0x200 && ldat_not)

        }

        public void InitLevelSpawns(LID lid)
        {
            throw new NotImplementedException("init level spawns");
        }

    }
}

