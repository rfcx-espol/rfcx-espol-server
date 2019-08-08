import daemon
import alertRaiser

# print(daemon.__file__)
with daemon.DaemonContext():
    inter = alertRaiser.setInterval(60, alertRaiser.checkLatestData())
