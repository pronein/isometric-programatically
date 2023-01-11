using Assets.Scripts.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class MaterialAsset
{
    [SerializeField]
    private Material m_Material;
    [SerializeField]
    private float m_Amount;

    public MaterialAsset(Material material, float amount)
    {
        m_Material = material;
        m_Amount = amount;
    }
}

[CustomEditor(typeof(MaterialAsset))]
public class MaterialAssetEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        root.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

        root.Add(new Label("Material"));

        root.Add(new ObjectField
        {
            bindingPath = "m_Material"
        });

        root.Add(new FloatField
        {
            bindingPath = "m_Amount"
        });

        return root;
    }
}

[CustomEditor(typeof(TileMetadata))]
public class TileMetadataEditor : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
        var asset = Resources.Load<VisualTreeAsset>("tile_metadata_editor");
        var rootTree = asset.Instantiate();

        var materialsListView = rootTree.Q<ListView>("MaterialsListView");

        var materialKeys = serializedObject.FindProperty("_materialKeys");
        var materials = materialKeys.managedReferenceValue as List<Material>;

        var materialValues = serializedObject.FindProperty("_materialValues");
        var values = materialValues.managedReferenceValue as List<float>;

        for(int idx = 0, len = Math.Min(materials.Count, values.Count); idx < len; idx++)
        {
            var materialAsset = new MaterialAsset(materials[idx], values[idx]);
            materialsListView.itemsSource.Add(materialAsset);
        }

        return rootTree;
    }
}
