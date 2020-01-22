import pymongo
import time
import random
import sys
import pprint

if len(sys.argv) > 2:
    print('You have specified too many arguments')
    sys.exit()

if len(sys.argv) < 2:
    print('You need to specify the name of the station')
    sys.exit()

input_station_name = sys.argv[1]

DB_NAME = "BosqueProtector1"

client = pymongo.MongoClient("mongodb://localhost:27017/")
db = client[DB_NAME]

#Collections
station_collection = db["Station"]
sensor_collection = db["Sensor"]
data_collection  = db["Data"]

"""
to insert documents to Data collection I need :
    {
        "StationId" : 16, //got from Station
        "SensorId" : 1,   //got from Sensor
        "Timestamp" : NumberLong(1566016729), //generated
        "Type" : "Temperature", //got from Sensor
        "Value" : 25.11, //generated
        "Units" : "Celcius", //generated
        "Location" : "Environment" //got from Sensor
    }
"""
#get id of station using station name
station_document = station_collection.find_one( { "Name": input_station_name })

if station_document == None:
    print("There is no such station.")
    print("These are the stations available:")
    for s in station_collection.find() :
        print( '\t"{}"'.format( s['Name'] ) )
    
    print('\nNote: Be sure to use quotation marks. ')
    sys.exit()

station_id = station_document['Id']

#get details of sensors such as Id, Type and Location
sensor_documents = sensor_collection.find( { "StationId": station_id } )

class Data(object):

    def __init__(self , *args, **kwargs):
        self.StationId = kwargs.get('StationId')
        self.SensorId  = kwargs.get('SensorId')
        self.Type      = kwargs.get('Type')
        self.Location  = kwargs.get('Location')        
        self.Units     = self.determineUnits(self.Type)

    def determineUnits(self, typeOfSensor):
        if typeOfSensor == 'Temperature' : 
            units = 'Celsius'
        elif typeOfSensor == 'Humidity':
            units = 'Percentaje'
        else : 
            raise Exception('Unknown type of sensor. Please update station_simulator.py, the unknown type of sensor is: {}'.format(typeOfSensor))
        return units

    # returns a dictionary out of the object structure
    def toSimulatedData(self):
        self.Timestamp  = int(time.time()) #set current timestamp in seconds
        self.SavedAt = int(time.time())
        self.Value = self.simulateValue(self.Type)
        return vars(self) 
    
    def simulateValue(self, typeOfSensor):    
        if typeOfSensor == 'Temperature' :
            value = random.uniform(25,50)
        elif typeOfSensor == 'Humidity':
            value = random.uniform(40,60)
        else : 
            raise Exception('Unknown type of sensor. Please update station_simulator.py, the unknown type of sensor is: {}'.format(typeOfSensor))
        return round(value,2)

#I prepare the abstractions of data to be inserted
data_abstractions =[]
for s in sensor_documents:
        #Here I extract the data
        data = Data(
            StationId = station_id, 
            SensorId  = s["Id"],
            Type      = s["Type"],
            Location  = s["Location"] 
        )
        data_abstractions.append(data)

#I have the abstractions ready, Is just a matter of call the toSimulatedData() method
while True:    
    print("\n --- Simulating station {}... ---".format(input_station_name))
    for d in data_abstractions:        
        simulated_data = d.toSimulatedData()

        #so, here I can insert
        data_collection.insert_one(simulated_data)
        
        #to avoid error on inserting iteratively -> https://stackoverflow.com/questions/5906493/pymongo-insert-inside-the-loop
        simulated_data.pop('_id', None)

        #or print it
        pprint.pprint(simulated_data)
        print()
    time.sleep(300)
