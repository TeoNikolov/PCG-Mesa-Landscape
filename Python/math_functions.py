import random
import math

class LineSegment:
    def __init__(self, p0, p1, id):
        self._p0 = p0
        self._p1 = p1
        rls = (p1[0] - p0[0], p1[1] - p0[1]) # relative line segment from END to START endpoint
        self._length = math.sqrt(rls[0] * rls[0] + rls[1] * rls[1])
        self._dir = math.atan2(rls[1], rls[0])
        self._id = id
    
    def __str__(self):
        return str(self._p0) + ", " + str(self._p1) + ", l = " + str(self._length) + ", d = " + str(math.degrees(self._dir))

# maps a point on a unit square to cell coordinates on a grid map
def UnitPointToMapCoords(x, y, mapwidth, mapheight):
    return (int(x * (mapwidth - 1)), int(y * (mapheight - 1)))

# maps cell coordinates on a grid map to a point on a unit square
def MapCoordsToUnitPoint(x, y, mapwidth, mapheight):
    return ((mapwidth - 1) / x, (mapheight - 1) / y)

def RandomPointInSquare():
    x = random.random()
    y = random.random()
    return (x, y)