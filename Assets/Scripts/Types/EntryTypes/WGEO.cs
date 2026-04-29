using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGEO : Entry
{
    private byte[] raw;

    public byte[] info;
    public WGEO_vertex[] verts;
    public WGEO_tri[] tris;
    public WGEO_quad[] quads;
    public byte[] item4;
    public WGEO_colour[] colours;
    public byte[] item6;

    public WGEO(byte[] entrydata, int offset)
    {

        
    }
}
