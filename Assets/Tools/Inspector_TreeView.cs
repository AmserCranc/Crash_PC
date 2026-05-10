using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public class NSFTreeView : TreeView
{
    private readonly NSF _nsf;

    private readonly Dictionary<int, object> _lookup = new();

    private int _nextId = 1;

    public NSFTreeView(TreeViewState state, NSF nsf)
        : base(state)
    {
        _nsf = nsf;

        Reload();
    }

    protected override TreeViewItem BuildRoot()
    {
        _lookup.Clear();
        _nextId = 1;

        var root = CreateItem("Root", null);
        root.AddChild(CreateItem("Header", _nsf.nsd));
        var nsfItem = CreateItem("NSF", _nsf);

        root.AddChild(nsfItem);

        foreach (var chunk in _nsf.chunks)
        {
            var chunkItem = CreateItem(chunk.type.ToString(), chunk);

            nsfItem.AddChild(chunkItem);

            if (chunk is NormalChunk normal)
            {
                foreach (var entry in normal.entries)
                {
                    var entryItem =
                        CreateItem(entry.type.ToString(), entry);

                    chunkItem.AddChild(entryItem);
                }
            }
        }

        SetupDepthsFromParentsAndChildren(root);

        return root;
    }

    private TreeViewItem CreateItem(
        string name,
        object data)
    {
        int id = _nextId++;

        _lookup[id] = data;

        return new TreeViewItem(id, 0, name);
    }

    protected override void SelectionChanged(
        IList<int> selectedIds)
    {
        if (selectedIds.Count == 0)
            return;

        int id = selectedIds[0];

        if (_lookup.TryGetValue(id, out var obj))
        {
            NSFSelection.Set(obj);
        }
    }
}