using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Entity_Components
{
    public class TileEntity
    {
        public List<EntityMaterial> Materials { get; }
        public string EntityName { get; set; }

        public TileEntity()
        {
            Materials = new List<EntityMaterial>();
        }
    }
}
