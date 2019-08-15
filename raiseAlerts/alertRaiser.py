import time
from datetime import datetime, timedelta
import threading
import pymongo
import requests
import json


client = pymongo.MongoClient()
db = client.db_rfcx
TIME_INTERVAL = 5  # in seconds


def getAllActiveAlerts():
    """return all active alerts in a cursor object."""

    alerts = db.Alert
    activeAlerts = alerts.find({"Status": True})
    return activeAlerts


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


def checkConditions(alert):
    """
    Check all conditions in an alert. returns true if all conditions are met.

    Keyword arguments:
    alert -- dictionary of alert object
    data -- datalist to check with alert's conditions
    """

    # mFrecuency = alert["Frecuency"]
    raiseAlert = False
    mFrecuency = 5
    conditions = alert["Conditions"]
    for condition in conditions:
        raiseCondition = False
        stationId = condition["StationId"]
        sensorId = condition["SensorId"]
        dataset = getLatestDataByStation(
            mFrecuency, int(stationId), int(sensorId))
        if dataset.count() <= 0:
            return False
        if condition["Comparison"] == "MAYOR QUE":
            for data in dataset:
                if float(data["Value"]) > condition["Threshold"]:
                    raiseCondition = True
            if not raiseCondition:
                return False
        elif condition["Comparison"] == "MENOR QUE":
            for data in dataset:
                if float(data["Value"]) < condition["Threshold"]:
                    print(float(data["Value"]))
                    raiseCondition = True
            if not raiseCondition:
                return False
        elif condition["Comparison"] == "IGUAL":
            for data in dataset:
                if float(data["Value"]) == condition["Threshold"]:
                    print(float(data["Value"]))
                    raiseCondition = True
            if not raiseCondition:
                return False
    return True

# # average
# def checkConditions(alert):
#     """
#     Check all conditions in an alert. returns true if all conditions are met.

#     Keyword arguments:
#     alert -- dictionary of alert object
#     data -- datalist to check with alert's conditions
#     """

#     # mFrecuency = alert["Frecuency"]
#     mFrecuency = 5
#     conditions = alert["Conditions"]
#     for condition in conditions:
#         raiseCondition = False
#         stationId = condition["StationId"]
#         sensorId = condition["SensorId"]

#         dataset = getLatestDataByStation(mFrecuency, stationId, sensorId)
#         avg = sum(float(data["value"]) for data in dataset)
#         print(avg)
#         if condition["Comparison"] == "MORE THAN":
#             if avg > condition["threshold"]:
#                 raiseCondition = True
#             else:
#                 return False
#         elif condition["Comparison"] == "LESS THAN":
#             if avg < condition["threshold"]:
#                 raiseCondition = True
#             else:
#                 return False
#         elif condition["Comparison"] == "EQUALS":
#             if avg == condition["threshold"]:
#                 raiseCondition = True
#             else:
#                 return False
#     return True


def createIncident(alert):
    """
    Sends http post request to create an incident, returns request status code.

    Keyword arguments:
    alert -- dictionary of alert object
    """
    headers = {'Content-type': 'application/json'}
    data = {'RaisedAlertName': alert['Name'],
            'RaisedCondition': str(alert["Conditions"])}
    r = requests.post(url="http://localhost:5000/api/Incident",
                      data=json.dumps(data), headers=headers)
    return r.status_code


def checkLatestData():
    """
    checks if any alert should be raised based on the dataset of a set interval
    if there is any it sends a post request to create an incident
    """
    activeAlerts = getAllActiveAlerts()

    for alert in activeAlerts:
        if checkConditions(alert):
            createIncident(alert)


class setInterval:
    def __init__(self, interval, action):
        self.interval = interval
        self.action = action
        self.stopEvent = threading.Event()
        thread = threading.Thread(target=self.__setInterval)
        thread.start()

    def __setInterval(self):
        nextTime = time.time()+self.interval
        while not self.stopEvent.wait(nextTime-time.time()):
            nextTime += self.interval
            self.action()


checkLatestData()
