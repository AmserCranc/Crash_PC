using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

public class ZDAT : Entry
{
#region flags
    public const int ZONE_FLAG_UP_DOWN               = 0x1;
    public const int ZONE_FLAG_SOLID_BOTTOM          = 0x2;
    public const int ZONE_FLAG_HAS_WATER             = 0x4;
    public const int ZONE_FLAG_LOC_BASED_WAIT        = 0x8;
    public const int ZONE_FLAG_FOG                   = 0x10;
    public const int ZONE_FLAG_AUTO_CAM_ZOFFSET      = 0x80;
    public const int ZONE_FLAG_RIPPLE                = 0x100;
    public const int ZONE_FLAG_LIGHTNING             = 0x200;
    public const int ZONE_FLAG_DARK2                 = 0x400;
    public const int ZONE_FLAG_CAM_BOUNCE            = 0x1000;
    public const int ZONE_FLAG_NO_SAVE               = 0x2000;
    public const int ZONE_FLAG_SIDE_SCROLL           = 0x4000;
    public const int ZONE_FLAG_BACKWARD              = 0x8000;
    public const int ZONE_FLAG_SOLID_TOP             = 0x20000;
    public const int ZONE_FLAG_DISCARD_BELOW_PATHS   = 0x40000;
    public const int ZONE_FLAG_AUTO_CAM_SKIP_DISABLE = 0x80000;
    public const int ZONE_FLAG_SOLID_SIDES           = 0x100000;
    public const int ZONE_FLAG_FOG_LIGHTNING         = (0x10 | 0x200);
#endregion

    public List<Item> items = new();
    List<byte[]> rawitems = new();

    public byte[] header   => rawitems[0];
    public byte[] layout   => rawitems[1];
    public int camCount    => ConvertBits.FromInt32(header, 0x208);
    public int entityCount => ConvertBits.FromInt32(header, 0x20C);

    public ZDAT(Entry _entry) : base(_entry.data)
    {
        for(int item = 0; item < itemCount; item++)
            rawitems.Add(ExtractItem(item));

        for(int cam = 2; cam < 2 + camCount; cam++)
            items.Add(new cCamera(rawitems[cam]));

        for(int entity = 2; entity < 2 + camCount + entityCount; entity++)
           items.Add(new cEntity(rawitems[entity]));
        
    }


    public override void DrawToTreeView(NSFInspectorWindow n)
    {
        EditorGUILayout.LabelField(
            "Header",
            NSFInspectorWindow.ToHexString(header));

        EditorGUILayout.LabelField(
            "layout",
            NSFInspectorWindow.ToHexString(layout));

        EditorGUILayout.LabelField(
            "camCount",
            camCount.ToString());    

        EditorGUILayout.LabelField(
            "entityCount",
           entityCount.ToString());
    }
}
