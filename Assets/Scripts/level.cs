using System;
using static zoneData;
using LID = System.UInt32;
using EID = System.UInt32;
using UnityEngine;
using System.Collections.Generic;
using static GLOBAL;
using static geom;
using static gool;
using static solid;

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
   //public rgb8         prev_vram_fill_color;   /* 80057964 */
   //public rgb8         vram_fill_color;        /* 80057968 */
   //public rgb8         next_vram_fill_color;   /* 8005796C */
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







    // public void Init()
    // {
    //     Entry zone_spawn;
    //     zone_path path_spawn;
    //     uint progress_spawn;
    //     zone_header header;
    //     int idx, ref_inc;

    //     GLOBAL.pbak_state = 0;
    //     GLOBAL.crash = null;
    //     GLOBAL.current_display_flags  = gool.FLAG_DISPLAY_WORLDS | gool.FLAG_DISPANIM_OBJECTS | gool.FLAG_CAM_UPDATE;
    //     GLOBAL.next_display_flags     = gool.FLAG_DISPLAY_WORLDS | gool.FLAG_DISPANIM_OBJECTS | gool.FLAG_CAM_UPDATE;
    //     GLOBAL.level.spawns           = new uint[gool.SPAWN_COUNT];
    //     InitLevelSpawns(GLOBAL.ldat.lid);
    //     progress_spawn   = 0;
    //     ref_inc = 2;

    //     if(ldat.lid == LevelID.Hog_Wild 
    //         && ldat.lid == LevelID.Whole_Hog)
    //     {
    //         ref_inc = 1;
    //     }
    //     //GfxInitMatrices();
    //     if(ldat.lid == LevelID.Map_Main_Menu_Title_Sequence 
    //         && GLOBAL.state == 0x200 
    //         && ldat_not_inited == 0)
    //     {
    //         zone_spawn = GLOBAL.ldat.parent.entries[NSStringToEID(title_zone_name)];
    //     }

    //     if(bonus_return >= 1)
    //     {
    //         ldat.zone_spawn      = savestate.zone;
    //         ldat.path_idx_spawn  = savestate.path_idx;
    //         progress_spawn       = savestate.progress;
    //         bonus_return         = 0;
    //     }

    //     zone_spawn = GLOBAL.ldat.parent.entries[ldat.zone_spawn]; 
        
    //     GLOBAL.crash_eid = EID_NONE;
    //     cur_zone = null;
    //     cur_path = null;
    //     cur_progress = 0;
    //     //vram_fill_color.r = 0;
    //     //vram_fill_color.g = 0;
    //     //vram_fill_color.b = 0;
    //     respawn_stamp = 0;

    //     if(GLOBAL.state == gool.GAME_STATE_TITLE)
    //         PbakChoose(GLOBAL.crash_eid);
        
    //     header = new zone_header(zone_spawn.ExtractItem(0));
    //     idx = (int)(header.paths_idx + ldat.path_idx_spawn);
    //     path_spawn = zone_spawn.ExtractItem(idx);

    //     LevelUpdate(zone_spawn, path_spawn, (int)progress_spawn, 0);

    //     //ShaderParamsUpdate(1);
    //     //ShaderParamsUpdateRipple(1);

    //     ldat_not_inited = 0;       
    // }

    // public void InitLevelSpawns(LID lid)
    // {
    //     throw new NotImplementedException("init level spawns");
    // }

    // public EID NSStringToEID(char[] name)
    // {
    //     throw new NotImplementedException("NSStringToEID");
    // }

    // public void PbakChoose(EID pbak)
    // {
    //     throw new NotImplementedException("playback chooser");
    // }

    public void LevelUpdate(Entry zone, zone_path path, int progress, uint flags)
    {
        
    }

    public void InitLevelGlobals()
    {
        current_map_level    = 99;
        saved_title_state    = -1;
        title_state          = 7;
        sfx_vol              = 255;
        mus_vol              = 255;
        init_life_count      = 4 << 8;
        dword_800618E0       = 0;
        dword_8006190C       = 0;
        mono                 = 0;
        level_count          = 1;
        debug                = 0;
        dword_80061A40       = 0;
        options_changed      = 0;
        game_state           = 0;
        saved_item_pool1     = 0;
        saved_item_pool2     = 0;
        saved_level_count    = 1;
        LevelResetGlobals(1);
    }
    public void LevelResetGlobals(int flag)
    {
        if(flag != 1)
            return;

        checkpoint_id       = -1;
        death_count         = 0;
        respawn_count       = 0;
        health              = 0;
        fruit_count         = 0;
        cortex_count        = 0;
        brio_count          = 0;
        tawna_count         = 0;
        levels_unlocked     = 1;
        item_pool1          = 0;
        item_pool2          = 0;
        is_first_zone       = 1;
        current_map_level   = 99;
        level_count         = 1;
        saved_item_pool1    = 0;
        saved_item_pool2    = 0;
        saved_level_count   = 1;
        life_count          = init_life_count;

        levelState.level_spawns = new ushort[gool.LEVEL_SPAWN_COUNT];

    }
    public void InitMisc(int flag) 
    {
        switch (GLOBAL.ldat.lid) {
        case 5:
            if (flag >= 1)
            ObjectCreate(handles[4].self, 9, 4, 0, 0, 1);
            break;
        case 0xE:
            break;
        case 0x14:
        case 0x16:
            if (flag >= 1)
            ObjectCreate(handles[4].self, 23, 6, 0, 0, 1);
            break;
        case 0x17:
            if (flag >= 1)
            ambiance_object = ObjectCreate(handles[4].self, 39, 4, 0, 0, 1);
            break;
        case 0x22:
        case 0x2E:
            if (flag >= 1)
            ObjectCreate(handles[4].self, 53, 13, 0, 0, 1);
            break;
        case 0x28:
        case 0x2A:
            ShaderParamsUpdate(1);
            break;
        default:
            ambiance_object = null;
            break;
        }
        is_first_zone = 1;
        box_count = 0;
        gem_stamp = 0;
        island_cam_state = 0;
        if (pbak_state != 2)
            caption_obj = 0;
        title_pause_state = 0;
        TransSmoothStopAtSolid(0, 0, 0);
        fade_step = 32;
        cur_zone_query.once = 0;
        if (game_state != GAME_STATE_TITLE)
            fade_counter = 288;
        }
}