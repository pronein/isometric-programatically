using Assets.Scripts.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tilemaps
{
    [Serializable]
    public class TileMetadata
    {
        [SerializeField]
        private EntityBase m_CurrentEntityMetadata;

        [SerializeField]
        private List<EntityMaterial> m_MaterialComposition;

        private List<EntityBase> m_Entities;

        public TileMetadata()
        {
            m_Entities = new List<EntityBase>();
        }

        public List<EntityMaterial> Materials { get { return m_MaterialComposition; } set { m_MaterialComposition = value; } }

        public EntityBase getEntityMetadata(EntityBaseType tileType)
        {
            var entityBase = m_Entities.FirstOrDefault(e => e.TileType == tileType);
            
            if (entityBase == null)
            {
                switch(tileType)
                {
                    case EntityBaseType.Matter:
                        entityBase = new Matter();
                        entityBase.AddMaterial(new Material());
                        break;

                    case EntityBaseType.Product:
                        entityBase = new Product();
                        break;

                    case EntityBaseType.Structure:
                        entityBase = new Structure();
                        break;
                }

                m_Entities.Add(entityBase);
            }

            return m_CurrentEntityMetadata = entityBase;
        }
    }
}
