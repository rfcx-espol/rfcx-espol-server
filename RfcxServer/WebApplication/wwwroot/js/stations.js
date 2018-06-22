$(window).on('load',function(){
    devices = []
    $.get('api/Device/', getDevicesList);
    
    $("#masfilas").click(function(){
        $("#myt").append('<tr><td><input type="text" name="parametros[]"/></td><td> <input type="text" name="unidad[]"/></td><td> <input type="text" name="especificacion[]"/></td><td> <a href="#" class="delete"><i class="material-icons">delete_forever</i></a></td></tr>');
        $('.delete').off().click(function(e) {
            $(this).parent('td').parent('tr').remove();
        });      
    });
});

function getDevicesList(data) {
    var data_dic = JSON.parse(data);
    for(device of data_dic){
        var device_id = device['Id'];
        var device_name = device['Name'];
        var content = '<div class="estacion col-lg-3 col-md-3 col-sm-4 col-xs-6"><div class="titulo">'+
        '<h4><a href="/DeviceView?deviceName='+device_name+'&deviceId='+device_id+'">'+device_name+'</a></h4>'+
        '</div><div class="cuerpo">';

        devices_dic = {};
        devices_dic["id"] = device_id;
        devices_dic["content"] = content;
        devices_dic["sensorsId"] = [];
        devices.push(devices_dic);
    }
    getSensors();
}

function getSensors() {
    for(device of devices){
        var device_id = device['id']; 
        $.get('api/Device/' + parseInt(device_id) + '/Sensor/', function(data){
            var data_dic = JSON.parse(data); 
            for(sensor of data_dic){
                var sensor_id = sensor['Id'];
                var device_id = sensor['DeviceId'];
                var sensor_type = sensor['Type'];
                var sensor_location = sensor['Location'];
                devices[device_id-1]["content"] = devices[device_id-1]["content"] + '<p>tipo lugar<p>';
                devices[device_id-1]["content"] = devices[device_id-1]["content"].replace("tipo", sensor_type);
                devices[device_id-1]["content"] = devices[device_id-1]["content"].replace("lugar", sensor_location);
                devices[device_id-1]["sensorsId"].push(sensor_id);
            }
            devices[device_id-1]["content"] = devices[device_id-1]["content"] + '</div></div>';
            console.log(devices[device_id-1]["content"]);
            $(devices[device_id-1]["content"]).insertBefore(".plus-device");
            updateStations();
        });
    }
}

function updateStations(){
    var body_maximum_height = 0;
    var bodies = $(".cuerpo").get();
    var title_height = $(".titulo").get()[0];
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