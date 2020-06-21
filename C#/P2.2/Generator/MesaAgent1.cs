using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace P2._2.Generator
{
    public class MesaAgent1
    {
        int id;
        int width;
        int height;
        float scale;
        (int low, int high) segments;
        (int low, int high) angularvar;
        (int low, int high) extangularvar;
        (int low, int high) lengthvar;
        float extensionprob;
        (int low, int high) fradiusvar;
        int fdistance;
        int fcount;
        float fheight;
        List<LineSegment> pathsegments;
        List<Fault> faults;

        public MesaAgent1(int id, GenProperties gp) {
            this.id = id;
            width = gp.hm_width;
            height = gp.hm_height;
            scale = gp.m_scale;
            segments = gp.mp_segments;
            angularvar = gp.mp_angularvar;
            extangularvar = gp.mp_extangularvar;
            lengthvar = gp.mp_lengthvar;
            extensionprob = gp.mp_extensionprob;
            fradiusvar = gp.f_radiusvar;
            fdistance = gp.f_distance;
            fcount = gp.f_count;
            fheight = gp.f_height;
            pathsegments = new List<LineSegment>();
            faults = new List<Fault>();
        }

        public void Run(Heightmap heightmap, List<CFFKernel> cfflist) {
            GenerateMesaPath();
            GenerateFaults(cfflist);
            ApplyFaults(heightmap);
        }

        public void GenerateMesaPath() {
            //Initialise origin and random direction
            (float x, float y) point = MF.RandomPointInSquare();
            int px = (int)(width * (scale * (point.x - 0.5) + 0.5));
            int py = (int)(height * (scale * (point.y - 0.5) + 0.5));

            (float x, float y) start = (px, py);
            float dir = (float)(MF.Random.NextDouble() * 2 * Math.PI);
            int sid = 0; // 0 - First line segment, 1 - Normal line segment, 2 - Extension line segment

            // Construct path
            int imax = MF.Random.Next(segments.low, segments.high);
            for(int i = 0; i <= imax; i++) {
                (float x, float y) oldstart = start;
                float olddir = dir;
                (start, dir) = GenerateLineSegment(start, dir, sid);
                if(sid != 0) {
                    if(MF.Random.NextDouble() < extensionprob) {
                        float a = (dir + olddir) / 2;
                        a += dir < olddir ? (float)Math.PI / 2 : (float)-Math.PI / 2;
                        GenerateLineSegment(oldstart, a, 2);
                    }
                }
                sid = 1;
            }
        }
        // Generates and stores a line segment from a given starting point and direction
        // id: 0 - first line segment, 1 - normal line segment, 2 - extension line segment
        // Returns the endpoint of the line segment, and direction of that line segment
        public ((float x, float y), float) GenerateLineSegment((float x, float y) start, float dir, int id) {
            // Perturb the direction 
            if (id == 2)
                dir += extangularvar.low + (extangularvar.high - extangularvar.low) * (float)MF.Random.NextDouble();
            else
                dir += angularvar.low + (angularvar.high - angularvar.low) * (float)MF.Random.NextDouble();

            // Compute unit vector in that direction
            (float x, float y) dirnormal = ((float)Math.Cos(dir), (float)Math.Sin(dir));
            // Compute second endpoint
            float l = lengthvar.low + (lengthvar.high - lengthvar.low) * (float)MF.Random.NextDouble();
            (float x, float y) end = (start.x + dirnormal.x * l, start.y + dirnormal.y * l);
            pathsegments.Add(new LineSegment(start, end, id));
            return (end, dir);
        }

        // Determine faults for this mesa and apply them to the heightmap
        public void GenerateFaults(List<CFFKernel> cfflist) {
            // Determine number of faults for line segment
            List<int> fps = new List<int>();
            float total_length = 0;
            foreach (LineSegment s in pathsegments){
                total_length += s.length;
            }
            foreach(LineSegment s in pathsegments) {
                fps.Add((int)(fcount / total_length * s.length));
            }

            // Distribute leftover faults sequentially amoung line segments
            int leftover = fcount - fps.Sum();
            for(int i = 0; i < leftover; i++) {
                fps[i] += 1;
            }

            if (fps.Sum() > fcount) throw new Exception("Number of faults distributed among the path is greater than the specified fault count!");

            // Generate fault info
            for(int i = 0; i < pathsegments.Count; i++) {
                for(int j = 0; j < fps[i]; j++) {
                    LineSegment s = pathsegments[i];

                    // Compute point between endpoints
                    float ratio = (float)MF.Random.NextDouble();
                    (float x, float y) ptemp = (s.p0.x * (1 - ratio) + s.p1.x * ratio, s.p0.y * (1 - ratio) + s.p1.y * ratio);

                    // Choose on which side of the line segment the fault should be
                    float dd = MF.Random.NextDouble() < 0.5 ? (float)Math.PI / 2 : (float)-Math.PI / 2;

                    // Compute normal to line segment
                    float dir = s.dir + dd;
                    (float x, float y) dirnormal = ((float)Math.Cos(dir), (float)Math.Sin(dir));

                    // Compute fault position
                    float dist = (float)(fdistance * MF.Random.NextDouble());
                    (int x, int y) fp = ((int)Math.Round(ptemp.x + dirnormal.x * dist), (int)Math.Round(ptemp.y + dirnormal.y * dist));

                    // Assign fault CFF (random fault radius)
                    faults.Add(new Fault(fp, cfflist[MF.Random.Next(0, cfflist.Count)]));
                }
            }
        }

        public void ApplyFaults(Heightmap heightmap) {
            for(int i = 0; i < faults.Count; i++) {
                Console.WriteLine($"Agent {id + 1}: Applying fault {i + 1}/{faults.Count}");
                faults[i].Apply(heightmap);
            }
            Console.WriteLine();
        }
    }
}
