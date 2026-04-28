using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public GameObject chunkDisplay;
    public string levelID;
    public string streamLocation = "Assets/Streams/";

    NSD levelHeader;
    NSF levelData;

    void Start()
    {
        Debug.Log($"Attempting load at {levelID}");
        levelHeader = new($"{streamLocation}{levelID}.nsd");
        levelData = new($"{streamLocation}{levelID}.nsf");

#region Chunk Visualiser
        List<Color> blocks = new();
        foreach(Chunk chunk in levelData.chunks)
            blocks.Add(chunk.disp);

        this.GetComponent<MeshRenderer>().material.SetInt("_ColourCount", blocks.Count);
        this.GetComponent<MeshRenderer>().material.SetColorArray("_Colours", blocks);
#endregion
    }
}
