#!/bin/bash
systemctl stop alert-daemon
rm /var/rfcx-espol-server/alertRaiser.py
rm /etc/systemd/system/alert-daemon.service
cp alertRaiser.py /var/rfcx-espol-server
cp alert-daemon.service /etc/systemd/system
systemctl daemon-reload
systemctl enable alert-daemon
systemctl start alert-daemon