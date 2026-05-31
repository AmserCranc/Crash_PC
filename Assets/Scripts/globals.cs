using System.Collections.Generic;
using UnityEngine;
using LID = System.UInt32;
using EID = System.UInt32;
using System;
using System.IO;
using static gool;
using static zoneData;

static public class GLOBAL
{
    static public Dictionary<int, int> texEIDMap;
    static public Material HUD;
    static public Material WGEO_material;
    static public Material DEMO;
    static public gool_object crash;
    static public Level level;

#region globals
    static public int          game_state;
    static public LID          current_lid_ro;
    static public uint         dword_80061890;
    static public int          screen_shake;
    static public int          fog_z;
    static public uint         next_display_flags;
    static public int          respawn_count;
    static public gool_object  fruit_hud;
    static public gool_object  life_hud;
    static public gool_object  ambiance_object;
    static public uint         current_display_flags;
    static public int          i_death_cam;
    static public int          dword_800618B8;
    static public gool_object  pause_obj;
    static public uint         dword_800618C0;
    static public gool_object  pickup_hud;
    static public int          cam_rot_xz_ro;
    static public gool_object  aku_mask;
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
    static public gool_object  camera_spin_object;
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
    static public gool_object  light_source_obj;
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
    static public gool_object  caption_obj;
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
    static public gool_object  previous_box;
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



