using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public GameObject chunkDisplay;
    public string levelID;

    void Start()
    {
        Debug.Log($"Attempting load at {levelID}");
        NSF level = new($"Assets/Streams/{levelID}.nsf");

        int chunksCount = level.chunks.Count;
        List<Color> blocks = new();
        foreach(Chunk chunk in level.chunks)
            blocks.Add(chunk.disp);

        this.GetComponent<MeshRenderer>().material.SetInt("_ColourCount", blocks.Count);
        this.GetComponent<MeshRenderer>().material.SetColorArray("_Colours", blocks);

    }
}
