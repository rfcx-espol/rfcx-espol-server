from os import makedirs
from shutil import copyfile

from pymongo import MongoClient

from trymodel import tensoroutput, structure_lists_output

MINIMUM_PROBABILITY = 0.5

def raw_as_lists(image_path_source):
	raw = tensoroutput(image_path_source)
	species, probs = structure_lists_output(raw)
	return species, probs

def get_top_pick(species, probs):
	if probs[0] < MINIMUM_PROBABILITY:
		return None
	return species[0]
    

client = MongoClient('localhost', 27017)

db = client.BosqueProtector

collection = db.Camera_Image

cursor = collection.find({"State" : "PENDIENTE"})

path = "/var/rfcx-espol-server/resources/images/"
# temp = "/home/johnny/Pictures/temp/"

# makedirs(temp, exist_ok=True)

for image in cursor:
	image_path_source = path+str(image['StationId']) + "/" + image['Path']
	# image_path_temp = temp+image['Path']
	# copyfile(image_path_source, image_path_temp)
	species, probs = raw_as_lists(image_path_source)
	top_pick = get_top_pick(species, probs)
	if top_pick is not None:
		if image.get('Family') is None:
			image['Family'] = []
		if image.get('Tags') is None:
			image['Tags'] = []
		image['Family'].append(top_pick)
		image['Tags'] += species[1:]
		collection.replace_one({'_id': image['_id']}, image)
    
