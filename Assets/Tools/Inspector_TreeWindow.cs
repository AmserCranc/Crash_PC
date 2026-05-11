using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NSFInspectorWindow : EditorWindow
{
    private object _current;

    [MenuItem("Window/NSF Inspector")]
    public static void Open()
    {
        GetWindow<NSFInspectorWindow>("NSF Inspector");
    }

    private void OnEnable()
    {
        NSFSelection.SelectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        NSFSelection.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged(object obj)
    {
        _current = obj;
        Repaint();
    }

    private void OnGUI()
    {
        if (_current == null)
        {
            GUILayout.Label("Nothing Selected");
            return;
        }

        switch (_current)
        {
            case NSD header:
                DrawHeader(header);
                break;

            case NormalChunk chunk:
                DrawNormalChunk(chunk);
                break;

            case TextureChunk tex:
                DrawTextureChunk(tex);
                break;

            case Entry entry:
                DrawEntry(entry);
                break;

            default:
                GUILayout.Label(_current.GetType().Name);
                break;
        }
    }

    private void DrawHeader(NSD h)
    {
        GUILayout.Label("Header", EditorStyles.boldLabel);

        EditorGUILayout.LabelField(
            "Level Name",
            h.levelID.ToString());
    }

    private void DrawNormalChunk(NormalChunk c)
    {
        GUILayout.Label(
            "Normal Chunk",
            EditorStyles.boldLabel);

        EditorGUILayout.LabelField(
            "Entries",
            c.entries.Count.ToString());
    }

    private void DrawTextureChunk(TextureChunk c)
    {
        GUILayout.Label(
            "Texture Chunk",
            EditorStyles.boldLabel);
    }

    private void DrawEntry(Entry e)
    {
        GUILayout.Label("Entry", EditorStyles.boldLabel);

        EditorGUILayout.LabelField(
            "Entry",
            e.GetType().ToString());

        e.DrawToTreeView(this);
    }


    public static string ToBinaryString(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        return string.Join(
            "_",
            data.Select(b => Convert.ToString(b, 2).PadLeft(8, '0'))
        );
    }

    public static string ToHexString(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        return string.Join(
            "_",
            data.Select(b => b.ToString("X2"))
        );
    }
}