using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesaGen.Generator
{
    public class GenProperties
    {
        public static int seed {set;  get; }
        public int hm_width{get;} 
        public int hm_height {get;} 
        public int hm_cellsize {get;} 
        public int m_origins {get;} 
        public float m_scale {get;} 
        public (int low, int high) mp_segments {get;} 
        public (int low, int high) mp_angularvar {get;}
        public (int low, int high) mp_extangularvar {get;} 
        public (int low, int high) mp_lengthvar {get;} 
        public float mp_extensionprob {get;} 
        public (int low, int high) f_radiusvar {get;} 
        public int f_distance {get;} 
        public int f_count {get;}
        public float f_height {get;} 
        public (float height, float hardness)[] mat_list {get;}
        public float erosion_factor {get;}
        public float caprock_height {get;}
        public int levelling_radius {get;}
        public int road_maxdist {get;} 
        public int road_iterations {get;} 
        public int road_width {get;}

        public GenProperties(int hm_width,
        int hm_height,
        int hm_cellsize,
        int m_origins,
        float m_scale,
        (int, int) mp_segments,
        (int, int) mp_angularvar,
        (int, int) mp_extangularvar,
        (int, int) mp_lengthvar,
        float mp_extensionprob,
        (int, int) f_radiusvar,
        int f_distance,
        int f_count,
        float f_height,
        (float, float)[] mat_list,
        float erosion_factor,
        float caprock_height,
        int levelling_radius,
        int road_maxdist,
        int road_iterations,
        int road_width) {
            this.hm_width = hm_width;
            this.hm_height = hm_height;
            this.hm_cellsize = hm_cellsize;
            this.m_origins = m_origins;
            this.m_scale = m_scale;
            this.mp_segments = mp_segments;
            this.mp_angularvar = mp_angularvar;
            this.mp_extangularvar = mp_extangularvar;
            this.mp_lengthvar = mp_lengthvar;
            this.mp_extensionprob = mp_extensionprob;
            this.f_radiusvar = f_radiusvar;
            this.f_distance = f_distance;
            this.f_count = f_count;
            this.f_height = f_height;
            this.mat_list = mat_list;
            this.erosion_factor = erosion_factor;
            this.caprock_height = caprock_height;
            this.levelling_radius = levelling_radius;
            this.road_maxdist = road_maxdist;
            this.road_iterations = road_iterations;
            this.road_width = road_width;
        }
    }
}
