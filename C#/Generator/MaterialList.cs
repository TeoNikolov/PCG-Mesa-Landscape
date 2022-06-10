using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesaGen.Generator
{
    public class MaterialList
    {
        (float height, float hardness)[] matlist;

        public MaterialList((float,float)[] matlist) {
            this.matlist = matlist;

            // lexicographically sort the material list
            Array.Sort(this.matlist);

            // get the max hardness
            float maxHardness = -1;
            foreach ((float, float) m in matlist)
            {
                if (m.Item2 > maxHardness) maxHardness = m.Item2;
            }

            // normalize the materials' hardness
            for (int i = 0; i < matlist.Length; i++)
            {
                matlist[i].Item2 /= maxHardness;
            }

            // ensure lowest material has a height of 0.0
            matlist[0].Item1 = 0.0f;
        }

        public (float height, float hardness)[] GetMaterialList() {
            return matlist;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < matlist.Length; i++)
            {
                result += matlist[i].Item1 + " : " + matlist[i].Item2 + "\n";
            }
            return result;
        }
    }
}
