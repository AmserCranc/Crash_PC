using System.Collections.Generic;
using UnityEngine;
using LID = System.UInt32;
using EID = System.UInt32;
using System;
using System.IO;

static public class GLOBAL
{
    static public Dictionary<int, int> texEIDMap;
    static public Material HUD;
    static public Material WGEO_material;
    static public Material DEMO;
    static public GameState gamestate;
    static public GameObject crash;

#region globals
    static public int          state;
    static public LID          current_lid_ro;
    static public uint         dword_80061890;
    static public int          screen_shake;
    static public int          fog_z;
    static public uint         next_display_flags;
    static public int          respawn_count;
    static public GameObject   fruit_hud;
    static public GameObject   life_hud;
    static public GameObject   ambiance_object;
    static public uint         current_display_flags;
    static public int          i_death_cam;
    static public int          dword_800618B8;
    static public GameObject   pause_obj;
    static public uint         dword_800618C0;
    static public GameObject   pickup_hud;
    static public int          cam_rot_xz_ro;
    static public GameObject   aku_mask;
    static public int          state_flag;
    static public int          title_state;
    static public int          saved_title_state;
    static public int          current_map_level;
    static public uint         dword_800618E0;
    static public uint         dword_800618E4;
    static public uint         dword_800618E8;
    static public int          life_count;
    static public uint         health;
    static public int          fruit_count;
    static public int          cortex_count;
    static public int          brio_count;
    static public int          tawna_count;
    static public uint         current_zone_flags_ro;
    static public int          init_life_count;
    static public uint         dword_8006190C;
    static public int          mono;
    static public uint         sfx_vol;
    static public uint         mus_vol;
    static public GameObject   camera_spin_object;
    static public Vector3      camera_trans_ro;
    static public Vector3      camera_rotation_ro;
    static public uint         ticks_current_frame;
    static public Vector2      screen_ro;
    static public int          level_count;
    static public int          levels_unlocked;
    static public uint         dword_8006194C;
    static public int          camera_spin_object_vert;
    static public uint         dword_80061954; 
    static public uint         dword_80061958;
    static public uint         dword_8006195C;
    static public uint         dword_80061960;
    static public GameObject   light_source_obj;
    static public uint         dword_80061968;
    static public int          dcam_zoom_speed;
    static public int          dcam_flip_speed;
    static public uint         percent_complete;
    static public uint         card_flags_ro;
    static public int          bonus_round;
    static public int          card_part_count_ro;
    static public int          box_count;
    static public uint         item_pool1;
    static public uint         island_cam_rot_x;
    static public uint         gem_stamp;
    static public int          island_cam_state;
    static public uint         is_first_zone;
    static public int          debug;
    static public int          checkpoint_id;
    static public uint         dword_000619A4;
    static public uint         dword_800619A8;
    static public uint         item_pool2;
    static public uint         map_level_links;
    static public int          title_pause_state;
    static public uint         map_key_links;
    static public GameObject   caption_obj;
    static public uint         dword_800619C0;
    static public uint         dword_800619C4;
    static public int          draw_count_ro;
    static public char         card_string;
    static public Texture2D    card_icon;
    static public uint[]       card_partinfos;
    static public int          gem_count;
    static public int          key_count;
    static public uint         dword_80061A18;
    static public uint         saved_item_pool1;
    static public uint         saved_item_pool2;
    static public Vector3      spawn_trans;
    static public int          pbak_state;
    static public int          fade_counter;
    static public int          fade_step;
    static public int          death_count;
    static public uint         dword_80061A40;
    static public uint         dword_80061A44;
    static public uint         dword_80061A48;
    static public uint         dword_80061A4C;
    static public int          saved_level_count;
    static public uint         dword_80061A54;
    static public int          options_changed;
    static public GameObject   previous_box;
    static public uint         boxes_y;
    static public zone_entity  previous_box_entity;

    static public EID          crash_eid = 0;
#endregion

    

#region NS data
    static public LDAT ldat; //Disc data

#endregion
}

// public class BGM
// {
//     public List<INST> instruments;
//     public List<MIDI> sequences;

//     public BGM()
//     {
//         instruments = new();
//         sequences = new();
//     }
// }

public class GameState
{
    public Level level;


    public class Level
    {
        public const int EID_NONE = 0x6396347F;

#region .sdata
        public int      bonus_return    = 0;
        public int      ldat_not_inited = 1;
        public char[]   title_zone_name = "0b_pZ".ToCharArray();
        public int      first_spawn     = 0;
#endregion
#region .bss
        public Entry        cur_zone, obj_zone;     /* 80057914, 80057918 */
        public zone_path    cur_path;               /* 8005791C */
        public int          cur_progress;           /* 80057920 */
        public Vector3      unk_80057924;           /* 80057924 */
        public int          cam_rot_xz;             /* 80057930 */
        public Vector3      cam_rot_before;         /* 80057934 */
        public Vector3      cam_rot_after;          /* 80057940 */
        public Vector3      cam_rot_xz_dir;         /* 8005794C */
        public uint         unk_80057958;           /* 80057958 */
        public uint         unk_8005795C;           /* 8005795C */
        public int          draw_count;             /* 80057960 */
        public rgb8         prev_vram_fill_color;   /* 80057964 */
        public rgb8         vram_fill_color;        /* 80057968 */
        public rgb8         next_vram_fill_color;   /* 8005796C */
        public uint         respawn_stamp;          /* 80057970 */
        public level_state  savestate;              /* 80057974 */
#endregion

        public level_state  levelState;
        public nsd_ldat     ldat;

        public class level_state
        {
            public Vector3 player_trans, player_rot, player_scale;
            public EID zone;
            public int path_idx;
            public uint progress;
            public LID lid;
            public int flag;
            public ushort[] level_spawns;
            public uint[] spawns;
            public int box_count;
        }

        public class nsd_ldat
        {
            public uint     magic;         
            public LID      lid;           
            public EID      zone_spawn ;   
            public int      path_idx_spawn;
            public uint     fov;    
            public EID[]    exec_map;

            public nsd_ldat()
            {
                magic           = GLOBAL.ldat.magic;
                lid             = GLOBAL.ldat.lid;
                zone_spawn      = GLOBAL.ldat.zone_spawn;
                path_idx_spawn  = GLOBAL.ldat.path_idx_spawn;
                fov             = GLOBAL.ldat.fov;
                exec_map        = GLOBAL.ldat.exec_map;
            }
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
                
            }
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
            //public rgb8     unknown_h;
            public byte     unused_a;
            //public rgb8     vram_fill;
            public byte     unused_b;
            //public rgb8     far_color;
            public uint     unused_c;
            //public gool_colors object_colors;
            //public gool_colors player_colors;
        }





        public void Init()
        {
            Entry zone_spawn;
            ZonePath path_spawn;
            uint progress_spawn;
            zone_header header;
            int idx, ref_inc;

            GLOBAL.pbak_state = 0;
            GLOBAL.crash = null;
            GLOBAL.current_display_flags  = gool.FLAG_DISPLAY_WORLDS | gool.FLAG_DISPANIM_OBJECTS | gool.FLAG_CAM_UPDATE;
            GLOBAL.next_display_flags     = gool.FLAG_DISPLAY_WORLDS | gool.FLAG_DISPANIM_OBJECTS | gool.FLAG_CAM_UPDATE;
            GLOBAL.level.spawns           = new uint[gool.SPAWN_COUNT];
            InitLevelSpawns(GLOBAL.ldat.lid);
            progress_spawn   = 0;
            ref_inc = 2;

            if(ldat.lid == LevelID.Hog_Wild 
                && ldat.lid == LevelID.Whole_Hog)
            {
                ref_inc = 1;
            }
            //GfxInitMatrices();
            if(ldat.lid == LevelID.Map_Main_Menu_Title_Sequence 
                && GLOBAL.state == 0x200 
                && ldat_not_inited == 0)
            {
                zone_spawn = GLOBAL.ldat.parent.entries[NSStringToEID(title_zone_name)];
            }

            if(bonus_return >= 1)
            {
                ldat.zone_spawn      = savestate.zone;
                ldat.path_idx_spawn  = savestate.path_idx;
                progress_spawn       = savestate.progress;
                bonus_return         = 0;
            }

            zone_spawn = GLOBAL.ldat.parent.entries[ldat.zone_spawn]; 
            
            GLOBAL.crash_eid = EID_NONE;
            cur_zone = null;
            cur_path = null;
            cur_progress = 0;
            vram_fill_color.r = 0;
            vram_fill_color.g = 0;
            vram_fill_color.b = 0;
            respawn_stamp = 0;

            if(GLOBAL.state == GAME_STATE_TITLE)
                PbakChoose(GLOBAL.crash_eid);
            
            header = new zone_header(zone_spawn.ExtractItem(0));
            idx =

            

        }

        public void InitLevelSpawns(LID lid)
        {
            throw new NotImplementedException("init level spawns");
        }

        public EID NSStringToEID(char[] name)
        {
            throw new NotImplementedException("NSStringToEID");
        }

        public void PbakChoose(EID pbak)
        {
            throw new NotImplementedException("playback chooser");
        }

    }
}

