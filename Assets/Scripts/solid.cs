using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using static gool;
using static zoneData;
using static GLOBAL;


public class solid
{
    static readonly uint[] circle_bitmap = new uint[32] {
        0b00000000000011111111000000000000,
        0b00000000001111111111110000000000,
        0b00000000111111111111111100000000,
        0b00000001111111111111111110000000,
        0b00000011111111111111111111000000,
        0b00000111111111111111111111100000,
        0b00001111111111111111111111110000,
        0b00011111111111111111111111111000,
        0b00111111111111111111111111111100,
        0b00111111111111111111111111111100,
        0b01111111111111111111111111111110,
        0b01111111111111111111111111111110,
        0b11111111111111111111111111111111,
        0b11111111111111111111111111111111,
        0b11111111111111111111111111111111,
        0b11111111111111111111111111111111,
        0b11111111111111111111111111111111,
        0b11111111111111111111111111111111,
        0b11111111111111111111111111111111,
        0b11111111111111111111111111111111,
        0b01111111111111111111111111111110,
        0b01111111111111111111111111111110,
        0b00111111111111111111111111111100,
        0b00111111111111111111111111111100,
        0b00011111111111111111111111111000,
        0b00001111111111111111111111110000,
        0b00000111111111111111111111100000,
        0b00000011111111111111111111000000,
        0b00000001111111111111111110000000,
        0b00000000111111111111111100000000,
        0b00000000001111111111110000000000,
        0b00000000000011111111000000000000
        };

    /* all integer-valued points in the upper right quadrant
    of a 32x32 square centered at the origin, such that x >= y,
    sorted by distance from the origin */
    private static readonly Vector2[] sorted_points =
    {
        new Vector2(1, 0),   new Vector2(1, 1),   new Vector2(2, 0),   new Vector2(2, 1),
        new Vector2(2, 2),   new Vector2(3, 0),   new Vector2(3, 1),   new Vector2(3, 2),
        new Vector2(4, 0),   new Vector2(4, 1),   new Vector2(3, 3),   new Vector2(4, 2),
        new Vector2(5, 0),   new Vector2(4, 3),   new Vector2(5, 1),   new Vector2(5, 2),
        new Vector2(4, 4),   new Vector2(5, 3),   new Vector2(6, 0),   new Vector2(6, 1),
        new Vector2(6, 2),   new Vector2(5, 4),   new Vector2(6, 3),   new Vector2(7, 0),
        new Vector2(7, 1),   new Vector2(5, 5),   new Vector2(6, 4),   new Vector2(7, 2),
        new Vector2(7, 3),   new Vector2(6, 5),   new Vector2(8, 0),   new Vector2(7, 4),
        new Vector2(8, 1),   new Vector2(8, 2),   new Vector2(6, 6),   new Vector2(8, 3),
        new Vector2(7, 5),   new Vector2(8, 4),   new Vector2(9, 0),   new Vector2(9, 1),
        new Vector2(7, 6),   new Vector2(9, 2),   new Vector2(8, 5),   new Vector2(9, 3),
        new Vector2(9, 4),   new Vector2(7, 7),   new Vector2(8, 6),   new Vector2(10, 0),
        new Vector2(10, 1),  new Vector2(10, 2),  new Vector2(9, 5),   new Vector2(10, 3),
        new Vector2(8, 7),   new Vector2(10, 4),  new Vector2(9, 6),   new Vector2(11, 0),
        new Vector2(11, 1),  new Vector2(11, 2),  new Vector2(10, 5),  new Vector2(8, 8),
        new Vector2(9, 7),   new Vector2(11, 3),  new Vector2(10, 6),  new Vector2(11, 4),
        new Vector2(12, 0),  new Vector2(9, 8),   new Vector2(12, 1),  new Vector2(11, 5),
        new Vector2(12, 2),  new Vector2(10, 7),  new Vector2(12, 3),  new Vector2(11, 6),
        new Vector2(12, 4),  new Vector2(9, 9),   new Vector2(10, 8),  new Vector2(12, 5),
        new Vector2(13, 0),  new Vector2(13, 1),  new Vector2(11, 7),  new Vector2(13, 2),
        new Vector2(13, 3),  new Vector2(12, 6),  new Vector2(10, 9),  new Vector2(11, 8),
        new Vector2(13, 4),  new Vector2(12, 7),  new Vector2(13, 5),  new Vector2(14, 0),
        new Vector2(14, 1),  new Vector2(10, 10), new Vector2(14, 2),  new Vector2(11, 9),
        new Vector2(14, 3),  new Vector2(13, 6),  new Vector2(12, 8),  new Vector2(14, 4),
        new Vector2(13, 7),  new Vector2(14, 5),  new Vector2(11, 10), new Vector2(12, 9),
        new Vector2(15, 0),  new Vector2(15, 1),  new Vector2(15, 2),  new Vector2(14, 6),
        new Vector2(13, 8),  new Vector2(15, 3),  new Vector2(15, 4),  new Vector2(11, 11),
        new Vector2(12, 10), new Vector2(14, 7),  new Vector2(13, 9),  new Vector2(15, 5),
        new Vector2(16, 0),  new Vector2(16, 1),  new Vector2(14, 8),  new Vector2(16, 2),
        new Vector2(15, 6),  new Vector2(16, 3),  new Vector2(12, 11), new Vector2(13, 10),
        new Vector2(16, 4),  new Vector2(15, 7),  new Vector2(14, 9),  new Vector2(16, 5),
        new Vector2(12, 12), new Vector2(15, 8),  new Vector2(13, 11), new Vector2(16, 6),
        new Vector2(14, 10), new Vector2(16, 7),  new Vector2(15, 9),  new Vector2(13, 12),
        new Vector2(14, 11), new Vector2(16, 8),  new Vector2(15, 10), new Vector2(16, 9),
        new Vector2(13, 13), new Vector2(14, 12), new Vector2(15, 11), new Vector2(16, 10),
        new Vector2(14, 13), new Vector2(15, 12), new Vector2(16, 11), new Vector2(14, 14),
        new Vector2(15, 13), new Vector2(16, 12), new Vector2(15, 14), new Vector2(16, 13),
        new Vector2(15, 15), new Vector2(16, 14), new Vector2(16, 15), new Vector2(16, 16)
    };

        /* note: these consts and not originally part of the .data section
        they are embedded in load instructions */
    public readonly struct Bound
    {
        public readonly Vector3 Min;
        public readonly Vector3 Max;

        public Bound(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }
    private static Vector3 VecShl8(float x, float y, float z)
    {
        return new Vector3(x * 256f, y * 256f, z * 256f);
    }
    private static readonly Bound test_bound_default = new Bound(
        VecShl8(-100, 0, -100),
        VecShl8(100, 0, 100)
    );
    private static readonly Bound test_bound_event = new Bound(
        VecShl8(-150, 0, -150),
        VecShl8(150, 665, 150)
    );
    private static readonly Bound test_bound_surface = new Bound(
        VecShl8(-6.25f, 0, -6.25f),
        VecShl8(6.25f, 665, 6.25f)
    );
    private static readonly Bound test_bound_zone = new Bound(
        VecShl8(-300, -267.5f, -300),
        VecShl8(300, 932.5f, 300)
    );
    private static readonly Bound test_bound_obj = new Bound(
        VecShl8(-75, 0, -75),
        VecShl8(75, 665, 75)
    );
    private static readonly Bound test_bound_objtop = new Bound(
        VecShl8(-75, 498.75f, -75),
        VecShl8(75, 665, 75)
    );
    private static readonly Bound test_bound_ceil = new Bound(
        VecShl8(-37.5f, 498.75f, -37.5f),
        VecShl8(37.5f, 665, 37.5f)
    );
    /* .sdata */
    public static int land_offset = 62500;     /* 800564BC; gp[0x30] */
    /* .sbss */
    public static int being_stopped;               /* 800565DC; gp[0x78] */
    //uint32_t *wall_cache;            /* 800566AC; gp[0xAC] */
    //uint32_t *wall_bitmap;           /* 800566F4; gp[0xBE] */
    //uint32_t *p_circle_bitmap;       /* 8005670C; gp[0xC4] */
    /* .bss */
    static public Vector3 prev_velocity;               /* 800567F8 */
    //static public zone_query cur_zone_query;       /* 8005CFEC */

    // static public void TransSmoothStopAtSolid(gool_object obj, ref Vector3 velocity, zone_query query)
    // {
    //     if (obj == null)
    //     {
    //         being_stopped = 0;
    //         return;
    //     }

    //     // LID-dependent terrain tuning
    //     if (nsd.levelID == LevelID.Hog_Wild || nsd.levelID == LevelID.Whole_Hog)
    //         land_offset = 162500;
    //     else
    //         land_offset = 62500;

    //     Vector3 cur_velocity = velocity;
    //     Vector3 cur_trans = obj.vectors.trans;

    //     // If previously stopped on a surface, damp small slope motion
    //     if (being_stopped == 1)
    //     {
    //         Vector3 slope_accel = new Vector3(
    //             prev_velocity.x - cur_velocity.x,
    //             prev_velocity.y - cur_velocity.y,
    //             prev_velocity.z - cur_velocity.z
    //         );

    //         if (Mathf.Abs(slope_accel.x) < 10 &&
    //             Mathf.Abs(slope_accel.y) < 10 &&
    //             Mathf.Abs(slope_accel.z) < 10)
    //         {
    //             cur_velocity.x = 0;
    //             cur_velocity.z = 0;
    //             // preserve Y (vertical motion)
    //             cur_velocity.y = velocity.y;
    //         }
    //     }

    //     Vector3 next_velocity = cur_velocity;
    //     Vector3 next_trans = cur_trans;

    //     TransPullStopAtSolid(obj, query, ref next_trans, ref next_velocity);

    //     Vector3 delta_trans = new Vector3(
    //         next_trans.x - cur_trans.x,
    //         next_trans.y - cur_trans.y,
    //         next_trans.z - cur_trans.z
    //     );

    //     if (!being_stopped &&
    //         (velocity.x != 0 || velocity.z != 0) &&
    //         Mathf.Abs(delta_trans.x) < 2 &&
    //         Mathf.Abs(delta_trans.y) < 2 &&
    //         Mathf.Abs(delta_trans.z) < 2)
    //     {
    //         prev_velocity = velocity;
    //         being_stopped = true;
    //     }
    //     else
    //     {
    //         being_stopped = false;
    //     }

    //     obj.vectors.trans = next_trans;
    //     velocity = next_velocity;

    //     // Event trigger logic
    //     if ((obj.process.status_a & 0x400) != 0)
    //     {
    //         uint arg = 0x6400;

    //         if ((obj.process.status_a & 1) == 0)
    //         {
    //             GoolSendEvent(0, obj, obj.process.gool_event, 1, ref arg);
    //         }
    //         else if (obj.process.gool_event != EVENT_BOX_STACK_BREAK)
    //         {
    //             GoolSendEvent(0, obj, obj.process.gool_event, 1, ref arg);
    //         }
    //     }
    // }
}
