using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using P2._2;

namespace P2._2.Generator
{
    public class CFFKernel
    {
        public static List<CFFKernel> InitKernals(GenProperties gp) {
            List<CFFKernel> kernels = new List<CFFKernel>();
            for(int r = gp.f_radiusvar.low; r <= gp.f_radiusvar.high; r++) {
                kernels.Add(new CFFKernel(r, gp.f_height));
            }
            return kernels;
        }

        public int r { get; }
        int size, anchor;
        float[,] data;

        public CFFKernel(int radius, float height) {
            if (height < 0) throw new ArgumentException($"CFFKernel received a height of {height}, must be above 0.0!");

            // Process radius
            r = radius;
            size = r * 2 - 1;
            data = new float[size, size];
            anchor = (size - 1) / 2;

            // Compute kernel data (CFF)
            for(int y = 0; y < size; y++) {
                for(int x = 0; x < size; x++) {
                    int s = r * r - (x - anchor) * (x - anchor) - (y - anchor) * (y - anchor);
                    if (s <= 0) continue;
                    data[x, y] = (float)(Math.Sqrt(s) / r) * height;
                }
            }

        }

        public void Apply(Heightmap heightmap, int x, int y) {
            // Translated CFF Corner coordinates
            (int x, int y) p1 = (x - anchor, y - anchor);
            (int x, int y) p2 = (x + anchor, y + anchor);

            if (p1.x >= heightmap.width || p1.y >= heightmap.height || p2.x < 0 || p2.y < 0) return;

            int inset_l = p1.x >= 0 ? 0 : Math.Abs(p1.x);
            int inset_t = p1.y >= 0 ? 0 : Math.Abs(p1.y);
            int inset_r = p2.x < heightmap.width ? 0 : Math.Abs(p2.x - (heightmap.width - 1));
            int inset_b = p2.y < heightmap.height ? 0 : Math.Abs(p2.y - (heightmap.height - 1));

            if (inset_r + inset_l >= size || inset_t + inset_b > size) throw new Exception("Kernel insets exceeded kernel size! Maybe kernel outside map?");

            for(int dy = inset_t; dy < size - inset_b; dy++) {
                for(int dx = inset_l; dx < size - inset_r; dx++) {
                    int hm_x = x - anchor + dx;
                    int hm_y = y - anchor + dy;
                    float v = heightmap[hm_x, hm_y];
                    float hm_v = v + data[dx, dy];
                    heightmap[hm_x, hm_y] = hm_v;
                }
            }
        }
    }
}
