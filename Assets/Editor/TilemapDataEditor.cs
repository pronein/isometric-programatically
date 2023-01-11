using Assets.Scripts.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(TilemapData))]
public class TilemapDataEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        root.Add(new Label
        {
            bindingPath = "m_Tilemap.name"
        });

        return root;
    }
}
