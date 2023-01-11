using Assets.Scripts.Core.Entities;
using Assets.Scripts.Tilemaps;
using System;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomEditor(typeof(EntityTile))]
public class EntityTileEditor : Editor
{
    SerializedProperty m_TileTypeProperty;

    private void OnEnable()
    {
        m_TileTypeProperty = serializedObject.FindProperty("m_EntityType");
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        var tileTypeField = new EnumField()
        {
            label = "Entity Tile Type",
            bindingPath = "m_EntityType"
        };

        tileTypeField.RegisterValueChangedCallback(
            (e) =>
            {
                var tileType = (EntityBaseType)e.newValue;
                Debug.Log($"Value changed to: {tileType}");

                ShowTileMetadata(root, (EntityBaseType)m_TileTypeProperty.enumValueIndex);
            });

        root.Add(tileTypeField);

        var spritePreviewField = new ObjectField
        {
            label = "Preview",
            bindingPath = "m_Preview"
        };

        root.Add(spritePreviewField);

        var spriteField = new ObjectField
        {
            label = "Sprite",
            bindingPath = "m_Sprite"
        };

        root.Add(spriteField);

        var spriteColorField = new ColorField
        {
            label = "Color",
            bindingPath = "m_Color"
        };

        root.Add(spriteColorField);

        var tileFlagsField = new EnumFlagsField
        {
            label = "Flags",
            bindingPath = "m_Flags"
        };

        root.Add(tileFlagsField);

        var tileMetadataContainer = new Foldout
        {
            text = "Entity Tile Metadata",
            value = false,
            name = "tileMetadataContainer"
        };

        root.Add(tileMetadataContainer);

        ShowTileMetadata(root, (EntityBaseType)m_TileTypeProperty.enumValueIndex);

        return root;
    }

    private void ShowTileMetadata(VisualElement container, EntityBaseType tileType)
    {
        var foldout = container.Q<Foldout>("tileMetadataContainer");

        foldout.Clear();

        var asset = Resources.Load<VisualTreeAsset>("tile_metadata_editor");
        var tileMetadataEditor = asset.Instantiate("m_TileMetadata");

        foldout.Add(tileMetadataEditor);

        switch (tileType)
        {
            case EntityBaseType.Matter:
                ShowMatterMetadata(foldout);
                break;
        }
    }

    private void ShowMatterMetadata(VisualElement container)
    {
    }
}
