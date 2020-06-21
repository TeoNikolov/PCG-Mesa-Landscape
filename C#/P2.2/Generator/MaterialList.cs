using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2._2.Generator
{
    public class MaterialList
    {
        (float height, float hardness)[] matlist;

        public MaterialList((float,float)[] matlist) {
            this.matlist = matlist;
            this.matlist.OrderBy(x => x.height);
        }

        public (float height, float hardness)[] GetMaterialList() {
            return matlist;
        }
    }
}
