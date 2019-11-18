#!/bin/bash

rm -r /var/alert-daemon/daemonvenv
virtualenv /var/alert-daemon/daemonvenv
source /var/alert-daemon/daemonvenv/bin/activate
pip3 install -r requirements.txt



