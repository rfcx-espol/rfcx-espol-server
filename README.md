# rfcx-espol-server

Espol server stores the audios:
For each station created, a folder with the name “station” is created, where N is an incremental auto id.
Those folders contain two folders more: 
audios and ogg
Only “audios” folder has the audios that send the station in format m4a.
You can see into server: rfcx-espol-server/files


Views and Controllers:

-	StationView (Home) used StationController:
Create, delete, edit one station.
-	Audios used DownloadController:
Find and download audios in file .zip

-	Map used MapController:
Show Bosque La Prosperina in google map with all station.
-	BosqueVirtual used BosqueVirtualController:
Show the game about virtual tour, animals, plants of the Bosque La Prosperina.
