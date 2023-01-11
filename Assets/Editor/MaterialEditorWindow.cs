using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialEditorWindow : EditorWindow
{
    private static List<Material> m_MaterialDatabase = new List<Material>();

    private VisualElement m_MaterialList;
    private static VisualTreeAsset m_MaterialRowTemplate;
    private ListView m_MaterialListView;
    private float m_ItemHeight = 40f;

    private VisualElement m_MaterialDetails;
    private VisualElement m_DisplayIcon;
    private Material m_ActiveMaterial;

    private Sprite m_DefaultMaterialIcon;

    [MenuItem("Solipsis/Material Editor")]
    public static void Init()
    {
        MaterialEditorWindow wnd = GetWindow<MaterialEditorWindow>();
        wnd.titleContent = new GUIContent("Material Database");

        Vector2 size = new Vector2(800, 640);
        wnd.minSize = size;
        wnd.maxSize = size;
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/material_editor_window.uxml");
        VisualElement rootFromUxml = visualTree.Instantiate();

        rootVisualElement.Add(rootFromUxml);

        m_DefaultMaterialIcon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/UnknownIcon.png", typeof(Sprite));

        m_MaterialRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/material_editor_list_item.uxml");

        LoadAllMaterials();

        m_MaterialList = rootVisualElement.Q<VisualElement>("MaterialList");
        GenerateListView();

        m_MaterialDetails = rootVisualElement.Q<VisualElement>("MaterialDetail");
        m_MaterialDetails.style.visibility = Visibility.Hidden;
        m_MaterialDetails.Q<TextField>("DisplayName").RegisterValueChangedCallback(
            evt =>
            {
                m_ActiveMaterial.Name = evt.newValue;

                m_MaterialListView.Rebuild();
            });
        m_MaterialDetails.Q<ObjectField>("IconPicker").RegisterValueChangedCallback(
            evt =>
            {
                Sprite newSprite = evt.newValue as Sprite;
                m_ActiveMaterial.Icon = newSprite == null
                    ? m_DefaultMaterialIcon
                    : newSprite;
                m_DisplayIcon.style.backgroundImage = newSprite == null
                    ? m_DefaultMaterialIcon.texture
                    : newSprite.texture;

                m_MaterialListView.Rebuild();
            });

        m_DisplayIcon = m_MaterialDetails.Q<VisualElement>("Icon");

        rootVisualElement.Q<Button>("Btn_AddMaterial").clicked += AddMaterial_OnClick;
        rootVisualElement.Q<Button>("Btn_DeleteMaterial").clicked += DeleteMaterial_OnClick;
    }

    private void DeleteMaterial_OnClick()
    {
        var path = AssetDatabase.GetAssetPath(m_ActiveMaterial);
        AssetDatabase.DeleteAsset(path);

        m_MaterialDatabase.Remove(m_ActiveMaterial);
        m_MaterialListView.Rebuild();

        m_MaterialDetails.style.visibility = Visibility.Hidden;
    }

    private void AddMaterial_OnClick()
    {
        Material newMaterial = CreateInstance<Material>();
        newMaterial.Name = "New Material";
        newMaterial.Icon = m_DefaultMaterialIcon;

        AssetDatabase.CreateAsset(newMaterial, $"Assets/Data/Materials/{newMaterial.Id}.asset");

        m_MaterialDatabase.Add(newMaterial);

        m_MaterialListView.Rebuild();
        m_MaterialListView.style.height = m_MaterialDatabase.Count * m_ItemHeight;
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => m_MaterialRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (element, index) =>
        {
            element.Q<VisualElement>("Icon").style.backgroundImage =
                m_MaterialDatabase[index] == null
                    ? m_DefaultMaterialIcon.texture
                    : m_MaterialDatabase[index].Icon.texture;

            element.Q<Label>("Name").text = m_MaterialDatabase[index].Name;
        };

        m_MaterialListView = new ListView(m_MaterialDatabase, 35, makeItem, bindItem);
        m_MaterialListView.selectionType = SelectionType.Single;
        m_MaterialListView.style.height = m_MaterialDatabase.Count * m_ItemHeight;
        m_MaterialList.Add(m_MaterialListView);

        m_MaterialListView.selectionChanged += MaterialListView_onSelectionChange;
    }

    private void MaterialListView_onSelectionChange(IEnumerable<object> selectedMaterials)
    {
        m_ActiveMaterial = (Material)selectedMaterials.FirstOrDefault();

        if (m_ActiveMaterial == null)
        {
            m_MaterialDetails.style.visibility = Visibility.Hidden;
            return;
        }

        SerializedObject so = new SerializedObject(m_ActiveMaterial);
        m_MaterialDetails.Bind(so);

        if (m_ActiveMaterial.Icon != null)
        {
            m_DisplayIcon.style.backgroundImage = m_ActiveMaterial.Icon.texture;
        }

        m_MaterialDetails.style.visibility = Visibility.Visible;
    }

    private void LoadAllMaterials()
    {
        m_MaterialDatabase.Clear();

        var allPaths = Directory.GetFiles("Assets/Data/Materials", "*.asset", SearchOption.AllDirectories);

        foreach (var path in allPaths)
        {
            var sanitizedPath = path.Replace(@"\", "/");
            m_MaterialDatabase.Add((Material)AssetDatabase.LoadAssetAtPath(sanitizedPath, typeof(Material)));
        }
    }
}

