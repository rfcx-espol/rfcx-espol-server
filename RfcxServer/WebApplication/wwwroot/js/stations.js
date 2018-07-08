$(window).on("load", function(){
    stations = []
    stations_input_changed = []
    $.ajax({
        url : 'api/Station/',
        type: 'GET',
        async: false,
        success : getStationsList
    })

    updateStationsHeight();

    $("#station_modal input.form-control").change(function () {
        var input_id = $(this).attr("id");
        if(!stations_input_changed.includes(input_id)) {
            switch(input_id) {
                case "latitude":
                    if(stations_input_changed.includes("longitude")) {
                        return;
                    }
                    break;
                case "longitude":
                    if(stations_input_changed.includes("latitude")) {
                        return;
                    }
                    break;
            }
            stations_input_changed.push(input_id);
        }
    });

    $('#station_modal').on('hidden.bs.modal', function (e) {
        $("form input").val("");
        $("h4#modal_label").html("Nueva Estación");
        $("input#api_key").removeAttr("disabled");
        stations_input_changed.length = 0;
    });

    $('#alert_modal').on('hidden.bs.modal', function (e) {
        $("input#st_id").val("");
    });
});

function getStationsList(data) {
    var data_dic = JSON.parse(data);
    for(station of data_dic){
        var station_id = station['Id'];
        var station_name = station['Name'];
        var content = '<div class="station col-lg-3 col-md-3 col-sm-4 col-xs-12"><div class="title">'+
        '<h4><a href="/StationView?stationName='+station_name+'&stationId='+station_id+'">'+station_name+'</a></h4>'+
        '<a class="material-icons edit" onclick="fillStationModal('+station_id+');">edit</a>'+
        '<a class="material-icons delete_station" onclick="showAlertModal('+station_id+');">delete</a>'+
        '</div><div class="station_body">';
        stations_dic = {};
        stations_dic["id"] = station_id;
        stations_dic["content"] = content;
        stations_dic["sensorsId"] = [];
        stations.push(stations_dic);
    }
    getSensorsList();
}

function getSensorsList() {
    var counter = 0;
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
                    stations[counter]["content"] = stations[counter]["content"] + '<p>tipo lugar<p>';
                    stations[counter]["content"] = stations[counter]["content"].replace("tipo", sensor_type);
                    stations[counter]["content"] = stations[counter]["content"].replace("lugar", sensor_location);
                    stations[counter]["sensorsId"].push(sensor_id);
                }
                stations[counter]["content"] = stations[counter]["content"] + '</div></div>';
                $(stations[counter]["content"]).insertBefore(".plus-station");
            }
        })
        counter++;
    }
}

function saveStation() {
    var id = $("input#db_id").val();
    if(id == "") {
        saveNewStation();
    } else {
        updateStation(id);
    }
    window.location.reload();
}

function saveNewStation() {
    var name = $("input#name").val();
    var lat = $("input#latitude").val();
    var long = $("input#longitude").val();
    var and_ver = $("input#android_version").val();
    var ser_ver = $("input#services_version").val();
    var api_k = $("input#api_key").val();
    var data = JSON.stringify({ "Name": name, "Latitude": lat, "Longitude": long,
                "AndroidVersion": and_ver, "ServicesVersion": ser_ver, "APIKey": api_k});
    $.ajax({
        type: 'POST',
        url: 'api/Station',
        dataType: 'json',
        async: false,
        data: data,
        contentType: 'application/json'
    });
}

function updateStation(id) {
    for(st of stations_input_changed) {
        var obj = {};
        var api_url = getApiUrl(st);
        var db_name = getDbName(st);
        var value = $("input#" + st).val();
        obj[db_name] = value;
        if(db_name == "Latitude") {
            var long = $("input#longitude").val();
            var data = JSON.stringify({ "Latitude": value, "Longitude":long });
        } else if(db_name == "Longitude") {
            var lat = $("input#latitude").val();
            var data = JSON.stringify({ "Latitude": lat, "Longitude":value });
        } else {
            var data = JSON.stringify(obj);
        }
        $.ajax({
            type: 'PATCH',
            url: 'api/Station/' + id + '/' + api_url,
            dataType: 'json',
            async: false,
            data: data,
            contentType: 'application/json'
        });
    }
}

function getApiUrl(st) {
    switch(st) {
        case "name":
            return "Name";
        case "latitude":
            return "Coordinates";
        case "longitude":
            return "Coordinates";
        case "android_version":
            return "AndroidV";
        case "services_version":
            return "ServicesV";
    }
}

function getDbName(st) {
    switch(st) {
        case "name":
            return "Name";
        case "latitude":
            return "Latitude";
        case "longitude":
            return "Longitude";
        case "android_version":
            return "AndroidVersion";
        case "services_version":
            return "ServicesVersion";
    }
}

function showAlertModal(id) {
    $("input#st_id").val(id);
    $("#alert_modal").modal("show");
}

function deleteStation() {
    var id = $("input#st_id").val();
    $.ajax({
        url : 'api/Station/'+id,
        type: 'DELETE',
        async: false
    });
    window.location.reload();
}

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
        if((body_maximum_height + d + 13) < 85) {
            $(b).height(72 - d);
        } else {
            $(b).height(body_maximum_height);
        }
    }
    if((body_maximum_height + d + 13) < 85) {
        $(".new_station_button").height(85);
    } else {
        $(".new_station_button").height(body_maximum_height + d + 13);
    }    
}

function closeModal(id){
    if(id == 1) {
        $("#station_modal").modal("hide");
    } else {
        $("#alert_modal").modal("hide");
    }
}