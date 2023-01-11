using System;

namespace Assets.Scripts.Core.Entities
{
    [Serializable]
    public class Matter : EntityBase
    {
        public Matter()
        {
            m_Name = "Matter";
        }

        public override EntityBaseType TileType => EntityBaseType.Matter;
    }
}
