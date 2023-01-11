using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Entities
{
    [Serializable]
    public class Product : EntityBase
    {
        [SerializeField]
        private List<Product> _productCompositionKeys;
        [SerializeField]
        private List<float> _productCompositionValues;
        protected Dictionary<Product, float> m_ProductComposition;

        public Product()
        {
            m_CanPassThrough = true;
            m_Name = "Product";
            m_PassThroughSpeed = 0.75f;
            m_ProductComposition = new Dictionary<Product, float>();
        }

        public override EntityBaseType TileType => EntityBaseType.Product;

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            _productCompositionKeys.Clear();
            _productCompositionValues.Clear();

            foreach (var kvp in m_ProductComposition)
            {
                _productCompositionKeys.Add(kvp.Key);
                _productCompositionValues.Add(kvp.Value);
            }
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            m_ProductComposition = new Dictionary<Product, float>();

            for (int i = 0, len = Math.Min(_productCompositionKeys.Count, _productCompositionValues.Count); i < len; i++)
            {
                m_ProductComposition.Add(_productCompositionKeys[i], _productCompositionValues[i]);
            }
        }
    }
}
