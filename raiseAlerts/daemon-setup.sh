#!/bin/bash
systemctl stop alert-daemon
rm /var/alert-daemon/alertRaiser.py
rm /etc/systemd/system/alert-daemon.service
cp alertRaiser.py /var/alert-daemon
cp alert-daemon.service /etc/systemd/system
systemctl daemon-reload
systemctl enable alert-daemon
systemctl start alert-daemon
