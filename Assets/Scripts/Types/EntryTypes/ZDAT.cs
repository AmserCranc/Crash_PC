using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

public class ZDAT : Entry
{
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
