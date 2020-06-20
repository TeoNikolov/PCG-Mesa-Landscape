class Fault:
    def __init__(self, pos, CFF):
        self._pos = pos
        self._CFF = CFF
        self._r = CFF._r

    def Apply(self, heightmap):
        self._CFF.Apply(heightmap, self.GetX(), self.GetY())

    def GetX(self):
        return int(self._pos[0])

    def GetY(self):
        return int(self._pos[1])


    def __str__(self):
        return "(" + str(self._pos[0]) + "," + str(self._pos[1]) + ", r = " + str(self._r) + ")"