// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using EID = System.UInt32;
// using LID = System.Int32;

// public class LDAT : Entry
// {
//     public new NormalChunk parent;

//     public const int EXEC_COUNT = 64;
//     public new const int
//         pMAGIC      = 0x00;
//     public const int
//         pLID        = 0x04,
//         pZONE_SPAWN = 0x08,
//         pPATH_IDX_S = 0x0C,
//         pUnknown    = 0x10,
//         pEXEC_MAP   = 0x14,
//         pFOV        = 0x114;

//     public new uint magic           => (uint)ConvertBits.FromInt32(data, pMAGIC);      //Big ol assumptions happening here
//     public LID      lid             =>       ConvertBits.FromInt32(data, pLID); 
//     public EID      zone_spawn      => (uint)ConvertBits.FromInt32(data, pZONE_SPAWN);
//     public int      path_idx_spawn  =>       ConvertBits.FromInt32(data, pPATH_IDX_S);
//     public uint     fov             => (uint)ConvertBits.FromInt32(data, pFOV);

//     public EID[] exec_map           => ReadExecMap();

//     public LDAT(Entry _entry, NormalChunk _parent) : base(_entry.data)
//     {
//         Debug.Log("Created LDAT");
//         parent = _parent;
//         GLOBAL.ldat = this;


//     }

//     private EID[] ReadExecMap()
//     {
//         EID[] arr = new EID[EXEC_COUNT];
//         for (int i = 0; i < EXEC_COUNT; i++)
//         {
//             arr[i] = (uint)ConvertBits.FromInt32(data, pEXEC_MAP + i * 4);
//         }
//         return arr;
//     }
// }
