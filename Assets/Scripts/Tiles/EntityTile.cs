using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts.Core.Entities;
using Assets.Scripts.Tilemaps;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EntityBaseType
{
    Matter = 0,
    Product = 1,
    Structure = 2
}

public enum EntityTileType
{
    EntityTile = 0,
    ConnectedTile = 1
}

public class EntityTile : Tile
{
    [SerializeField]
    private string m_Id = Guid.NewGuid().ToString().ToUpper();

    [SerializeField]
    private Sprite m_Preview;

    [SerializeField]
    private EntityBaseType m_EntityType;

    [SerializeReference]
    private TileMetadata m_TileMetadata = new TileMetadata();

    public EntityBaseType EntityType { get { return m_EntityType; } }
    public Sprite Preview { get { return m_Preview; } set { m_Preview = value; } }
    public string Id { get { return m_Id; } }
    public TileMetadata Metadata { get { return m_TileMetadata; } }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = m_Preview;
        tileData.color = Color.white;
        tileData.flags = TileFlags.LockTransform;
        tileData.colliderType = ColliderType.None;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Tiles/Entity Tile")]
    public static void CreateTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Entity Tile", "New Entity Tile", "Asset", "Save Entity Tile", "Assets");
        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset(CreateInstance<EntityTile>(), path);
    }
#endif
}
