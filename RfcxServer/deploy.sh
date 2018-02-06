type ffmpeg # ffmpeg is a dependency
. ./config.sh
#APP_DIR=/var/rfcx-espol-server/

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
# systemctl status rfcx-espol-server.service

# Icecast
cd ./Icecast
cp ./refresh_playlists.sh $APP_DIR
cp -r ./icecast-config $APP_DIR/icecast-config
chmod -R 777 $APP_DIR/icecast-config
cp -r ./icecast-web $APP_DIR/icecast-web
chmod -R 777 $APP_DIR/icecast-web
cp -r ./icecast-admin $APP_DIR/icecast-admin
chmod -R 777 $APP_DIR/icecast-admin
cp -r ./icecast-log $APP_DIR/icecast-log
chmod -R 777 $APP_DIR/icecast-log
cp ./rfcx-espol-playlist.service /etc/systemd/system
sed -i 's/ICE_USER/'$ICE_USER'/g' /etc/systemd/system/rfcx-espol-playlist.service
cd ..
systemctl stop rfcx-espol-playlist.service
systemctl enable rfcx-espol-playlist.service
systemctl start rfcx-espol-playlist.service
# systemctl status rfcx-espol-playlist.service