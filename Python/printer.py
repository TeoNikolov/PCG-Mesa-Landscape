import matplotlib.pyplot as plt
import numpy as np

def DrawMesaAgentI(agents, heightmap, drawfaults, drawfaultradius):
	plt.figure(figsize=[9, 9])
	plt.title("Generated Paths for " + str(len(agents)) + " Mesas")
	plt.xlabel("X")
	plt.ylabel("Y")
	plt.xlim([0, heightmap._width])
	plt.ylim([0, heightmap._height])
#	ax=plt.gca()                            # get the axis
#	ax.set_ylim(ax.get_ylim()[::-1])        # invert the axis
#	ax.xaxis.tick_top()                     # and move the X-Axis      
#	ax.yaxis.tick_left()
	plt.imshow(np.transpose(heightmap._data), cmap='gray_r')



	for a in agents:
		xcoords_origin = [a._pathsegments[0]._p0[0], a._pathsegments[0]._p1[0]]
		ycoords_origin = [a._pathsegments[0]._p0[1], a._pathsegments[0]._p1[1]]
		xcoords_normal = []
		ycoords_normal = []
		xcoords_extension = []
		ycoords_extension = []
		xcoords_faults = []
		ycoords_faults = []
		
		for f in a._faults:
			xcoords_faults.append(f.GetX())
			ycoords_faults.append(f.GetY())

		for i in range(1, len(a._pathsegments)):
			if a._pathsegments[i]._id == 1:
				xcoords_normal.append(a._pathsegments[i]._p0[0])
				xcoords_normal.append(a._pathsegments[i]._p1[0])
				ycoords_normal.append(a._pathsegments[i]._p0[1])
				ycoords_normal.append(a._pathsegments[i]._p1[1])
			else:
				xcoords_extension.append(a._pathsegments[i]._p0[0])
				xcoords_extension.append(a._pathsegments[i]._p1[0])
				ycoords_extension.append(a._pathsegments[i]._p0[1])
				ycoords_extension.append(a._pathsegments[i]._p1[1])

		plt.plot(xcoords_origin, ycoords_origin, color='green', linestyle='-')
		plt.plot(xcoords_normal, ycoords_normal, color='blue', linestyle='-')
		for i in range(0, len(xcoords_extension) - 1, 2):
			plt.plot([xcoords_extension[i], xcoords_extension[i + 1]],
			[ycoords_extension[i], ycoords_extension[i + 1]], color='red', linestyle='-')

		if drawfaultradius:
			for i in range(0, len(xcoords_faults)):
				plt.text(xcoords_faults[i], ycoords_faults[i] + 2, str(a._faults[i]._r), color="red", fontsize=12)

		if drawfaults:
			plt.plot(xcoords_faults, ycoords_faults, color='pink', marker='o', markersize='2', linestyle='')
	
		plt.plot(xcoords_origin, ycoords_origin, color='black', marker='o', markersize='2', linestyle='')
		plt.plot(xcoords_normal, ycoords_normal, color='black', marker='o', markersize='2', linestyle='')
		plt.plot(xcoords_extension, ycoords_extension, color='black', marker='o', markersize='2', linestyle='')
		
	plt.show()