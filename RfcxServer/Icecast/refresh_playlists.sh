#!/bin/sh

#File implementing Icecast and Ices services
#variables to use in script

var_path="/var/rfcx-espol-server/resources/bpv/audios/"
extension="/ogg"
icecast_command="icecast -b -c /var/rfcx-espol-server/icecast-config/icecast.xml"

echo | $icecast_command &

# while loop, run forever
while [ 1 -gt 0 ]; do

	#Asuming 4 devices, should be extended to n devices eventually
	for station in $(ls $var_path) 
	do
		#Dlete all files older than 60 minutes
		#find $var_path$i$extension -mmin +60 -type f -delete
		#Create playlist with remaining files in folder
		find $var_path$station$extension -type f -size +20k > $var_path$station/playlist.txt
		number=$(echo $station | tr -dc '0-9') 
		ices_command="ices /var/rfcx-espol-server/icecast-config/ices-playlist-$number.xml"
		#Call IceS excecutable
		$ices_command &
	done
	echo ices raise
	#Repeat every 5 minutes
	sleep 5m
done
