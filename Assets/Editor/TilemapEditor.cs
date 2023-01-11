using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CustomEditor(typeof(Tilemap))]
public class TilemapEditor : Editor
{
    public VisualTreeAsset m_Uxml;
    [SerializeField]
    private TileBase[] m_Tiles;

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        m_Uxml.CloneTree(root);

        var foldout = new Foldout() { viewDataKey = "TilemapEditorFoldout", text = "Default Inspector" };

        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);

        var target = serializedObject.targetObject;
        if (target is Tilemap)
        {
            var tileMap = target as Tilemap;
            BoundsInt bounds = new BoundsInt(tileMap.origin, tileMap.size);
            var tiles = tileMap.GetTilesBlock(bounds).Where(x => x is ConnectedTile).ToArray();
            var list = new ListView
            {
                makeItem = () =>
                {
                    var item = new Box();
                    item.AddToClassList("box-items");

                    item.Add(new Label());
                    item.Add(new ColorField() { label = "Color" });
                    item.Add(new ObjectField() { label = "Sprite", objectType = typeof(Sprite) });

                    return item;
                },
                bindItem = (element, idx) =>
                {
                    element.Q<Label>().text = tiles[idx].name;
                    element.Q<ColorField>().value = (tiles[idx] as Tile).color;
                    element.Q<ObjectField>().value = (tiles[idx] as Tile).sprite.texture;
                },
                itemsSource = tiles,
                selectionType = SelectionType.Single
            };
            list.AddToClassList("tile-list");
            list.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            //list.fixedItemHeight = 40;

            list.itemsChosen += Debug.Log;
            list.selectionChanged += (o) =>
            {
                var t = o.FirstOrDefault() as Tile;
                if (t != null)
                {
                    GridSelection.Select(serializedObject.targetObject, new BoundsInt(new Vector3Int((int)t.transform.GetPosition().x, (int)t.transform.GetPosition().y, (int)t.transform.GetPosition().z), Vector3Int.one));
                }
            };

            var tilesFoldout = new Foldout { viewDataKey = "TilesFoldout", text = "Tiles" };
            tilesFoldout.Add(list);
            root.Add(tilesFoldout);
        }

        root.Add(foldout);

        root.viewDataKey = "TilemapEditorRoot";

        return root;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
