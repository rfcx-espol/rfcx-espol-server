/*BEGIN:  Station name in select */

$(document).ready(function(){
    stations_input_changed = []
    $.ajax({
        url : 'api/Station/',
        type: 'GET',
        success : getStationsList
    })  

});

function getStationsList(data) {
    var data_dic = JSON.parse(data);
    var combo = document.getElementById("ddl");
    var list_station_name = [];
    var n_item = 0;
    for(station of data_dic){
        list_station_name.push(station['Name']);
        combo.options[n_item].text = station['Name'];  
        n_item ++;
    }

    var a = list_station_name.length;
    var b = combo.length;

    while(b>a){
        b --;
        combo.remove(b);
    }
}

/* END: Station name in select */
