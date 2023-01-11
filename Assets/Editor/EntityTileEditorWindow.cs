using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class EntityTileEditorWindow : EditorWindow
{
    private static List<EntityTile> m_TileDatabase = new List<EntityTile>();
    private static List<Material> m_MaterialDatabase = new List<Material>();

    private static VisualTreeAsset m_TileRowTemplate;
    private static VisualTreeAsset m_MaterialRowTemplate;
    private static VisualTreeAsset m_EntityMaterialRowTemplate;
    private Sprite m_DefaultTileIcon;
    private Sprite m_DefaultMaterialIcon;

    private List<Material> m_TileMaterialsAvailable;
    private List<EntityMaterial> m_TileMaterialsSelected;

    private VisualElement m_TileContainer;
    private ListView m_TileListView;
    private float m_ItemHeight = 40f;

    private VisualElement m_TileDetailContainer;

    private ListView m_MaterialsAvailableListView;
    private ListView m_MaterialsSelectedListView;

    private EntityTile m_ActiveTile;


    [MenuItem("Solipsis/Entity Tile Editor")]
    public static void Init()
    {
        EntityTileEditorWindow wnd = GetWindow<EntityTileEditorWindow>();
        wnd.titleContent = new GUIContent("Entity Tile Database");

        Vector2 size = new Vector2(800, 640);
        wnd.minSize = size;
        wnd.maxSize = size;
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/entity_tile_editor_window.uxml");
        var rootFromUxml = visualTree.Instantiate();

        m_DefaultMaterialIcon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/UnknownIcon.png", typeof(Sprite));
        m_DefaultTileIcon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/UnknownIcon.png", typeof(Sprite));

        m_TileRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/entity_tile_editor_list_item.uxml");
        m_MaterialRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/material_editor_list_item.uxml");
        m_EntityMaterialRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/entity_material_editor_list_item.uxml");

        rootVisualElement.Add(rootFromUxml);

        LoadAllTiles();
        LoadAllMaterials();

        m_TileContainer = rootVisualElement.Q<VisualElement>("TilesContainer");
        GenerateListView();

        m_TileDetailContainer = rootVisualElement.Q<VisualElement>("TileDetailsContainer");
        m_TileDetailContainer.style.visibility = Visibility.Hidden;

        var materialsContent = m_TileDetailContainer.Q<VisualElement>("MaterialsContent");
        m_MaterialsAvailableListView = materialsContent.Q<ListView>("AvailableMaterials");
        m_MaterialsSelectedListView = materialsContent.Q<ListView>("SelectedMaterials");

        materialsContent.Q<Button>("Btn_AddMaterial").clicked += AddMaterial_clicked;
        materialsContent.Q<Button>("Btn_RemoveMaterial").clicked += RemoveMaterial_clicked;
    }

    private void RemoveMaterial_clicked()
    {
        throw new NotImplementedException();
    }

    private void AddMaterial_clicked()
    {
        var selectedMaterial = m_MaterialsAvailableListView.selectedItem as Material;
        if (selectedMaterial == null)
        {
            return;
        }

        var entityMaterial = new EntityMaterial { Material = selectedMaterial, Amount = 1f };
        if (m_MaterialsSelectedListView.itemsSource == null)
        {
            m_MaterialsSelectedListView.itemsSource = new List<EntityMaterial>();
        }
        m_MaterialsSelectedListView.itemsSource.Add(entityMaterial);
        m_MaterialsSelectedListView.Rebuild();
        m_MaterialsSelectedListView.style.height = m_MaterialsSelectedListView.itemsSource.Count * m_ItemHeight;

        m_ActiveTile.Metadata.Materials = m_MaterialsSelectedListView.itemsSource as List<EntityMaterial>;
        new SerializedObject(m_ActiveTile).ApplyModifiedProperties();

        m_MaterialsAvailableListView.itemsSource.Remove(selectedMaterial);
        m_MaterialsAvailableListView.Rebuild();
        m_MaterialsAvailableListView.style.height = m_MaterialsAvailableListView.itemsSource.Count * m_ItemHeight;

        
    }

    private string BuildTileTypeString(int tileDbIndex)
    {
        var tile = m_TileDatabase[tileDbIndex];

        if (tile is ConnectedTile)
        {
            return $"(ConnectedTile - {(tile as ConnectedTile).EntityType})";
        }
        else if (tile is EntityTile)
        {
            return $"(EntityTile - {(tile as EntityTile).EntityType})";
        }
        else
        {
            return "(Tile)";
        }
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => m_TileRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (element, index) =>
        {
            element.Q<VisualElement>("PreviewIcon").style.backgroundImage =
                m_TileDatabase[index] == null
                    ? m_DefaultTileIcon.texture
                    : m_TileDatabase[index].Preview.texture;

            element.Q<Label>("DisplayName").text = m_TileDatabase[index].name;
            element.Q<Label>("TileType").text = BuildTileTypeString(index);
        };

        m_TileListView = new ListView(m_TileDatabase, 35, makeItem, bindItem);
        m_TileListView.selectionType = SelectionType.Single;
        m_TileListView.style.height = m_TileDatabase.Count * m_ItemHeight;
        m_TileContainer.Add(m_TileListView);

        m_TileListView.selectionChanged += TileListView_onSelectionChanged;
    }

    private void TileListView_onSelectionChanged(IEnumerable<object> selectedTiles)
    {
        m_ActiveTile = (EntityTile)selectedTiles.FirstOrDefault();

        if (m_ActiveTile == null)
        {
            m_TileDetailContainer.style.visibility = Visibility.Hidden;
            return;
        }

        var so = new SerializedObject(m_ActiveTile);
        m_TileDetailContainer.Bind(so);

        // Set initial default values (as necessary)
        m_MaterialsAvailableListView.Clear();
        m_MaterialsAvailableListView.fixedItemHeight = 35;
        m_MaterialsAvailableListView.makeItem = () => m_MaterialRowTemplate.CloneTree();
        m_MaterialsAvailableListView.bindItem = (element, index) =>
        {
            element.Q<VisualElement>("Icon").style.backgroundImage =
                m_MaterialDatabase[index] == null
                    ? m_DefaultMaterialIcon.texture
                    : m_MaterialDatabase[index].Icon.texture;

            element.Q<Label>("Name").text = m_MaterialDatabase[index].Name;
        };
        m_MaterialsAvailableListView.itemsSource =
            m_MaterialDatabase.Except(m_ActiveTile.Metadata.Materials.Select(m => m.Material)).ToList();
        m_MaterialsAvailableListView.style.height = m_MaterialsAvailableListView.itemsSource.Count * m_ItemHeight;

        m_MaterialsSelectedListView.Clear();
        m_MaterialsSelectedListView.fixedItemHeight = 35;
        m_MaterialsSelectedListView.makeItem = () => m_EntityMaterialRowTemplate.CloneTree();
        m_MaterialsSelectedListView.bindItem = (element, index) =>
        {
            element.Q<VisualElement>("Icon").style.backgroundImage =
                (m_MaterialsSelectedListView.itemsSource[index] as EntityMaterial) == null
                    ? m_DefaultMaterialIcon.texture
                    : (m_MaterialsSelectedListView.itemsSource[index] as EntityMaterial).Material.Icon.texture;

            element.Q<Label>("Name").text = (m_MaterialsSelectedListView.itemsSource[index] as EntityMaterial).Material.Name;
            element.Q<Slider>("Amount").value = (m_MaterialsSelectedListView.itemsSource[index] as EntityMaterial).Amount;
            element.Q<Slider>("Amount").RegisterValueChangedCallback(
                evt =>
                {
                    m_ActiveTile.Metadata.Materials[index].Amount = evt.newValue;
                    new SerializedObject(m_ActiveTile).ApplyModifiedProperties();
                });
        };
        m_MaterialsSelectedListView.itemsSource =
            m_ActiveTile.Metadata.Materials.ToList();
        m_MaterialsSelectedListView.style.height = m_MaterialsSelectedListView.itemsSource.Count * m_ItemHeight;

        m_TileDetailContainer.style.visibility = Visibility.Visible;

        // Set initial visibility based on values of active tile (m_ActiveTile)
    }

    private void LoadAllTiles()
    {
        m_TileDatabase.Clear();

        var allPaths = Directory.GetFiles("Assets/Data/Tiles", "*.asset", SearchOption.AllDirectories);

        foreach(var path in allPaths)
        {
            var sanitizedPath = path.Replace(@"\", "/");
            m_TileDatabase.Add((EntityTile)AssetDatabase.LoadAssetAtPath(sanitizedPath, typeof(EntityTile)));
        }
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
