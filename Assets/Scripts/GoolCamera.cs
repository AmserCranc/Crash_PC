using UnityEngine;

public static class GoolCamera
{
    public struct Pose
    {
        public Vector3 position;
        public Quaternion rotation;

        public Pose(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    public static Pose ProgressToPose(zoneData.zone_path path, int progress)
    {
        int absProgress = Mathf.Abs(progress);

        int ptIndex = absProgress >> 8;
        int frac = absProgress & 0xFF;

        ptIndex = Mathf.Clamp(ptIndex, 0, path.points.Count - 1);

        if(ptIndex == -1)
            return new Pose();

        zoneData.zone_path_point a = path.points[ptIndex];
        zoneData.zone_path_point b;

        bool useNextPath = false;
        int nextIndex = Mathf.Min(ptIndex + 1, path.points.Count - 1);

        if (ptIndex == path.points.Count - 1 && frac != 0)
        {
            var nextPath = GetNextPath(path);

            if (nextPath != null && nextPath.points.Count > 0)
            {
                b = nextPath.points[0];
                useNextPath = true;
            }
            else
            {
                b = path.points[ptIndex];
            }
        }
        else
        {
            b = path.points[nextIndex];
        }

        float t = frac / 256f;

        Vector3 posA = ToVector(path, a);
        Vector3 posB = useNextPath
            ? ToVector(GetNextPath(path), b)
            : ToVector(path, b);

        Vector3 position = Vector3.Lerp(posA, posB, t);

        Quaternion rotA = ToRotation(a);
        Quaternion rotB = ToRotation(b);

        Quaternion rotation = Quaternion.Slerp(rotA, rotB, t);

        return new Pose(position, rotation);
    }

    private static Vector3 ToVector(zoneData.zone_path path, zoneData.zone_path_point p)
    {
        return new Vector3(p.x, p.y, p.z) / 256f;
    }

    private static Quaternion ToRotation(zoneData.zone_path_point p)
    {
        return Quaternion.Euler(p.rot_x, p.rot_y, p.rot_z);
    }

    private static zoneData.zone_path GetNextPath(zoneData.zone_path path)
    {
        return null;
    }
}