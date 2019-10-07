#stop service if available
sudo systemctl stop station_simulator.service

#remove previous script and service
sudo rm /usr/local/bin/station_simulator.py
sudo rm /lib/systemd/system/station_simulator.service

#install requirements
pip install -r requirements.txt

#add new script and service configuration
sudo cp station_simulator.py /usr/local/bin
sudo cp station_simulator.service /lib/systemd/system

#reload global daemon manager
sudo systemctl daemon-reload

#enable and start script
sudo systemctl enable station_simulator.service
sudo systemctl start station_simulator.service
