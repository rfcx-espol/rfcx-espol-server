$(window).on("load", function(){
    stations = []
    $.ajax({
        url : 'api/Station/',
        type: 'GET',
        async: false,
        success : getStationsList
    })
    updateStationsHeight();
    $('#station_modal').on('hidden.bs.modal', function (e) {
        $("form input").val("");
        $("h4#modal_label").html("Nueva Estación");
        $("input#api_key").removeAttr("disabled");
    });    
    $("#masfilas").click(function(){
        $("#myt").append('<tr><td><input type="text" name="parametros[]"/></td><td> <input type="text" name="unidad[]"/></td><td> <a href="#" class="delete"><i class="material-icons">delete_forever</i></a></td></tr>');
        $('.delete').off().click(function(e) {
            $(this).parent('td').parent('tr').remove();
        });      
    });
});

function fillStationModal(id){
    $.ajax({
        url : 'api/Station/'+id,
        type: 'GET',
        async: false,
        success : function(data){
            var data_dic = JSON.parse(data);
            $("input#name").val(data_dic["Name"]);
            $("input#api_key").val(data_dic["APIKey"]);
            $("input#latitude").val(data_dic["Latitude"]);
            $("input#longitude").val(data_dic["Longitude"]);
            $("input#android_version").val(data_dic["AndroidVersion"]);
            $("input#services_version").val(data_dic["ServicesVersion"]);
            $("input#db_id").val(data_dic["Id"]);
            $("h4#modal_label").html("Editar Estación");
            $("input#api_key").attr("disabled","disabled");
            $("#station_modal").modal("show");
        }
    })
}

function saveStation() {
    var id = $("input#db_id").val();
    if(id == "") {
        saveNewStation();
        window.location.reload();
    } else {
        //updateStation(id);
        console.log("actualizar estación");
    }
}

function saveNewStation() {
    var name = $("input#name").val();
    var lat = $("input#latitude").val();
    var long = $("input#longitude").val();
    var and_ver = $("input#android_version").val();
    var ser_ver = $("input#services_version").val();
    var api_k = $("input#api_key").val();
    $.ajax({
        type: 'POST',
        url: 'api/Station', 
        dataType: 'json',
        data: JSON.stringify({ Name: name, Latitude: lat, Longitude: long,
            AndroidVersion: and_ver, ServicesVersion: ser_ver, APIKey: api_k}),
        contentType: 'application/json'
    });
}

/*function updateStation(int id) {
    
}*/

function getStationsList(data) {
    var data_dic = JSON.parse(data);    
    for(station of data_dic){
        var station_id = station['Id'];
        var station_name = station['Name'];
        var content = '<div class="station col-lg-3 col-md-3 col-sm-4 col-xs-12"><div class="title">'+
        '<h4><a href="/StationView?stationName='+station_name+'&stationId='+station_id+'">'+station_name+'</a></h4>'+
        '<a onclick="fillStationModal('+station_id+');" class="material-icons edit">edit</a>'+
        '<a class="material-icons delete_station">delete</a>'+
        '</div><div class="station_body">';
        /*var content = '<div class="station col-lg-3 col-md-3 col-sm-4 col-xs-12"><div class="title">'+
        '<div id="link"><h4><a href="/StationView?stationName='+station_name+'&stationId='+station_id+'">'+station_name+'</a></h4></div>'+
        '<div id="delete"><a class="material-icons">delete</a></div><div id="edit"><a class="material-icons">edit</a></div><div style="clear: left;"/>'+
        '</div><div class="station_body">';*/

        stations_dic = {};
        stations_dic["id"] = station_id;
        stations_dic["content"] = content;
        stations_dic["sensorsId"] = [];
        stations.push(stations_dic);
    }
    getSensorsList();
}

function getSensorsList() {
    for(station of stations){
        var station_id = station['id']; 
        $.ajax({
            url : 'api/Station/' + parseInt(station_id) + '/Sensor/',
            type: 'GET',
            async: false,
            success : function(data){
                var data_dic = JSON.parse(data); 
                for(sensor of data_dic){
                    var sensor_id = sensor['Id'];
                    var sensor_type = sensor['Type'];
                    var sensor_location = sensor['Location'];
                    stations[station_id-1]["content"] = stations[station_id-1]["content"] + '<p>tipo lugar<p>';
                    stations[station_id-1]["content"] = stations[station_id-1]["content"].replace("tipo", sensor_type);
                    stations[station_id-1]["content"] = stations[station_id-1]["content"].replace("lugar", sensor_location);
                    stations[station_id-1]["sensorsId"].push(sensor_id);
                }
                stations[station_id-1]["content"] = stations[station_id-1]["content"] + '</div></div>';
                $(stations[station_id-1]["content"]).insertBefore(".plus-station");
            }
        })
    }
}

function updateStationsHeight(){
    var body_maximum_height = 0;
    var bodies = $(".station_body").get();
    var title_height = $(".title").get()[0];
    var d = $(title_height).height();
    for(b of bodies) {
        if($(b).height() > body_maximum_height) {
            body_maximum_height = $(b).height();
        }
    }
    for(b of bodies) {
        $(b).height(body_maximum_height);
    }
    $(".new_station_button").height(body_maximum_height + d+13);
}

function closeModal(){
    $("#station_modal").modal("hide");
}