import pymongo
import os
import subprocess
import shlex
import re
import os.path

from pymongo import MongoClient
# client = MongoClient()
client = MongoClient('localhost', 27017)
#client = MongoClient('mongodb://localhost:27017')
db = client.BosqueProtector1
#db = client['pymongo_test']
images = db.Camera_Image

#param1 = "--image_file"
#param2 = "../rfcx-espol-server/resources/images/14/5c49e29f35efed3c8e14ec94.jpeg"
#param3 = "../rfcx-espol-server/resources/images/14/5c73684032d4de558abf61c8ER5S45SD.jpg"
list_images = images.find({'State' : 'PENDIENTE'} )

#path = "python classify_image.py --image_file ../rfcx-espol-server/resources/images/14/5c49e29f35efed3c8e14ec94.jpeg" 

pathimg = "/var/rfcx-espol-imageanalysis/"
pathresources = "/var/rcfx-espol-server/"

for post in list_images:
        station = post['StationId']
        path = post['Path']
	print(station)
        print(path)
        print("Paso por aqui antes de llegar al output")
        #if os.path.exists('../rfcx-espol-server/resources/images/'+str(station)+'/'+str(path)):
	output= os.popen('python '+pathimg+'classify_image.py --image_file'+pathresources+'rfcx-espol-server/resources/images/'+str(station)+'/'+str(path)).read()
        #print(output)        
        print(len(output))
        #Si el analisis no esta vacio
        if len(output) > 0:
            #print("si tiene datos")
            arreglo =  output.split("\n")
            #conjunto de arreglos
            primero = arreglo[0]
            segundo = arreglo[1]
            tercero = arreglo[2]
            #tamano de los arreglos
            tempuno = len(primero)
            tempdos = len(segundo)
            temptres = len(tercero)
            #Diviendo cada prediccion en tag y score
            #1prediccion
            primero_final = primero[:tempuno-1]
            arreglo_palabra_uno = primero_final.split("(")
            tag = arreglo_palabra_uno[0]
            print(tag)
            score_arreglo = arreglo_palabra_uno[1].split("=")
            score = score_arreglo[1]
            print(score)
            #2prediccion
            segundo_final = segundo[:tempdos-1]
            arreglo_palabra_dos = segundo_final.split("(")
            tag2 = arreglo_palabra_dos[0]
            print(tag2)
            score_arreglo_dos = arreglo_palabra_dos[1].split("=")
            score2 = score_arreglo_dos[1]
            print(score2)
            #3prediccion
            tercero_final = tercero[:temptres-1]
            arreglo_palabra_tres = tercero_final.split("(")
            tag3 = arreglo_palabra_tres[0]
            print(tag3)
            score_arreglo_tres = arreglo_palabra_tres[1].split("=")
            score3 = score_arreglo_tres[1]
            print(score3)

            #AQui se hace las actualizaciones en el TAg
            myquery = { "Path": path }
            newvalues = { "$addToSet": { "Tag": tag } }
            images.update_one(myquery, newvalues)
            
            #Aqui se hace las actualizaciones en el Predictions
            myquery2 = { "Path": path }
            newvalues2 = { "$set": { "Predictions": {tag:score , tag2:score2 , tag3:score3} } }
            images.update_one(myquery2, newvalues2)

            #Aqui se actualiza  el estado de PENDIENTE A  ANALIZADO
            myquery3 = {"Path": path}
            newvalues3 = { "$set": { "State": "Analizado" } }
            images.update_one(myquery3, newvalues3)
#esta bien

