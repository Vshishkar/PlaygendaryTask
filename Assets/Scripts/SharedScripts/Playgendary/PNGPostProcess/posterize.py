#! /usr/bin/python
import png
import sys
import shutil
import math
import string

if len(sys.argv) != 3:
	print 'Invalid arguments!'
	print 'Use -> ./posterize.py <filename> <posterizeValue>'
	sys.exit()

filename = sys.argv[1]
posterizeValue = int(sys.argv[2])

#if posteraizeValue < 2 || posteraizeValue > 256:
#    print 'Invalid posterize value! Must be integer in range (2, 256)!'
#    sys.exit()

print 'start posterization for file: ', filename

shutil.copyfile(filename, string.replace(filename, '.png', '_orig.png'))

#ROW Generator
def posterizeRow(row, channelsPerPixel, numOfAreas, numOfValues):
	for i in range(len(row) / channelsPerPixel):
		r = row[i * channelsPerPixel]
		g = row[i * channelsPerPixel + 1]
		b = row[i * channelsPerPixel + 2]
					
		#R	
		temp = math.floor(r / numOfAreas)
		r = int(math.floor(numOfValues * temp))
		
		#G
		temp = math.floor(g / numOfAreas)
		g = int(math.floor(numOfValues * temp))
		
		#B
		temp = math.floor(b / numOfAreas)
		b = int(math.floor(numOfValues * temp))	
		
		yield r
		yield g
		yield b
		
		if hasAlpha:
			yield row[i * channelsPerPixel + 3]

#Posterize generator
def posterize(png, posteraizeValue):	
	channelsPerPixel =  4 if hasAlpha else 3
	
	numOfAreas = 256.0 / posterizeValue
	numOfValues = 255.0 / (posterizeValue - 1)		
		
	for k in range(len(png[2])):	
		row = png[2][k]
		
		yield posterizeRow(row, channelsPerPixel, numOfAreas, numOfValues)
		

#main flow
r = png.Reader(filename)
image = r.read()

newRGB = posterize(image, posterizeValue)

hasAlpha = image[3]['alpha']

f = open(filename, 'wb')
w = png.Writer(image[0], image[1], alpha=hasAlpha)
w.write(f, newRGB)
f.close()
sys.exit()




 