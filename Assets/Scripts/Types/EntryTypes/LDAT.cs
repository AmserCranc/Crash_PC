using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDAT : Entry
{
//Init LDAT properly
    public uint lid     => 0; 

    public LDAT(Entry _entry) : base(_entry.data)
    {
        GLOBAL.ldat = this;
    }
}
