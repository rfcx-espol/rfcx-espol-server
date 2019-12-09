import time
from datetime import datetime, timedelta
import threading
import pymongo
import requests
import json


client = pymongo.MongoClient()
db = client.BosqueProtector1


def getAllActiveAlerts():
    """return all active alerts in a cursor object."""

    alerts = db.Alert
    neededAlerts = []
    activeAlerts = alerts.find({"Status": True})
    for alert in activeAlerts:
        resta = int(time.time()) - alert["LastChecked"]
        frec = 60*alert["Frecuency"]
        if resta >= frec:
            neededAlerts.append(alert)

    return neededAlerts

def getLatestDataByStation(timeFrame, stationId, sensorId):
    """
    return latest data, last (time) minutes.

    Keyword arguments:
    timeFrame -- time interval to gather latest data in minutes (default 1)
    stationId -- id of the station where the data was aquired
    sensorId -- id of the sensor that captured the data
    """

    data = db.Data
    ts = int(time.time() - timeFrame*60)

    latestData = data.find(
        {
            "SavedAt": {"$gte": ts},
            "StationId": stationId,
            "SensorId": sensorId
        }
    )
    return latestData

def calculateAverageValue(latestData):
    values = []
    for data in latestData:
        values.append(data["Value"])
    average = sum(values)/len(values)
    return average

def checkConditionsAverage(alert):
    values = []
    updateLastChecked(alert)
    frecuency = alert["Frecuency"]
    conditions = alert["Conditions"]
    for condition in conditions:
        stationId = condition["StationId"]
        sensorId = condition["SensorId"]
        dataset = getLatestDataByStation(
            frecuency, int(stationId), int(sensorId))
        if dataset.count() <= 0:
            return False, values
        value = calculateAverageValue(dataset)

        if condition["Comparison"] == "MAYOR QUE":
            if float(value) > condition["Threshold"]:
                values.append(value)
            else:
                return False, values
        elif condition["Comparison"] == "MENOR QUE":
            if float(value) < condition["Threshold"]:
                values.append(value)
            else:
                return False, values
        
        elif condition["Comparison"] == "IGUAL":
            if float(value) == condition["Threshold"]:
                values.append(value)
            else:
                return False, values
    
    return True, values
        


def checkConditions(alert):
    """
    Check all conditions in an alert. returns true if all conditions are met.

    Keyword arguments:
    alert -- dictionary of alert object
    data -- datalist to check with alert's conditions
    """
    updateLastChecked(alert)
    mFrecuency = alert["Frecuency"]
    raiseAlert = False
    conditions = alert["Conditions"]
    values = []
    for condition in conditions:
        raiseCondition = False
        stationId = condition["StationId"]
        sensorId = condition["SensorId"]
        dataset = getLatestDataByStation(
            mFrecuency, int(stationId), int(sensorId))
        if dataset.count() <= 0:
            return False, values
        if condition["Comparison"] == "MAYOR QUE":
            for data in dataset:
                if float(data["Value"]) > condition["Threshold"]:
                    raiseCondition = True
                    values.append(data)
            if not raiseCondition:
                return False, values
        elif condition["Comparison"] == "MENOR QUE":
            for data in dataset:
                if float(data["Value"]) < condition["Threshold"]:
                    raiseCondition = True
                    values.append(data)
            if not raiseCondition:
                return False, values
        
        elif condition["Comparison"] == "IGUAL":
            for data in dataset:
                if float(data["Value"]) == condition["Threshold"]:
                    raiseCondition = True
                    values.append(data)
            if not raiseCondition:
                return False, values
    
    return raiseCondition, values


def createIncident(alert, data):
    """
    Sends http post request to create an incident, returns request status code.

    Keyword arguments:
    alert -- dictionary of alert object
    """
    headers = {'Content-type': 'application/json'}
    data = {'RaisedAlertName': alert['Name'],
            'RaisedCondition': stringifyCondition(alert, data)}
    #request to local database
    r = requests.post(url="http://localhost:5000/api/Incident",
                      data=json.dumps(data), headers=headers)
    # r = requests.post(url="http://200.126.14.250/api/Incident",
    #                   data=json.dumps(data), headers=headers)
    return r.status_code

def updateLastChecked(alert):
    headers = {'Content-type': 'application/json'}
    data = int(time.time())
    r = requests.patch(url="http://localhost:5000/api/Alert/"+ str(alert.get("_id"))+"/LastChecked",
                      data=json.dumps(data), headers=headers)
    # r = requests.patch(url="http://200.126.14.250/api/Alert/"+ str(alert.get("_id"))+"/LastChecked",
    #                   data=json.dumps(data), headers=headers)
    return r.status_code

def stringifyCondition(alert, datas):
    message = ""
    conditions = alert["Conditions"]
    for i in range(len(conditions)):
        stationId = str(conditions[i]["StationId"])
        sensorId = str(conditions[i]["SensorId"])
        threshold = str(conditions[i]["Threshold"])
        comparision = conditions[i]["Comparison"]
        value = str(datas[i])
        timestamp = str(int(time.time()))
        # unit = str(datas[i]["Units"])

        message += "["+timestamp+"]: "+ value + " " \
            + comparision + " "+ threshold \
                +" en estación con id: " + stationId \
                    + " usando el sensor con id: " + sensorId + "\n"
    return message  

def checkLatestData():
    """
    checks if any alert should be raised based on the dataset of a set interval
    if there is any it sends a post request to create an incident
    """
    activeAlerts = getAllActiveAlerts()

    for alert in activeAlerts:
        check, data = checkConditionsAverage(alert)
        if check:
            print(createIncident(alert, data))

while True:
    print("running...")
    checkLatestData()
    time.sleep(60)