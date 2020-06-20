import bisect

class MaterialList:
	def __init__(self, matlist):
		self._list = []
		for i in matlist:
			self.AddMaterial(i[0], i[1])

	# add a material to the list, keeping it sorted by material height
	def AddMaterial(self, height, hardness):
		if height < 0.0 or height > 1.0:
			raise Exception("Material height not between 0.0 and 1.0. Got " + str(height) + " instead.")

		bisect.insort(self._list, (height, hardness))

	# returns the material list
	def GetMaterialList(self):
		return self._list