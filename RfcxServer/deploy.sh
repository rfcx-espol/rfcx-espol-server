type ffmpeg # ffmpeg is a dependency
. ./config.sh
#APP_DIR=/var/rfcx-espol-server/

mkdir $APP_DIR
find $APP_DIR  -type f ! -name 'files' -print0 | xargs -0 rm -vf

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
cp -r ./icecast-config $APP_DIR/
chmod -R 777 $APP_DIR/icecast-config
cp -r ./icecast-web $APP_DIR/
chmod -R 777 $APP_DIR/icecast-web
cp -r ./icecast-admin $APP_DIR/
chmod -R 777 $APP_DIR/icecast-admin
cp -r ./icecast-log $APP_DIR/
chmod -R 777 $APP_DIR/icecast-log
cp ./rfcx-espol-playlist.service /etc/systemd/system
sed -i 's/ICE_USER/'$ICE_USER'/g' /etc/systemd/system/rfcx-espol-playlist.service
cd ..
systemctl stop rfcx-espol-playlist.service
systemctl enable rfcx-espol-playlist.service
systemctl start rfcx-espol-playlist.service
# systemctl status rfcx-espol-playlist.service