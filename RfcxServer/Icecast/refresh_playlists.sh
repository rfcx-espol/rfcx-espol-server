#!/bin/sh

#File implementing Icecast and Ices services
#variables to use in script

var_path="/var/rfcx-espol-server/files/device"
extension="/ogg"
icecast_command="icecast -b -c /var/rfcx-espol-server/icecast-config/icecast.xml"

echo | $icecast_command &

while [ 1 -gt 0 ]; do # while loop, run forever

	
	for i in 0 1 2 3 # assuming 4 devices, should be extended to n devices eventually
	do
		
		find $var_path$i$extension -mmin +10 -type f -delete # delete all files older than 10 minutes
		find $var_path$i$extension -type f -size +20k > $var_path$i/playlist.txt # create playlist with remaining files in folder
	
		ices_command="ices /var/rfcx-espol-server/icecast-config/ices-playlist-$i.xml"
		$ices_command & # call IceS excecutable
	done
	echo ices raise
	sleep 5m # repeat every 5 minutes
done

