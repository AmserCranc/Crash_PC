using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LID = System.Int32;
using static gool;
using static DemoPlayback;
using static Level;
using static Cam;
using static Title;

unsafe public class Bootstrap : MonoBehaviour
{
    public const string
        FILE_BASE     = "s00000";

    public const LID
        LID_BOOTLEVEL = 15;


    public GameObject chunkDisplay;
    public string streamLocation = "Assets/Streams/";

    public NSD levelHeader;
    public NSF levelData;

    public LID lid;
    int is_pause_lid, can_pause;

//Controls
    public InputSystem.Pad[] pads;
    private static readonly KeyCode[] AllKeyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));


    // .sdata
    int wgeom_disabled = 0,
        paused = 0,
        pause_status = 0,
        done = 0;
    // .sbss
    uint pause_stamp, pause_draw_stamp;

    int ticks_elapsed;

    void Start()
    {
        pads = new InputSystem.Pad[2];
        pads[0] = new InputSystem.Pad();
        GLOBAL.level.InitLevelGlobals();
        LoadLevel(LID_BOOTLEVEL);
        CreateCoreObjects();
        GLOBAL.level.InitMisc(1);

#region Chunk Visualiser
        List<Color> blocks = new();
        foreach(Chunk chunk in levelData.chunks)
            blocks.Add(chunk.disp);

        this.GetComponent<MeshRenderer>().material.SetInt("_ColourCount", blocks.Count);
        this.GetComponent<MeshRenderer>().material.SetColorArray("_Colours", blocks);
#endregion
    
    }

    void LoadLevel(LID level)
    {
        lid = level;
        Debug.Log($"Attempting load {FILE_BASE}{level}");
        levelHeader = new($"{streamLocation}{FILE_BASE}{level}.nsd");
        levelData   = new($"{streamLocation}{FILE_BASE}{level}.nsf");

    }

    void CreateCoreObjects()
    {
        GLOBAL.pause_obj = null;
        
        if(GLOBAL.current_lid_ro        != LevelID.Intro 
            && GLOBAL.current_lid_ro    != LevelID.Ending)
        {
            GLOBAL.life_hud     = ObjectCreate(GLOBAL.objectCollections[1].GetComponent<gool_object>(), 4, 0, 0, 0, 0);
            GLOBAL.fruit_hud    = ObjectCreate(GLOBAL.objectCollections[1].GetComponent<gool_object>(), 4, 1, 0, 0, 0);
            GLOBAL.pickup_hud   = ObjectCreate(GLOBAL.objectCollections[1].GetComponent<gool_object>(), 4, 5, 0, 0, 0);
        }
    }


    void FixedUpdate()
    {
        lid = GLOBAL.ldat.lid;
        is_pause_lid = (lid != LevelID.Map_Main_Menu_Title_Sequence && lid != LevelID.Level_Completion_Screen && lid != LevelID.Intro) ? 1 : 0; 
        can_pause = (GLOBAL.pbak_state == 0) && ((is_pause_lid > 0 && GLOBAL.title_pause_state != -1) || GLOBAL.title_pause_state > 0) ? 1 : 0;

//Handle pausing
        if ((pads[0].tapped & 0x800) != 0 && (can_pause == 1))
        {
            paused = 1 - paused;

            if (paused == 1)
            {
                if (GLOBAL.pause_obj == null)
                {
                    GLOBAL.pause_obj = ObjectCreate(GLOBAL.objectCollections[7].GetComponent<gool_object>(), 4, 4, 0, 0, 0);

                    if (GLOBAL.pause_obj != null)
                    {
                        pause_status = 1;
                        //Draw pause logo
                    }
                    else
                    {
                        pause_status = 0;
                        paused = 0;
                        GLOBAL.pause_obj = null;
                    }
                }
            }
            else if (GLOBAL.pause_obj != null) 
            { 
                uint arg = 0;
                SendEvent(null, GLOBAL.pause_obj.GetComponent<gool_object>(), 0xC00, 1, arg); /* send resume/kill? event to pause screen object */
                GLOBAL.pause_obj = null;
                pause_status = -1;
            }
        }
        else { pause_status = 0;}
//Step playback
        if(GLOBAL.crash && crash_eid != gool.EID_NONE) { PbakPlay(crash_eid);}
//Handle game return to menu
        if(GLOBAL.next_lid == -1 && lid != LevelID.Map_Main_Menu_Title_Sequence
            &&  (GLOBAL.game_state == GAME_STATE_GAMEOVER
                || GLOBAL.game_state == GAME_STATE_CONTINUE
                || GLOBAL.game_state == 0x400))
        { GLOBAL.next_lid = LevelID.Map_Main_Menu_Title_Sequence; }
//Handle bonus return
        if(GLOBAL.next_lid != -1) 
        {
            SendToColliders(null, EVENT_LEVEL_END, 0, 0, 0);
            if(GLOBAL.next_lid == -2)
            {
                lid = GLOBAL.level.savestate.lid;
                GLOBAL.level.bonus_return = 1;
                GLOBAL.bonus_return2 = 1;
            }
            else
            {
                lid = GLOBAL.next_lid;
                GLOBAL.level.bonus_return = 0;
                GLOBAL.bonus_return2 = 0;
            }

            levelData = null;
            levelHeader = null;
            paused = 0;
//Handle level transition
            if(lid == LevelID.Map_Main_Menu_Title_Sequence)
            {
                GLOBAL.respawn_count = 0;
                GLOBAL.death_count = 0;
                GLOBAL.cortex_count = 0;
                GLOBAL.brio_count = 0;
                GLOBAL.tawna_count = 0;
                GLOBAL.checkpoint_id = 0;
            }
            LoadLevel(lid);
            CreateCoreObjects();
            if(GLOBAL.bonus_return2 >= 0)
            {
                GLOBAL.next_lid = -2;
                GLOBAL.level.SpawnObjects();
                GLOBAL.next_lid = -1;
                GLOBAL.level.Restart(GLOBAL.level.savestate);
            }
            GLOBAL.level.bonus_return = 0;
        }
//Start level
        GLOBAL.level.SpawnObjects();
        if(paused == 0)
        {
            GLOBAL.zone_header = new zoneData.zone_header(GLOBAL.level.cur_zone.ExtractItem(0));
            if ((GLOBAL.zone_header.display_flags & (ZDAT.ZONE_FLAG_DARK2 | ZDAT.ZONE_FLAG_LIGHTNING)) != 0)
            { //handle level graphics 
            }
    
            if ((GLOBAL.zone_header.display_flags & ZDAT.ZONE_FLAG_RIPPLE) != 0)
            { //Handle ripple graphics
            }

            CamUpdate();
        }
        GLOBAL.zone_header = new zoneData.zone_header(GLOBAL.level.cur_zone.ExtractItem(0));
        if (((GLOBAL.current_display_flags & FLAG_DISPLAY_WORLDS) != 0) && (GLOBAL.zone_header.world_count != 0) && (wgeom_disabled == 0)) 
        {
            if ((GLOBAL.zone_header.display_flags & ZDAT.ZONE_FLAG_DARK2) != 0)
                Debug.LogWarning("Implement GfxTransformWorldsDark2(ot)");
            else if ((GLOBAL.zone_header.display_flags & ZDAT.ZONE_FLAG_FOG_LIGHTNING) == ZDAT.ZONE_FLAG_FOG_LIGHTNING)
                Debug.LogWarning("GfxTransformWorldsDark(ot)");
            else if ((GLOBAL.zone_header.display_flags & ZDAT.ZONE_FLAG_FOG) != 0)
                Debug.LogWarning("GfxTransformWorldsFog(ot)");
            else if ((GLOBAL.zone_header.display_flags & ZDAT.ZONE_FLAG_RIPPLE) != 0)
                Debug.LogWarning("GfxTransformWorldsRipple(ot)");
            else if ((GLOBAL.zone_header.display_flags & ZDAT.ZONE_FLAG_LIGHTNING) != 0)
                Debug.LogWarning("GfxTransformWorldsLightning(ot)");
            else
                Debug.LogWarning("GfxTransformWorlds(ot)");
        }
//Tick gool engine
        UpdateObjects(paused == 0 ? 1 : 0);
//Handle title logic
        if (GLOBAL.ldat.lid == LevelID.Map_Main_Menu_Title_Sequence)
            TitleUpdate();


    }


    void Update()
    {
        CollectInputs();
    }

    void CollectInputs()
    {
        InputSystem.bufferedKeys.Clear();
        foreach(KeyCode key in AllKeyCodes)
            if(Input.GetKeyDown(key))
                InputSystem.bufferedKeys.Add(key);

    }


}
