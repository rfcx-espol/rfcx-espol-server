setInterval(displayLastData, 300000);

function displayLastData(){
    for(station of stations){
        for(sensor of station['sensorsId']){
          $.get('api/Station/'+station['id']+'/Sensor/'+sensor+'/Data/lastData', function(data){
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
  var counter = 0;  
  for(station of stations){
      var stationId = station['id']; 
      $.get('api/Station/'+parseInt(stationId)+'/Sensor/', function(data){
        var dataDic = JSON.parse(data);
        for(sensor of dataDic){
          var id = sensor['Id'];
          var idStation = sensor['StationId'];
          var typeSensor = sensor['Type'];
          var locationSensor = sensor['Location'];
          var contentS = stations[counter]["content"];
          if(typeSensor.includes("Hum")){
            stations[counter]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-tint" style="font-size:20px; color: #527cfb" ></i> Amb.: </p><p class="valueHum" id="humedadId"> <i class="fa fa-circle-o-notch fa-spin" style="font-size:15px"></i></p>';
            stations[counter]["content"] = stations[counter]["content"].replace("humedadId", id);
          }
          else if(typeSensor.includes("Temp") && (locationSensor.includes("Amb") || locationSensor.includes("Env"))){
            stations[counter]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-thermometer" style="font-size:20px; color: #424084;"></i> Amb.: </p><p class="valueTempAmb" id="tempAmbId"> <i class="fa fa-circle-o-notch fa-spin" style="font-size:15px"></i></p>';
            stations[counter]["content"] = stations[counter]["content"].replace("tempAmbId", id);
          }else{
            stations[counter]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-thermometer" style="font-size:20px; color: #ff7800;"></i> Disp.: </p><p class="valueTempDisp" id="tempDispId"> <i class="fa fa-circle-o-notch fa-spin" style="font-size:15px"></i></p>';
            stations[counter]["content"] = stations[counter]["content"].replace("tempDispId", id);
          }
          stations[counter]["content"] = stations[counter]["content"] +'</div>'+
                                  '</div>';
          
          stations[counter]["sensorsId"].push(id);
        }
        counter++;
        initMap();
      });
    }  
      
  }

function initMap() {
	var bosque = {lat: -2.15437, lng: -79.963035};
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
	
	for(station of stations){
		var coordenadas = {lat: parseFloat(station['lat']), lng: parseFloat(station['long'])};
		var contentString = station["content"];
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