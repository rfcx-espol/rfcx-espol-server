#!/bin/bash
systemctl stop alert-daemon
rm /var/alert-daemon/alertRaiser.py
rm /etc/systemd/system/alert-daemon.service
rm install_requirements.sh
rm requirements.txt
cp requirements.txt /var/alert-daemon
cp install_requirements.sh /var/alert-daemon
cp alertRaiser.py /var/alert-daemon
cp alert-daemon.service /etc/systemd/system
systemctl daemon-reload
systemctl enable alert-daemon
systemctl start alert-daemon