using System;
using static zoneData;
using LID = System.Int32;
using EID = System.UInt32;
using UnityEngine;
using System.Collections.Generic;
using static GLOBAL;
using static geom;
using static gool;
using static solid;

static public class Level
{
    public const int EID_NONE = 0x6396347F;

#region .sdata
    static public int      bonus_return    = 0;
    static public int      ldat_not_inited = 1;
    static public char[]   title_zone_name = "0b_pZ".ToCharArray();
    static public int      first_spawn     = 0;
#endregion
#region .bss
    static public Entry        cur_zone, obj_zone;     /* 80057914, 80057918 */
    static public zone_path    cur_path;               /* 8005791C */
    static public int          cur_progress;           /* 80057920 */
    static public Vector3      unk_80057924;           /* 80057924 */
    static public int          cam_rot_xz;             /* 80057930 */
    static public Quaternion   cam_rot_before;         /* 80057934 */
    static public Quaternion   cam_rot_after;          /* 80057940 */
    static public Quaternion   cam_rot_xz_dir;         /* 8005794C */
    static public uint         unk_80057958;           /* 80057958 */
    static public uint         unk_8005795C;           /* 8005795C */
    static public int          draw_count;             /* 80057960 */
    static public Color        prev_vram_fill_color;   /* 80057964 */
    static public Color        vram_fill_color;        /* 80057968 */
    static public Color        next_vram_fill_color;   /* 8005796C */
    static public uint         respawn_stamp;          /* 80057970 */
    static public level_state  savestate;              /* 80057974 */

    static public int current_lid;
    static public int next_lid;
    static public int lid;
    static public int update_pend;
    static public uint zone_spawn;
    static public int path_idx_spawn;
    static public int cur_zone_flags_ro;
#endregion

    static public level_state  levelState;

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

    static public void LevelUpdate(Entry zone, zone_path path, int progress, uint flags)
    {
        //Skipping the SLST and loading

        if(zone == null || path == null)
        {
            Level.update_pend = 0;
            return;
        }

        int pathLength = path.length << 8;
        try {progress = Math.Clamp(progress, 0, pathLength -1);}
        catch{ progress = 0;}
        int ptIndex = progress >> 8;
        int currentPtIndex = cur_progress >> 8;

        bool change =
            zone != cur_zone ||
            path != cur_path ||
            ptIndex != currentPtIndex;

        if(change)
        {
            if(zone != cur_zone)
            {
                bool neighbourFlag = true;
                item_pool1 = 0;
                zone_header previousHeader = null;

                if(game_state == GAME_STATE_TITLE)
                    neighbourFlag = (flags & 2) != 0;

                if(cur_zone != null)
                {
                    obj_zone = zone;
                    neighbourFlag = (flags & 2) != 0;
                    previousHeader = new zone_header(cur_zone.ExtractItem(0));
                    ZoneTerminateDifference(zone);
                }

                if((flags & 1) != 0)
                    for(int i = 0; i < spawns.Length; i++)
                        spawns[i] &= 0xFFFFFFF9;

                cur_zone = zone;
                cur_path = path;
                cur_progress = progress;

                zone_header header = new zone_header(zone.ExtractItem(0));

                foreach(var neighbour in header.neighbours)
                {
                    if(neighbour == 0)
                        continue;

                    zone_header neighbourHeader = new zone_header(nsf.entryTable[(int)neighbour].ExtractItem(0));

                    if((neighbourHeader.display_flags & 1) == 0)
                    {
                        previous_box = null;
                        previous_box_entity = null;
                        boxes_y = 0x19000;

                        neighbourHeader.display_flags |= 3;
                    }

                    if(neighbourFlag)
                        neighbourHeader.display_flags |= 4;
                    else
                        neighbourHeader.display_flags &= ~4u;
                }

               // LevelUpdateMisc(header.gfx, (int)flags);
            }
            else
            {
                cur_zone = zone;
                cur_path = path;
                cur_progress = progress;
            }
        }
        else
        {
            if(progress == cur_progress)
            {
                Level.update_pend = 0;
                return;
            }

            cur_progress = progress;
        }

        Transform cam = Camera.main.transform;

        var pose = GoolCamera.ProgressToPose(cur_path, -cur_progress);

        cam_rot_before = cam.rotation;

        cam.position = pose.position;
        cam.rotation = pose.rotation;

        cam_rot_after = cam.rotation;

        Level.update_pend = 0;
    }

    static public void InitLevelGlobals()
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
    static public void LevelResetGlobals(int flag)
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

        level_spawns = new ushort[gool.LEVEL_SPAWN_COUNT];

    }
    static public void InitMisc(int flag) 
    {
        switch (GLOBAL.nsd.levelID) 
        {
            case 5:
                if (flag >= 1)
                ObjectCreate(GLOBAL.objectCollections[4].GetComponent<gool_object>(), 9, 4, 0, 0, 1);
                break;
            case 0xE:
                break;
            case 0x14:
            case 0x16:
                if (flag >= 1)
                ObjectCreate(GLOBAL.objectCollections[4].GetComponent<gool_object>(), 23, 6, 0, 0, 1);
                break;
            case 0x17:
                if (flag >= 1)
                ambiance_object = ObjectCreate(GLOBAL.objectCollections[4].GetComponent<gool_object>(), 39, 4, 0, 0, 1);
                break;
            case 0x22:
            case 0x2E:
                if (flag >= 1)
                ObjectCreate(GLOBAL.objectCollections[4].GetComponent<gool_object>(), 53, 13, 0, 0, 1);
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
            caption_obj = null;
        title_pause_state = 0;
        //TransSmoothStopAtSolid(0, 0, 0);
        fade_step = 32;
        //cur_zone_query.once = 0;
        if (game_state != GAME_STATE_TITLE)
            fade_counter = 288;
        }

    static public void ShaderParamsUpdate(int i)
    {
        throw new NotImplementedException();
    }


    
    static public void SpawnObjects()
    {
        throw new NotImplementedException();
    }

    static public void Restart(Level.level_state savestate)
    {
        
    }

    static void ZoneTerminateDifference(Entry newZone)
    {
        if (cur_zone == null)
            return;

        zoneData.zone_header curHeader =
            new zone_header(cur_zone.ExtractItem(0));

        zoneData.zone_header newHeader =
            new zone_header(newZone.ExtractItem(0));

        HashSet<Entry> newNeighbours = new();

        for (int i = 0; i < newHeader.neighbour_count; i++)
        {
            var e = GLOBAL.nsf.entryTable[(int)newHeader.neighbours[i]];
            if (e != null)
                newNeighbours.Add(e);
        }

        // Compare against current zone neighbours
        for (int i = 0; i < curHeader.neighbour_count; i++)
        {
            var curNeighbour = GLOBAL.nsf.entryTable[(int)curHeader.neighbours[i]];
            if (curNeighbour == null)
                continue;

            if (newNeighbours.Contains(curNeighbour))
                continue;

            var nHeader = new zone_header(curNeighbour.ExtractItem(0));

            // If it was active, terminate it
            if ((nHeader.display_flags & 1) != 0)
            {
                GoolZoneObjectsTerminate(curNeighbour);

                // clear bits 1 & 2 (0x3)
                nHeader.display_flags &= ~3u;
            }
        }
    }

    static public void LevelUpdateMisc(zone_gfx gfx, int flags)
    {
        //respawn_stamp = ticks_elapsed;
        prev_vram_fill_color = vram_fill_color;
        next_vram_fill_color = gfx.unknown_h;
        cur_zone_flags_ro = (int)gfx.flags;
        // if (!(flags & 4))
        //     MidiSetStateStopped();
    }
}