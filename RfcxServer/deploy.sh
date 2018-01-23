type ffmpeg # ffmpeg is a dependency

APP_DIR=/var/rfcx-espol-server/
rm -r $APP_DIR
mkdir $APP_DIR

# Web Application
cd ./WebApplication
dotnet restore
dotnet publish -c Release -o $APP_DIR
cp ./rfcx-espol-server.service /etc/systemd/system
cd ..
systemctl stop rfcx-espol-server.service
systemctl enable rfcx-espol-server.service
systemctl start rfcx-espol-server.service
systemctl status rfcx-espol-server.service

# Icecast
cd ./Icecast
cp ./refresh_playlists.sh $APP_DIR
cp ./rfcx-espol-playlist.service /etc/systemd/system
cd ..
# systemctl stop rfcx-espol-playlist.service
# systemctl enable rfcx-espol-playlist.service
# systemctl start rfcx-espol-playlist.service
# systemctl status rfcx-espol-playlist.service