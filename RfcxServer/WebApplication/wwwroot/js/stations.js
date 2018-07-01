$(window).on('load',function(){
    stations = []
    $.get('api/Station/', getDevicesList);
    
    $("#masfilas").click(function(){
        $("#myt").append('<tr><td><input type="text" name="parametros[]"/></td><td> <input type="text" name="unidad[]"/></td><td> <a href="#" class="delete"><i class="material-icons">delete_forever</i></a></td></tr>');
        $('.delete').off().click(function(e) {
            $(this).parent('td').parent('tr').remove();
        });      
    });
});

function getDevicesList(data) {
    var data_dic = JSON.parse(data);
    for(station of data_dic){
        var station_id = station['Id'];
        var station_name = station['Name'];
        var content = '<div class="estacion col-lg-3 col-md-3 col-sm-4 col-xs-6"><div class="titulo">'+
        '<h4><a href="/DeviceView?deviceName='+station_name+'&deviceId='+station_id+'">'+station_name+'</a></h4>'+
        '</div><div class="cuerpo">';

        stations_dic = {};
        stations_dic["id"] = station_id;
        stations_dic["content"] = content;
        stations_dic["sensorsId"] = [];
        stations.push(stations_dic);
    }
    getSensors();
}

function getSensors() {
    for(station of stations){
        var station_id = station['id']; 
        $.get('api/Station/' + parseInt(station_id) + '/Sensor/', function(data){
            var data_dic = JSON.parse(data); 
            for(sensor of data_dic){
                var sensor_id = sensor['Id'];
                var station_id = sensor['DeviceId'];
                var sensor_type = sensor['Type'];
                var sensor_location = sensor['Location'];
                stations[station_id-1]["content"] = stations[station_id-1]["content"] + '<p>tipo lugar<p>';
                stations[station_id-1]["content"] = stations[station_id-1]["content"].replace("tipo", sensor_type);
                stations[station_id-1]["content"] = stations[station_id-1]["content"].replace("lugar", sensor_location);
                stations[station_id-1]["sensorsId"].push(sensor_id);
            }
            stations[station_id-1]["content"] = stations[station_id-1]["content"] + '</div></div>';
            console.log(stations[station_id-1]["content"]);
            $(stations[station_id-1]["content"]).insertBefore(".plus-device");
            updateStations();
        });
    }
}

function updateStations(){
    var body_maximum_height = 0;
    var bodies = $(".cuerpo").get();
    var title_height = $(".titulo").get()[0];
    console.log(title_height);
    var d = $(title_height).height();
    for(b of bodies) {
        if($(b).height() > body_maximum_height) {
            body_maximum_height = $(b).height();
        }
    }
    for(b of bodies) {
        $(b).height(body_maximum_height);
    }
    console.log("titulo: " + d);
    console.log("cuerpo: " + body_maximum_height);
    $(".cuerpo-nuevo").height(body_maximum_height + d+13);
}