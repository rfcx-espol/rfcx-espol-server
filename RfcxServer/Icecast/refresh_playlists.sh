#!/bin/sh

var_path="/var/rfcx-espol-server/files/device"
extension="/ogg"
icecast_command="icecast -b -c /var/rfcx-espol-server/icecast-config/icecast.xml"
echo | $icecast_command

while [ 1 -gt 0 ]; do

	

	pid=`ps ax | awk '/[i]ces2/{print $1}'`

	for i in 0 1 2 3
	do
		
		find $var_path$i$extension -mmin +10 -type f -delete
		find $var_path$i$extension -type f > $var_path$i/playlist.txt
	

		var=`echo $pid | awk -v i="$i" '{print $i}'`
		echo $var
		ices_command="ices /var/rfcx-espol-server/icecast-config/ices-playlist-$i.xml"
		$ices_command &
	done
	echo ices raise
	sleep 5m
done

