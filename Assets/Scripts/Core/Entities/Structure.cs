using System;

namespace Assets.Scripts.Core.Entities
{
    [Serializable]
    public class Structure : Product
    {
        public Structure()
        {
            m_Name = "Structure";
            m_CanPassThrough = false;
            m_WalkOverSpeed = 0f;
        }

        public override EntityBaseType TileType => EntityBaseType.Structure;
    }
}
