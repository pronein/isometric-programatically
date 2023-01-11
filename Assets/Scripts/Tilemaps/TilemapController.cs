using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tilemaps
{
    [ExecuteInEditMode]
    public class TilemapController : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TilemapData> m_TilemapData;
        //private IDictionary<Tilemap, TilemapData> m_TilemapData;

        private Tilemap[] m_Tilemaps;

        // Start is called before the first frame update
        void OnEnable()
        {
            m_Tilemaps = GetComponentsInChildren<Tilemap>();

            Debug.Log($"Tilemap Count: {m_Tilemaps.Length}");

            //m_TilemapData = new Dictionary<Tilemap, TilemapData>();
            m_TilemapData = new List<TilemapData>();

            foreach (var tilemap in m_Tilemaps)
            {
                var tilemapData = new TilemapData();
                tilemapData.Initialize(tilemap);

                m_TilemapData.Add(tilemapData);
            }

            Tilemap.tilemapPositionsChanged += Tilemap_TilemapPositionsChanged;
            Tilemap.tilemapTileChanged += Tilemap_TilemapTileChanged;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Tilemap_TilemapPositionsChanged(Tilemap arg1, NativeArray<Vector3Int> arg2)
        {
            Debug.Log("tile position changed");
        }

        private void Tilemap_TilemapTileChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
        {
            Debug.Log("tile changed");
            foreach (var tileData in syncTiles)
            {
                var tmData = m_TilemapData.FirstOrDefault(d => d.Tilemap == tilemap);
                if (tmData != null)
                {
                    if (tileData.tile == null)
                    {
                        tmData.RemoveTile(tileData.position);
                    }
                    else
                    {
                        tmData.AddTile(tileData.position, tileData.tile);
                    }
                }
            }
        }

        public void OnBeforeSerialize()
        {
            //throw new NotImplementedException();
        }

        public void OnAfterDeserialize()
        {
            //throw new NotImplementedException();
        }
    }
}
