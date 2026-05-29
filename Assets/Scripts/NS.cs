// using System;
// using LID = System.UInt32;
// using EID = System.UInt32;

// unsafe static public class NS
// {
//     public const int EXEC_COUNT = 64;


//     public static void Init(tNS _ns, LID _lid)
//     {
//         switch(_lid)
//         {
//             case LevelID.Map_Main_Menu_Title_Sequence:
//                 InitTitle(_ns); break;
                
//             case LevelID.Level_Completion_Screen:
//                 InitCompleted(_ns); break;
            
//             case LevelID.Intro:
//             case LevelID.Ending:
//                 InitMisc(_ns); break;

//             default:
//                 InitLevel(_ns);
//                 InitMisc(_ns);
//                 break;         
//         }
//     }

//     static void InitTitle(tNS _ns)
//     {
//         Open(_ns.ldat.exec_map[4], 0, 1);
//     }

//     static void InitCompleted(tNS _ns)
//     {
        
//     }

//     static void InitLevel(tNS _ns)
//     {
        
//     }

//     static void InitMisc(tNS _ns)
//     {
        
//     }


// }

// public struct tNS
// {
//     public int         initiated;
//     public LID         level_ID;

//     public NSD_LDat    ldat;

// }

// unsafe public struct NSD_LDat
// {
//     public uint        magic;
//     public LID         lid;
//     public EID         zone_spawn;
//     public int         path_IDX_spawn;
//     public uint        unk_10;
//     public fixed EID   exec_map[NS.EXEC_COUNT];
//     public uint        fov;
//     public fixed byte  image_data[0xF5F8];

// }
