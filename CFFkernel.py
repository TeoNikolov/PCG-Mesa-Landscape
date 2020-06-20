import numpy as np
import math

def InitKernels(radiusvar, height):
	kernels = []
	for r in range(radiusvar[0], radiusvar[1] + 1):
		kernels.append(CFFKernel(r, height))
	return kernels

class CFFKernel:
	def __init__(self, r, height):
		if height < 0:
			raise Exception("CFFKernel was supplied a height of " + str(height) + ". Must be above 0.0!")

		# process radius
		self._r = math.ceil(r)
		self._size = self._r * 2 - 1 # should be + 1, but the eddge values will always be 0
		self._data = np.zeros((self._size, self._size))
		self._anchor = math.floor((self._size - 1) / 2)

		# compute kernel data (CFF)
		for y in range(0, self._size):
			for x in range(0, self._size):
				s = self._r**2 - (x - self._anchor)**2 - (y - self._anchor)**2
				if s <= 0:
					continue
				self._data[x][y] = (math.sqrt(s) / r) * height

	def Apply(self, heightmap, x, y):
		# corner coordinates
		p1 = (x - self._anchor, y - self._anchor) # top-left corner
		p2 = (x + self._anchor, y + self._anchor) # bottom-right corner
		
		if p1[0] >= heightmap._width or p1[1] >= heightmap._height or p2[0] < 0 or p2[1] < 0:
			print("I: Kernel outside heightmap bounds.")
			return
		
		inset_l = 0 if p1[0] >= 0 else abs(p1[0])
		inset_t = 0 if p1[1] >= 0 else abs(p1[1])
		inset_r = 0 if p2[0] < heightmap._width else (p2[0] - (heightmap._width - 1))
		inset_b = 0 if p2[1] < heightmap._height else (p2[1] - (heightmap._height - 1))

		if (inset_r + inset_l) >= self._size or (inset_t + inset_b) >= self._size:
			raise Exception("Kernel insets exceeded kernel size! Maybe kernel outside map?")
	
		for dy in range(inset_t, self._size - inset_b):
			for dx in range(inset_l, self._size - inset_r):
				hm_x = x - self._anchor + dx
				hm_y = y - self._anchor + dy
				v = heightmap.GetValue(hm_x, hm_y)
				#hm_v = v + (1 - v) * self._data[dx][dy]
				hm_v = v + self._data[dx][dy]
				heightmap.SetValue(hm_x, hm_y, hm_v)