using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

enum ConnectedDirection
{
    None = 0,
    East = 1,
    North = 2,
    West = 4,
    South = 8
}

enum SpriteIndex
{
    NorthSouth = 0,
    EastWest = 1,
    EastCorner = 2,
    NorthCorner = 3,
    WestCorner = 4,
    SouthCorner = 5,
    WestT = 6,
    SouthT = 7,
    EastT = 8,
    NorthT = 9,
    Cross = 10,
    Single = 11
}

public class EntityTile : Tile
{
    public Sprite[] m_Sprites;
    public Sprite m_Preview;
    public Dictionary<EntityComponent,float> m_Components;

    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (var yd = -1; yd <= 1; yd++)
        {
            for (var xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasEntityTile(tilemap, position))
                    tilemap.RefreshTile(position);
            }
        }
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        // North
        int mask = HasEntityTile(tilemap, location + new Vector3Int(0, 1, 0)) ? (int)ConnectedDirection.North : 0;
        // East
        mask += HasEntityTile(tilemap, location + new Vector3Int(1, 0, 0)) ? (int)ConnectedDirection.East : 0;
        // South
        mask += HasEntityTile(tilemap, location + new Vector3Int(0, -1, 0)) ? (int)ConnectedDirection.South : 0;
        // West
        mask += HasEntityTile(tilemap, location + new Vector3Int(-1, 0, 0)) ? (int)ConnectedDirection.West : 0;

        var index = GetIndex((byte)mask);

        if (index >= 0 && index < m_Sprites.Length)
        {
            tileData.sprite = m_Sprites[index];
            tileData.color = Color.white;

            //var m = tileData.transform;
            //m.SetTRS(Vector3.zero, GetRotation((byte)mask), Vector3.one);

            //tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.None;
        }
        else
        {
            Debug.LogWarning("Not enough sprites in EntityTile instance");
        }
    }

    private bool HasEntityTile(ITilemap tilemap, Vector3Int location)
    {
        return tilemap.GetTile(location) == this;
    }

    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return (int)SpriteIndex.Single;
            case 2:
            case 8:
            case 10: return (int)SpriteIndex.NorthSouth;
            case 1:
            case 4:
            case 5: return (int)SpriteIndex.EastWest;
            case 9: return (int)SpriteIndex.NorthCorner;
            case 12: return (int)SpriteIndex.EastCorner;
            case 6: return (int)SpriteIndex.SouthCorner;
            case 3: return (int)SpriteIndex.WestCorner;
            case 13: return (int)SpriteIndex.SouthT;
            case 14: return (int)SpriteIndex.WestT;
            case 7: return (int)SpriteIndex.NorthT;
            case 11: return (int)SpriteIndex.EastT;
            case 15: return (int)SpriteIndex.Cross;
        }

        return -1;
    }

    private Quaternion GetRotation(byte mask)
    {
        switch (mask)
        {
            case 9:
            case 10:
            case 7:
            case 2:
            case 8:
                return Quaternion.Euler(0f, 0f, -90f);
            case 3:
            case 14:
                return Quaternion.Euler(0f, 0f, -180f);
            case 6:
            case 13:
                return Quaternion.Euler(0f, 0f, -270f);
        }

        return Quaternion.Euler(0f, 0f, 0f);
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/EntityTile")]
    public static void CreateEntityTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Entity Tile", "New Entity Tile", "Asset", "Save Entity Tile", "Assets");
        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EntityTile>(), path);
    }
#endif
}
