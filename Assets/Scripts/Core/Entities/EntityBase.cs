using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Entities
{
    [Serializable]
    public abstract class EntityBase : ISerializationCallbackReceiver
    {
        [SerializeField]
        protected bool m_CanPassThrough = false;
        [SerializeField]
        protected float m_PassThroughSpeed = 0f;
        [SerializeField]
        protected float m_WalkOverSpeed = 1f;
        [SerializeField]
        protected string m_Name = "Entity";

        [SerializeField]
        private List<Material> _materialKeys;
        [SerializeField]
        private List<float> _materialValues;
        protected Dictionary<Material, float> m_MaterialComposition;

        public EntityMatterState MatterState { get; set; } = EntityMatterState.Solid;

        public bool CanPassThrough { get { return m_CanPassThrough; } }
        public float PassThroughSpeed { get { return m_PassThroughSpeed; } }
        public float WalkOverSpeed { get { return m_WalkOverSpeed; } }

        public EntityBase()
        {
            m_MaterialComposition = new Dictionary<Material, float>();
        }

        public virtual EntityBaseType TileType { get; }

        public void AddMaterial(Material material)
        {
            m_MaterialComposition.Add(material, 0f);
        }

        public virtual void OnBeforeSerialize()
        {
            _materialKeys.Clear();
            _materialValues.Clear();

            foreach(var kvp in m_MaterialComposition)
            {
                _materialKeys.Add(kvp.Key);
                _materialValues.Add(kvp.Value);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            m_MaterialComposition = new Dictionary<Material, float>();

            for(int i = 0, len = Math.Min(_materialKeys.Count, _materialValues.Count); i < len; i++)
            {
                m_MaterialComposition.Add(_materialKeys[i], _materialValues[i]);
            }
        }
    }
}
