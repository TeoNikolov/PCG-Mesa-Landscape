#! /usr/bin/env python

from rendering import Renderer
import random
import numbers
import numpy as np
import heightmap as hmp
import material, printer
import mesa_agent_I as maI
import mesa_agent_II as maII
import matplotlib.pyplot as plt
import CFFkernel as CFF

def ValidateInput(hm_width, hm_height, hm_cellsize, m_origins, m_scale, mp_segments, mp_angularvar,
               mp_extangularvar, mp_lengthvar, mp_extensionprob, f_radiusvar, f_distance, f_count,
               f_height, mat_list, road_maxdist, road_iterations, road_width):
    if not isinstance(hm_width, int):
        raise Exception("Heightmap width value must be an int!")
    if hm_width <= 0:
        raise Exception("Heightmap width must be greater than 0!")

    if not isinstance(hm_height, int):
        raise Exception("Heightmap height value must be an int!")
    if hm_height <= 0:
        raise Exception("Heightmap height must be greater than 0!")

    if not isinstance(hm_cellsize, int):
        raise Exception("Cell size value must be an int!")
    if hm_cellsize <= 0:
        raise Exception("Cell size must be greater than 0!")

    if not isinstance(m_origins, int):
        raise Exception("Number of mesa origins value must be an int!")
    if m_origins <= 0:
        raise Exception("Number of mesa origins must be greater than 0!")
    
    if not isinstance(m_scale, numbers.Number):
        raise Exception("Origin scale value must be number!")
    if m_scale <= 0:
        raise Exception("Origin scale must be greater than 0!")

    if not isinstance(mp_segments[0], int):
        raise Exception("Mesa path segments LOW value must be a number!")
    if not isinstance(mp_segments[1], int):
        raise Exception("Mesa path segments HIGH value must be a number!")
    if mp_segments[0] > mp_segments[1]:
        raise Exception("Mesa path segments LOW must be lower or equal to Mesa path segments HIGH!")

    if not isinstance(mp_angularvar[0], numbers.Number):
        raise Exception("Angular variation LOW value must be a number!")
    if not isinstance(mp_angularvar[1], numbers.Number):
        raise Exception("Angular variation HIGH value must be a number!")
    if mp_angularvar[0] > mp_angularvar[1]:
        raise Exception("Angular variation LOW must be lower or equal to angular variation HIGH!")
    if mp_angularvar[0] <= -180:
        raise Exception("Angular variation LOW must be strictly greater than -180 degrees!")
    if mp_angularvar[1] >= 180:
        raise Exception("Angular variation HIGH must be strictly lower than 180 degrees!")

    if not isinstance(mp_extangularvar[0], numbers.Number):
        raise Exception("Extension angular variation LOW value must be a number!")
    if not isinstance(mp_extangularvar[1], numbers.Number):
        raise Exception("Extension angular variation HIGH value must be a number!")
    if mp_extangularvar[0] > mp_extangularvar[1]:
        raise Exception("Extension angular variation LOW must be lower or equal to extension angular variation HIGH!")
    if mp_extangularvar[0] <= -180:
        raise Exception("Extension angular variation LOW must be strictly greater than -180 degrees!")
    if mp_extangularvar[1] >= 180:
        raise Exception("Extension angular variation HIGH must be strictly lower than 180 degrees!")

    if not isinstance(mp_lengthvar[0], numbers.Number):
        raise Exception("Length variation LOW value must be a number!")
    if not isinstance(mp_lengthvar[1], numbers.Number):
        raise Exception("Length variation HIGH value must be a number!")
    if mp_lengthvar[0] > mp_lengthvar[1]:
        raise Exception("Length variation LOW must be lower or equal to length variation HIGH!")
    
    if not isinstance(mp_extensionprob, numbers.Number):
        raise Exception("Extension probability value must be a number!")
    if mp_extensionprob < 0.0 or mp_extensionprob > 1.0:
        raise Exception("Extension probability value must be between 0.0 and 1.0!")

    if not isinstance(f_radiusvar[0], int):
        raise Exception("Fault radius LOW value must be an integer!")
    if not isinstance(f_radiusvar[1], int):
        raise Exception("Fault radius HIGH value must be an integer!")
    if f_radiusvar[0] > f_radiusvar[1]:
        raise Exception("Fault radius LOW must be lower or equal to fault radius HIGH!")
        
    if not isinstance(f_distance, numbers.Number):
        raise Exception("Fault distance value must be number!")
    if f_distance < 0:
        raise Exception("Fault distance must be greater or equal to 0!")

    if not isinstance(f_count, int):
        raise Exception("Fault count value must be an integer!")
    if f_count <= 0:
        raise Exception("Fault count must be greater than 0!")

    if not isinstance(f_height, numbers.Number):
        raise Exception("Fault height value must be a number!")
    if f_height <= 0:
        raise Exception("Fault height value must be above 0.0!")

    if not isinstance(road_maxdist, numbers.Number):
        raise Exception("Road max distance value must be number!")
    if road_maxdist <= 0:
        raise Exception("Road max distance must be greater than 0!")

    if not isinstance(road_iterations, int):
        raise Exception("Road iterations value must be an integer!")
    if road_iterations <= 0:
        raise Exception("Road iterations must be greater than 0!")

    if not isinstance(road_width, numbers.Number):
        raise Exception("Road width value must be number!")
    if road_width <= 0:
        raise Exception("Road width must be greater than 0!")

    print("Valid input. Warning: material list not checked.")

if __name__ == '__main__':
    # input variables
    _seed = 44
    hm_width = 4096             # heightmap width
    hm_height = 4096            # heightmap heigh
    hm_cellsize = 1             # size of heightmap cell                - meters
    m_origins = 4               # mesa origins
    m_scale = 1.0               # mesa origin scale
    mp_segments = [1, 4]        # mesa path line segments
    mp_angularvar = [-50, 50]               # mesa path angular variation           - degrees
    mp_extangularvar = [-15, 15]      # mesa path extension angular variation - degrees
    mp_lengthvar = [20, 200]      # mesa path length variation            - meters
    mp_extensionprob = 0.1     # probability for mesa path extension
    f_radiusvar = [150, 150]        # fault radius variation                - meters
    f_distance = 200             # fault distance from mesa path         - meters
    f_count = 1000                # fault count per mesa
    f_height = 1.0              # max fault height
    mat_list = [(0.2, 50.0),
                (0.3, 100.0),
                (0.0, 1.0),
                (0.2, 10.0),
                (0.7, 1000.0)]  # material list
    road_maxdist = 20           # max dist for next road segment        - meters
    road_iterations = 50        # max road placement attemps
    road_width = 10             # road width                            - meters

    ValidateInput(hm_width, hm_height, hm_cellsize, m_origins, m_scale, mp_segments, mp_angularvar,
               mp_extangularvar, mp_lengthvar, mp_extensionprob, f_radiusvar, f_distance, f_count,
               f_height, mat_list, road_maxdist, road_iterations, road_width)

    # init
    random.seed(_seed)
    app = Renderer()
    heightmap = hmp.HeightMap(hm_width, hm_height)
    matlist = material.MaterialList(mat_list)
    CFFkernels = CFF.InitKernels(f_radiusvar, f_height)

    masI = []
    for i in range(0, m_origins):
        magent = maI.MesaAgentI(i, hm_width, hm_height, m_scale, mp_segments,
                                mp_angularvar, mp_extangularvar, mp_lengthvar, mp_extensionprob,
                                f_radiusvar, f_distance, f_count, f_height)
        magent.Run(heightmap, CFFkernels)
        masI.append(magent)

    printer.DrawMesaAgentI(masI, heightmap, drawfaults=False, drawfaultradius=False)

    # wait for input (prevent render window from popping up over OS shell)
    #input("Waiting for input...")

    # finally, render the heightmap
    app.render_heightmap(heightmap._data, hm_cellsize)
    app.run()