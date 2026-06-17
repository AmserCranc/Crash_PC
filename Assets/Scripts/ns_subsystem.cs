using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ns_subsystem
{
    public string subname;
    public Action init;
    public Action init2;
    public Action on_load;
    public Action unused;
    public Action kill;
}

static public class NS_subsystems
{
    static readonly char[] alpha_map = new char[] 
                                {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
                                'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
                                'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
                                'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                                'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                                'Y', 'Z', '_', '!' };




    static public int NSStringToEID(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        uint eid = 0;

        for (int c = 0; c < 5; c++)
        {
            eid <<= 6;

            if (c >= name.Length)
                continue;

            int index = Array.IndexOf(alpha_map, name[c]);

            if (index < 0)
                continue;

            eid |= (uint)index;
        }

        return (int)((eid << 1) | 1u);
    }

    static public void SLSTInit()
    {
        Debug.Log("SLST init");
    }

    static public void TitleInit()
    {
        if(GLOBAL.current_lid_ro != LevelID.Map_Main_Menu_Title_Sequence)
            return;
        
        Title.titleStruct = new Title.Title_s();
    }

    static public void LDATInit()
    {
        int ldat_not_inited = 1;

        Entry zone_spawn;
        zoneData.zone_path path_spawn;
        uint progress_spawn;
        zoneData.zone_header header;
        int idx, ref_inc;

        GLOBAL.pbak_state = 0;
        GLOBAL.crash = null;
        GLOBAL.current_display_flags  = gool.FLAG_DISPLAY_WORLDS | gool.FLAG_DISPANIM_OBJECTS | gool.FLAG_CAM_UPDATE;
        GLOBAL.next_display_flags     = gool.FLAG_DISPLAY_WORLDS | gool.FLAG_DISPANIM_OBJECTS | gool.FLAG_CAM_UPDATE;
        Level.levelState              = new Level.level_state();
        Level.levelState.spawns       = new uint[gool.SPAWN_COUNT];
        gool.InitLevelSpawns();
        progress_spawn                = 0;
        ref_inc                       = 2;

        if(GLOBAL.nsd.levelID == LevelID.Hog_Wild 
            && GLOBAL.nsd.levelID == LevelID.Whole_Hog)
        {
            ref_inc = 1;
        }
        if(GLOBAL.nsd.levelID == LevelID.Map_Main_Menu_Title_Sequence 
            && GLOBAL.state == gool.GAME_STATE_GAMEOVER 
            && ldat_not_inited == 0)
        {
            zone_spawn = GLOBAL.nsf.entryTable[NSStringToEID("0b_pz")];
        }

        if(Level.bonus_return >= 1)
        {
            Level.zone_spawn        = Level.savestate.zone;
            Level.path_idx_spawn    = Level.savestate.path_idx;
            progress_spawn          = Level.savestate.progress;
            Level.bonus_return      = 0;
        }

        zone_spawn = GLOBAL.nsf.entryTable[GLOBAL.nsd.levelStartZoneEID]; 
        
        GLOBAL.crash_eid = gool.EID_NONE;
        Level.cur_zone = null;
        Level.cur_path = null;
        Level.cur_progress = 0;
        //vram_fill_color.r = 0;
        //vram_fill_color.g = 0; //This might be to set default camera colour
        //vram_fill_color.b = 0;
        Level.respawn_stamp = 0;

        if(GLOBAL.state == gool.GAME_STATE_TITLE)
            Playback.PbakChoose(GLOBAL.crash_eid);
        
        header = new zoneData.zone_header(zone_spawn.ExtractItem(0));
        idx = (int)(header.paths_idx + Level.path_idx_spawn);
        path_spawn = new zoneData.zone_path(zone_spawn.ExtractItem(idx));

        Level.LevelUpdate(zone_spawn, path_spawn, (int)progress_spawn, 0);

        //ShaderParamsUpdate(1);
        //ShaderParamsUpdateRipple(1);

        ldat_not_inited = 0;       
        

    }

    static public void PbakInit()
    {
        Debug.Log("PbakInit");
    }

    static public void InitLID()
    {
        Debug.Log("InitLID");
    }

    static public void TitleLoadNextState()
    {
        Debug.Log("TitleLoadNextState");
    }

    static public void TGEOOnLoad()
    {
        Debug.Log("TGEOOnLoad");
    }

    static public void ZDATOnLoad()
    {
        Debug.Log("ZDATOnLoad");
    }

    static public void MDATOnLoad()
    {
        Debug.Log("MDATOnLoad");
    }

    static public void SLSTKill()
    {
        
    }

    static public void TitleKill()
    {
        
    }

    static public void PBAKKill()
    {
        
    }
}
