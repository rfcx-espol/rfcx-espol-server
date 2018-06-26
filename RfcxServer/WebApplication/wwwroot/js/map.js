setInterval(displayLastData, 300000);

function displayLastData(){
    for(device of devices){
        for(sensor of device['sensorsId']){
          $.get('api/Device/'+device['id']+'/Sensor/'+sensor+'/Data/lastData', function(data){
            var lastData = JSON.parse(data)
            if(lastData!=null){
              var p = document.getElementById(lastData['SensorId']);
              if(p != null){
                var unit = lastData['Units'];
                if(unit=="CELCIUS"){
                  unit = "°C";
                }else if (unit=="H"){
                  unit = "%";
                }
                p.innerHTML = lastData['Value'] +" "+ unit;
                console.log("changed to", lastData['Value'] + unit);
              }
            }
          });
        }
    }
}
function getDataSensor(){
    for(device of devices){
      var deviceId = device['id']; 
      $.get('api/Device/'+parseInt(deviceId)+'/Sensor/', function(data){
        var dataDic = JSON.parse(data);
        for(sensor of dataDic){
          var id = sensor['Id'];
          var idDevice = sensor['DeviceId'];
          var typeSensor = sensor['Type'];
          var locationSensor = sensor['Location'];
          var contentS = devices[idDevice-1]["content"];
          if(typeSensor=="Humedad"){
            devices[idDevice-1]["content"] = contentS + '<p style="font-size: 14px; float: left;"><i class="fa fa-tint" style="font-size:20px; color: #527cfb" ></i> Amb.: </p><p class="valueHum" id="humedadId" style="font-size: 14px; line-height: 2;"> 30 H</p>';
            devices[idDevice-1]["content"] = devices[idDevice-1]["content"].replace("humedadId", id);
          }
          else if(typeSensor=="Temperature" && locationSensor=="Ambiente"){
            devices[idDevice-1]["content"] = contentS + '<p style="font-size: 14px; float: left;"><i class="fa fa-thermometer" style="font-size:20px; color: #424084;"></i> Amb.: </p><p class="valueTempAmb" id="tempAmbId" style="font-size: 14px; line-height: 2;"> 35 °C</p>';
            devices[idDevice-1]["content"] = devices[idDevice-1]["content"].replace("tempAmbId", id);
          }else{
            devices[idDevice-1]["content"] = contentS + '<p style="font-size: 14px; float: left;"><i class="fa fa-thermometer" style="font-size:20px; color: #ff7800;"></i> Disp.: </p><p class="valueTempDisp" id="tempDispId" style="font-size: 14px; line-height: 2;"> 40 °C</p>';
            devices[idDevice-1]["content"] = devices[idDevice-1]["content"].replace("tempDispId", id);
          }
          devices[idDevice-1]["content"] = devices[idDevice-1]["content"] +'</div>'+
                                  '</div>';
          
          devices[idDevice-1]["sensorsId"].push(id);
        }
        initMap();
      });
      
    }    
  }
  
  function initMap() {
        var bosque = {lat: -2.152062, lng: -79.963488};
        var estilos =[
            {
                featureType: "poi",
                elementType: "labels",
                stylers: [
                      { visibility: "off" }
                ]
            },
            {
                featureType: "transit",
                elementType: "labels",
                stylers: [
                      { visibility: "off" }
                ]
            }
        ];
        var map = new google.maps.Map(document.getElementById('map'), {
          zoom: 17,
          center: bosque,
          styles: estilos
        });
        map.setMapTypeId(google.maps.MapTypeId.HYBRID);
        
        for(device of devices){
          var coordenadas = {lat: parseFloat(device['lat']), lng: parseFloat(device['long'])};
          var contentString = device["content"];
          var infowindow = new google.maps.InfoWindow({
          content: contentString
          });
          var imageURL = 'http://maps.google.com/mapfiles/kml/paddle/orange-circle.png';
          var image = {
            url: imageURL,
            scaledSize: new google.maps.Size(30, 30)
          };
          var marker = new google.maps.Marker({
            position: coordenadas,
            map: map,
            icon: image
          });
          //infowindow.open(map,marker);
          var currentInfoWindow = null;
          google.maps.event.addListener(marker,'click', (function(marker,contentString,infowindow){ 
            return function() {
                if (currentInfoWindow != null) { 
                  currentInfoWindow.close(); 
                }
                infowindow.open(map, marker); 
                currentInfoWindow = infowindow;
                displayLastData();
            };
          })(marker,contentString,infowindow));
          
        }
        
          
}
