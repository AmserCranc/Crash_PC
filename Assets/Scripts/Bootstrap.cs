using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LID = System.UInt32;
using static gool;

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

    void Start()
    {
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
            GLOBAL.life_hud     = ObjectCreate(handles[1].self, 4, 0, 0, 0, 0);
            GLOBAL.fruit_hud    = ObjectCreate(handles[1].self, 4, 1, 0, 0, 0);
            GLOBAL.pickup_hud   = ObjectCreate(handles[1].self, 4, 5, 0, 0, 0);
        }
    }


}
