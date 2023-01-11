using Assets.Scripts.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TilemapController))]
public class TilemapControllerEditor : Editor
{
    [SerializeField]
    VisualTreeAsset m_ItemAsset;

    [SerializeField]
    VisualTreeAsset m_EditorAsset;

    public override VisualElement CreateInspectorGUI()
    {
        var root = m_EditorAsset.CloneTree();

        var listView = root.Q<ListView>();

        listView.makeItem = m_ItemAsset.CloneTree;

        return root;
    }
}
