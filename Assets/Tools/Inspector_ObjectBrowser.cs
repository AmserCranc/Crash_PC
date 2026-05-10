using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class NSFBrowserWindow : EditorWindow
{
    private TreeViewState _treeState;
    private NSFTreeView _treeView;

    public Bootstrap b;

    private NSF _nsf;

    [MenuItem("Window/NSF Browser")]
    public static void Open()
    {
        GetWindow<NSFBrowserWindow>("NSF Browser");
    }

    private void OnEnable()
    {
        b = GameObject.Find("Bootstrap").GetComponent<Bootstrap>();
        _treeState ??= new TreeViewState();

        _nsf = b.levelData;

        _treeView = new NSFTreeView(_treeState, _nsf);
    }

    private void OnGUI()
    {
        _treeView.OnGUI(new Rect(
            0,
            0,
            position.width,
            position.height));
    }
}