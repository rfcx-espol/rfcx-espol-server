setInterval(displayLastData, 300000);

function displayLastData(){
    for(stationId in stations){
        for(sensor of stations[stationId]['sensorsId']){
          $.get('api/Station/'+stationId+'/Sensor/'+sensor+'/Data/lastData', function(data){
            var lastData = JSON.parse(data)
            if(lastData!=null){
              var p = document.getElementById(lastData['SensorId']);
              if(p != null){
                var unit = lastData['Units'];
                if(unit=="CELCIUS" || unit=="Celcius"){
                  unit = "°C";
                }else if (unit=="Percent"){
                  unit = "%";
                }
                p.innerHTML = lastData['Value'] +" "+ unit;
              }
            }
          });
        }
    }
}

function getDataSensor(){
  for(var stationId in stations){
      $.get('api/Station/'+stationId+'/Sensor/', function(data){
				var dataDic = JSON.parse(data);
					for(sensor of dataDic){
						var idStation = sensor['StationId'];
						var id = sensor['Id'];
						var typeSensor = sensor['Type'];
						var locationSensor = sensor['Location'];
						var contentS = stations[idStation]["content"];
						
						if(typeSensor.includes("Hum")){
							stations[idStation]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-tint" style="color: #527cfb" ></i> Amb.: </p><p class="valueHum" id="humedadId"> - </p>';
							stations[idStation]["content"] = stations[idStation]["content"].replace("humedadId", id);
						}
						else if(typeSensor.includes("Temp") && (locationSensor.includes("Amb") || locationSensor.includes("Env"))){
							stations[idStation]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-thermometer" style="color: #424084;"></i> Amb.: </p><p class="valueTempAmb" id="tempAmbId"> - </p>';
							stations[idStation]["content"] = stations[idStation]["content"].replace("tempAmbId", id);
						}else{
							stations[idStation]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-thermometer" style="color: #ff7800;"></i> Disp.: </p><p class="valueTempDisp" id="tempDispId"> - </p>';
							stations[idStation]["content"] = stations[idStation]["content"].replace("tempDispId", id);
						}
						stations[idStation]["content"] = stations[idStation]["content"] +'</div>'+
																		'</div>';
						
						stations[idStation]["sensorsId"].push(id);
					}
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
	
	for(var stationId in stations){
		var coordenadas = {lat: parseFloat(stations[stationId]["lat"]), lng: parseFloat(stations[stationId]["long"])};
		var contentString = stations[stationId]["content"];
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