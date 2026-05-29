using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LID = System.UInt32;

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
        LoadLevel(LID_BOOTLEVEL);

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

        // tNS nsStruct = new();
        // NS.Init(nsStruct, level);
    }
}
