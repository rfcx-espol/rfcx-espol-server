#!/bin/sh


icecast_command="icecast -b -c /var/rfcx-espol-server/icecast-config/icecast.xml"
echo | $icecast_command

while [ 1 -gt 0 ]; do

	

	pid=`ps ax | awk '/[i]ces2/{print $1}'`

	for i in 0 1 2 3
	do
		
		find ./device$i/ogg -mmin +10 -type f -delete
		find ./device$i/ogg -type f > /device$i/playlist.txt
	

		var=`echo $pid | awk -v i="$i" '{print $i}'`
		echo $var
		ices_command="ices2 /var/rfcx-espol-server/icecast-config/ices-playlist-$i.xml"
		$ices_command &
	done
	echo ices raise
	sleep 5m
done

