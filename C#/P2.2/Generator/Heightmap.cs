using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2._2.Generator
{
    public class Heightmap
    {
        float[,] data;
        public int width { get; }
        public int height { get; }

        public Heightmap(int width, int height) {
            this.width = width;
            this.height = height;
            data = new float[width, height];
        }

        public float this[int x, int y] {
            get {
                return data[x, y];
            }
            set {
                data[x, y] = value;
            }
        }

        public float Max() {
            float max = 0;
            foreach (float f in data)
                max = f > max ? f : max;
            return max;
        }
    }
}
