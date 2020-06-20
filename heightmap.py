import numpy as np

class HeightMap:
   
    def __init__(self, width, height):
        self._data = np.zeros((width, height))
        self._width = width
        self._height = height

    # get the value of the heightmap at X and Y
    def GetValue(self, x, y):
        if x > self._data.shape[0] or y > self._data.shape[1] or x < 0 or y < 0:
            raise Exception("Supplying (" + str(x) + "," + str(y) +
                            ") when dimensions are (" + str(self._data.shape[0]) +
                            "," + str(self._data.shape[1]) +")")

        return self._data[x][y]
    
    # set the value of the heightmap at X and Y
    # value must be between 0.0 and 1.0
    def SetValue(self, x, y, value):
        if x > self._data.shape[0] or y > self._data.shape[1] or x < 0 or y < 0:
            raise Exception("Supplying (" + str(x) + "," + str(y) +
                            ") when dimensions are (" + str(self._data.shape[0]) +
                            "," + str(self._data.shape[1]) +")")

        #if value < 0 or value > 1:
        #   raise Exception("Setting heightmap value to " + str(value) + ". Must be between 0.0 and 1.0!")

        self._data[x][y] = value