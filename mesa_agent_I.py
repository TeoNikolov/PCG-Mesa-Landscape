import random, math, fault, sys
import math_functions as mf
import matplotlib.pyplot as plt

# computes mesa origins, constructs mesa paths, generates faults
class MesaAgentI:

	def __init__(self, id, hm_width, hm_height, scale, segments,
			  angularvar, extangularvar, lengthvar, extensionprob,
			  fradiusvar, fdistance, fcount, fheight):
		self._id = id
		self._width = hm_width
		self._height = hm_height
		self._scale = scale
		self._segments = segments
		self._angularvar = [math.radians(angularvar[0]), math.radians(angularvar[1])]
		self._extangularvar = [math.radians(extangularvar[0]), math.radians(extangularvar[1])]
		self._lengthvar = lengthvar
		self._extensionprob = extensionprob
		self._fradiusvar = fradiusvar
		self._fdistance = fdistance
		self._fcount = fcount
		self._fheight = fheight
		self._pathsegments = []
		self._faults = []

	def Run(self, heightmap, cfflist):
		self.GenerateMesaPath()
		self.GenerateFaults(cfflist)
		self.ApplyFaults(heightmap)

	def GenerateMesaPath(self):
		# initialize origin and random direction
		point = mf.RandomPointInSquare()
		px = int(self._width * (self._scale * (point[0] - 0.5) + 0.5))
		py = int(self._height * (self._scale * (point[1] - 0.5) + 0.5))

		start = (px, py)
		dir = random.random() * 2 * math.pi # [0, 2pi)
		sid = 0 # 0 - first line segment, 1 - normal line segment, 2 - extension line segment
		# construct path
		for i in range(0, random.randint(self._segments[0], self._segments[1])):
			oldstart = start
			olddir = dir
			start, dir = self.GenerateLineSegment(start, dir, sid)
			# if not first line segment, random extension
			if sid != 0:
				if random.random() < self._extensionprob:
					a = (dir + olddir) / 2.0
					a += math.pi / 2 if dir < olddir else -math.pi / 2
					self.GenerateLineSegment(oldstart, a, 2)
			sid = 1

	# generates and stores a line segment from a given starting point and direction
	# id: 0 - first line segment, 1 - normal line segment, 2 - extension line segment
	# returns the endpoint of the line segment, and direction of that line segment
	def GenerateLineSegment(self, start, dir, id):
		# perturbe the direction
		if id == 2:
			dir += self._extangularvar[0] + (self._extangularvar[1] - self._extangularvar[0]) * random.random()
		else:
			dir += self._angularvar[0] + (self._angularvar[1] - self._angularvar[0]) * random.random()

		# compute unit vector in that direction
		dirnormal = (math.cos(dir), math.sin(dir))
		# compute second endpoint
		l = self._lengthvar[0] + (self._lengthvar[1] - self._lengthvar[0]) * random.random()
		end = (start[0] + dirnormal[0] * l, start[1] + dirnormal[1] * l)
		self._pathsegments.append(mf.LineSegment(start, end, id))
		return end, dir

	# determine faults for this mesa and apply them to the heightmap
	def GenerateFaults(self, cfflist):
		# determine number of faults for line segment
		fps = []
		total_length = 0
		for s in self._pathsegments:
			total_length += s._length
		for s in self._pathsegments:
			fps.append(math.floor((self._fcount / total_length) * s._length))

		# distribute left-over faults sequentially among line segments
		leftover = self._fcount - sum(fps) # should never exceed the number of path segments
		for i in range(0, leftover):
			fps[i] += 1
	
		if (sum(fps) > self._fcount):
			raise Exception("Number of faults distributed among the path is greater than the specified fault count!")

		# generate fault info
		for i in range(len(self._pathsegments)):
			for j in range(fps[i]):
				s = self._pathsegments[i]

				# compute point between endpoints
				ratio = random.random()
				ptemp = (s._p0[0] * (1 - ratio) + s._p1[0] * ratio, s._p0[1] * (1 - ratio) + s._p1[1] * ratio)

				# choose on which side of the line segment the fault should be
				dd = math.pi / 2 if random.random() < 0.5 else -math.pi / 2

				# compute normal to line segment
				dir = s._dir + dd
				dirnormal = (math.cos(dir), math.sin(dir))
				
				# compute fault position
				dist = self._fdistance * random.random()
				fp = ptemp[0] + dirnormal[0] * dist, ptemp[1] + dirnormal[1] * dist
				
				# assign fault CFF (random fault radius)
				self._faults.append(fault.Fault(fp, cfflist[random.randrange(0, len(cfflist))]))

	def ApplyFaults(self, heightmap):
		for i in range(0, len(self._faults)):
			#print("Agent " + str(self._id) + ": Applying fault " + str(i) + "/" + str(len(self._faults)))
			sys.stdout.write("Agent " + str(self._id + 1) + ": Applying fault " + str(i + 1) + "/" + str(len(self._faults)))
			sys.stdout.flush()
			sys.stdout.write('\r')
			sys.stdout.flush()
			self._faults[i].Apply(heightmap)
		sys.stdout.write('\n')
		sys.stdout.flush()