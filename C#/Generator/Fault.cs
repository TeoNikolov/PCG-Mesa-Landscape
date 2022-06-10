using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesaGen.Generator
{
    public class Fault
    {
        (int x, int y) pos;
        CFFKernel CFF;
        int r;

        public Fault((int x, int y) pos, CFFKernel CFF) {
            this.pos = pos;
            this.CFF = CFF;
            r = CFF.r;
        }

        public void Apply(Heightmap heightmap) {
            CFF.Apply(heightmap, GetX(), GetY());
        }

        public int GetX() {
            return pos.x;
        }

        public int GetY() {
            return pos.y;
        }

        public override string ToString() {
            return $"({pos.x}, {pos.y}, r = {r})";
        }
    }
}
