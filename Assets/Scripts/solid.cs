using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static gool;
using static zoneData;
using static GLOBAL;


public class solid
{
    static public void TransSmoothStopAtSolid(
        gool_object obj,
        ref Vector3 velocity,
        zone_query query)
    {
        if (obj == null)
        {
            being_stopped = false;
            return;
        }

        // LID-dependent terrain tuning
        if (ldat.lid == LevelID.Hog_Wild || ldat.lid == LevelID.Whole_Hog)
            land_offset = 162500;
        else
            land_offset = 62500;

        Vector3 cur_velocity = velocity;
        Vector3 cur_trans = obj.vectors.trans;

        // If previously stopped on a surface, damp small slope motion
        if (being_stopped)
        {
            Vector3 slope_accel = new Vector3(
                prev_velocity.x - cur_velocity.x,
                prev_velocity.y - cur_velocity.y,
                prev_velocity.z - cur_velocity.z
            );

            if (Mathf.Abs(slope_accel.x) < 10 &&
                Mathf.Abs(slope_accel.y) < 10 &&
                Mathf.Abs(slope_accel.z) < 10)
            {
                cur_velocity.x = 0;
                cur_velocity.z = 0;
                // preserve Y (vertical motion)
                cur_velocity.y = velocity.y;
            }
        }

        Vector3 next_velocity = cur_velocity;
        Vector3 next_trans = cur_trans;

        TransPullStopAtSolid(obj, query, ref next_trans, ref next_velocity);

        Vector3 delta_trans = new Vector3(
            next_trans.x - cur_trans.x,
            next_trans.y - cur_trans.y,
            next_trans.z - cur_trans.z
        );

        if (!being_stopped &&
            (velocity.x != 0 || velocity.z != 0) &&
            Mathf.Abs(delta_trans.x) < 2 &&
            Mathf.Abs(delta_trans.y) < 2 &&
            Mathf.Abs(delta_trans.z) < 2)
        {
            prev_velocity = velocity;
            being_stopped = true;
        }
        else
        {
            being_stopped = false;
        }

        obj.vectors.trans = next_trans;
        velocity = next_velocity;

        // Event trigger logic
        if ((obj.process.status_a & 0x400) != 0)
        {
            uint arg = 0x6400;

            if ((obj.process.status_a & 1) == 0)
            {
                GoolSendEvent(0, obj, obj.process.gool_event, 1, ref arg);
            }
            else if (obj.process.gool_event != EVENT_BOX_STACK_BREAK)
            {
                GoolSendEvent(0, obj, obj.process.gool_event, 1, ref arg);
            }
        }
    }
}
