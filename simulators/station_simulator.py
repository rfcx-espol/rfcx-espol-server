import pymongo
import time

DB_NAME = "db_rfcx00"

client = pymongo.MongoClient("mongodb://localhost:27017/")
db = client[DB_NAME]

#Collections
station_collection = db["Station"]
sensor_collection = db["Sensor"]
data_collection  = db["Data"]

#myquery = { "address": "Park Lane 38" }
#mydoc = mycol.find(myquery)

"""
to insert documents to Data collection I need :
    {
        "StationId" : 16, //got from Station
        "SensorId" : 1,   //got from Sensor
        "Timestamp" : NumberLong(1566016729),//generated
        "Type" : "Temperature", //got from Sensor
        "Value" : 25.11, //generated
        "Units" : "Celcius", //generated
        "Location" : "Environment" //got from Sensor
    }
"""
#get id of station using station name
station_document = station_collection.find_one( { "Name": "Estacion Sansung phone" })
station_id = station_document['Id']
print(station_document['Id'])

#get details of sensors such as Id, Type and Location
sensor_documents = sensor_collection.find( { "StationId": station_id } )
t = int(time.time())
for d in sensor_documents :
    data = {
        "StationId" : station_id, 
        "SensorId" : d["Id"],
        "Timestamp" : t,
        "Type" : d["Type"], 
        "Value" : -25.11,
        "Units" : "Celcius", 
        "Location" : d["Location"] 
    }
    #Here I extract the data
    #so, here I can Up
    data_collection.insert_one(data)
    print(data)


"""
cursor = station_collection.find({})
for document in cursor:
  station_id = document['Id'] #get id of station
  print(station_id)
"""