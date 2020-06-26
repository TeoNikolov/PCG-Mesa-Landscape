using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace P2._2.Generator
{
    // A global agent exposing heightmap-wide operations, such as Geometry gradation and Geometry levelling
    // The operations have no requirements of the heightmap and can be called at any time
    // As such, there is no specific Run() sequence for this agent
    public class MesaAgent2
    {
        private static int _idcounter = 1;

        int id;
        int width;
        int height;
        float fheight;
        float erosion_factor;
        float caprock_height;
        int levelling_radius;
        (float, float)[] mat_list;
        Heightmap hm;
        Heightmap matMap;
        Heightmap hardnessMap;

        public MesaAgent2(Heightmap heightmap, GenProperties gp) {
            id = _idcounter++;
            width = heightmap.width;
            height = heightmap.height;
            fheight = gp.f_height;
            mat_list = gp.mat_list;
            hm = heightmap;
            erosion_factor = gp.erosion_factor;
            caprock_height = gp.caprock_height;
            levelling_radius = gp.levelling_radius;
        }

        public void Run()
        {
            float peak = hm.Max();
            NormalizeHeightmap();
            GenerateMaterialsLayer();
            GenerateHardnessLayer();
            GradateGeometry();
            LevelGeometry();
            hm.Scale(peak);
        }

        private void NormalizeHeightmap()
        {
            hm.Normalize();
        }

        private void GenerateMaterialsLayer()
        {
            matMap = new Heightmap(hm.width, hm.height);
            for (int y = 0; y < hm.height; y++)
            {
                for (int x = 0; x < hm.width; x++)
                {
                    int mat_ID = -1;
                    for (int mat = 0; mat < mat_list.Length; mat++)
                    {
                        if (hm[x, y] < mat_list[mat].Item1) break;
                        mat_ID = mat;
                    }
                    matMap[x, y] = mat_ID;
                }
            }
        }

        private void GenerateHardnessLayer()
        {
            if (matMap == null)
            {
                throw new Exception(this + " material map not instantiated! How did this happen?");
            }

            hardnessMap = new Heightmap(matMap.width, matMap.height);
            for (int y = 0; y < matMap.height; y++)
            {
                for (int x = 0; x < matMap.width; x++)
                {
                    hardnessMap[x, y] = mat_list[(int)matMap[x,y]].Item2;
                }
            }
        }

        // this is a destructive step - it modifies the original heightmap!
        private void GradateGeometry()
        {
            for (int y = 0; y < hm.height; y++)
            {
                for (int x = 0; x < hm.width; x++)
                {
                    hm[x, y] -= erosion_factor * (1.0f - hardnessMap[x, y]);
                    if (hm[x, y] > caprock_height) hm[x, y] = caprock_height;
                }
            }
        }

        private void LevelGeometry()
        {
            int size = levelling_radius * 2 - 1;
            int anchor = (size - 1) / 2;

            for (int y = 0; y < hm.height; y++)
            {
                for (int x = 0; x < hm.width; x++)
                {
                    // Translated CFF Corner coordinates
                    (int x, int y) p1 = (x - anchor, y - anchor);
                    (int x, int y) p2 = (x + anchor, y + anchor);

                    if (p1.x >= hm.width || p1.y >= hm.height || p2.x < 0 || p2.y < 0) return;

                    int inset_l = p1.x >= 0 ? 0 : Math.Abs(p1.x);
                    int inset_t = p1.y >= 0 ? 0 : Math.Abs(p1.y);
                    int inset_r = p2.x < hm.width ? 0 : Math.Abs(p2.x - (hm.width - 1));
                    int inset_b = p2.y < hm.height ? 0 : Math.Abs(p2.y - (hm.height - 1));

                    if (inset_r + inset_l >= size || inset_t + inset_b > size) throw new Exception("Kernel insets exceeded kernel size! Maybe kernel outside map?");

                    float accumulator = 0;
                    int acc_count = 0;
                    for (int dy = inset_t; dy < size - inset_b; dy++)
                    {
                        for (int dx = inset_l; dx < size - inset_r; dx++)
                        {
                            int hm_x = x - anchor + dx;
                            int hm_y = y - anchor + dy;
                            if (hm_x == x && hm_y == y) continue; // must ignore the current point
                            accumulator += hm[hm_x, hm_y];
                            acc_count++;
                        }
                    }

                    if (acc_count == 0)
                    {
                        throw new Exception("Accumulator count was 0 and it should be impossible! How did this happen?");
                    }

                    hm[x, y] = (accumulator / acc_count + hm[x, y]) / 2.0f;
                }
            }
        }

        public override string ToString()
        {
            return "Mesa Agent II " + id;
        }
    }
}
