#!/bin/bash

rm -r daemonvenv
virtualenv daemonvenv
source venv/bin/activate
pip3 install -r requirements.txt



