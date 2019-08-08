import time
import threading
import pymongo
import requests


client = pymongo.MongoClient()
db = client.db_rfcx
TIME_INTERVAL = 60  # in seconds


def getAllActiveAlerts():
    """return all active alerts in a cursor object."""

    alerts = db.Alert
    activeAlerts = alerts.find({"Status": True})
    return activeAlerts


def getLatestData(time):
    """
    return latest data, last (time) minutes.

    Keyword arguments:
    time -- time interval to gather latest data in minutes (default 1)
    """

    data = db.data
    ts = time.time()
    latestData = data.find(
        {"Timestamp": {"$gte": ts-TIME_INTERVAL, "$lte": ts}})
    return latestData


def checkConditions(alert, dataset):
    """
    Check all conditions in an alert. returns true if all conditions are met.

    Keyword arguments:
    alert -- dictionary of alert object
    data -- datalist to check with alert's conditions
    """

    conditions = alert["Conditions"]
    raiseAlert = False
    for condition in conditions:
        raiseCondition = False
        if condition["Comparison"] == "MORE THAN":
            for data in dataset:
                if float(data["value"]) > condition["threshold"]:
                    raiseCondition = True
            if not raiseCondition:
                return raiseAlert
        elif condition["Comparison"] == "LESS THAN":
            for data in dataset:
                if float(data["value"]) < condition["threshold"]:
                    raiseCondition = True
            if not raiseCondition:
                return raiseAlert
        elif condition["Comparison"] == "EQUALS":
            for data in dataset:
                if float(data["value"]) == condition["threshold"]:
                    raiseCondition = True
            if not raiseCondition:
                return raiseAlert
    raiseAlert = True
    return raiseAlert


def createIncident(alert):
    """
    Sends http post request to create an incident, returns request status code.

    Keyword arguments:
    alert -- dictionary of alert object
    """
    r = requests.post("http://localhost:5000/api/Incident", data={
                      'RaisedAlertName': alert['Name'], 'RaisedCondition': alert["Conditions"]})
    return r.status_code


def checkLatestData():
    """
    checks if any alert should be raised based on the dataset of a set interval
    if there is any it sends a post request to create an incident
    """
    activeAlerts = getAllActiveAlerts()
    dataset = getLatestData(TIME_INTERVAL)

    for alert in activeAlerts:
        if checkConditions(alert, dataset):
            createIncident(alert)

    print("incidente creado")


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
