using Assets.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tilemaps
{
    /// <summary>
    /// Stores/Manipulates the association between a tiles position (Vector3Int),
    /// the rendered tile (TileBase) and the associated metadata (TileMetadata) for
    /// a given Tilemap
    /// </summary>
    [Serializable]
    public class TilemapData
    {
        [SerializeField]
        private Tilemap m_Tilemap;

        [SerializeField]
        private List<TileMetaVector> m_Tiles;

        public TilemapData()
        {
            m_Tiles = new List<TileMetaVector>();
        }

        public Tilemap Tilemap { get { return m_Tilemap; } }

        public void Initialize(Tilemap tilemap)
        {
            m_Tilemap = tilemap;

            var tilemapBounds = new BoundsInt(tilemap.origin, tilemap.size);

            int z = tilemapBounds.z;
            for (int x = tilemapBounds.xMin, lenX = tilemapBounds.xMax; x <= lenX; x++)
            {
                for (int y = tilemapBounds.yMin, lenY = tilemapBounds.yMax; y <= lenY; y++)
                {
                    var position = new Vector3Int(x, y, z);
                    if (tilemap.HasTile(position))
                    {
                        AddTile(position, tilemap.GetTile(position));
                    }
                }
            }
        }

        /* TODO: If we change to a different tile in same position */
        public void AddTile(Vector3Int position, TileBase tile)
        {
            if (!m_Tiles.Any(t => t.Position == position))
            {
                m_Tiles.Add(new TileMetaVector
                {
                    Position = position,
                    Tile = tile,
                    Meta = new TileMetadata()
                });
            }
        }

        public void RemoveTile(Vector3Int position)
        {
            var tileMetaVector = m_Tiles.FirstOrDefault(t => t.Position == position);
            if (tileMetaVector != null)
            {
                m_Tiles.Remove(tileMetaVector);
            }
        }
    }
}
