using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace P2._2.Generator
{
    static class MF
    {
        static Random r = new Random(1337);
        public static (int, int) UnitPointToMapCoords(float x, float y, int mapwidth, int mapheight) {
            return ((int)(x * (mapwidth - 1)), (int)(y * (mapheight - 1)));
        }

        public static (float, float) MapCoordsToUnitPoint(float x, float y, int mapwidth, int mapheight) {
            return ((mapwidth - 1) / x, (mapheight - 1) / y);
        }

        public static (float, float) RandomPointInSquare() {
            return ((float)r.NextDouble(), (float)r.NextDouble());
        }

        public static Random Random {
            get => r;
        }
    }

    public class LineSegment
    {
        public (float x, float y) p0 { get; }
        public (float x, float y) p1 { get; }
        public float length { get; }
        public float dir { get; }
        public int id { get; }

        public LineSegment((float, float) p0, (float, float) p1, int id) {
            this.p0 = p0;
            this.p1 = p1;
            (float, float) rls = (this.p1.x - this.p0.x, this.p1.y - this.p0.y);
            length = (float)Math.Sqrt(rls.Item1 * rls.Item1 + rls.Item2 * rls.Item2);
            dir = (float)Math.Atan2(rls.Item2, rls.Item1);
            this.id = id;
        }

        public override string ToString() {
            return $"({p0.x},{p0.y}), ({p1.x},{p1.y}), l:{length}, d:{(180 / Math.PI) * dir}";
        }
    }
}
