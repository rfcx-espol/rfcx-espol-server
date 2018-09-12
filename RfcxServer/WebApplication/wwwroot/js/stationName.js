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
    var list_station_N = [];

    for(station of data_dic){
        list_station_name.push(station['Name']);
        list_station_N.push(station['Id']);
    }    

    var id;
    var zl;
    var l = 0;
    var ln = list_station_N.length;
    for(var t=0; t<ln; t++){

        zl = combo.options[t].value;
        l = zl.length;

        id = combo.options[t].value.substr( 7, l);

        for(var y=0; y<ln; y++){
            console.log("list_station_N[y]: " + list_station_N[y]);
            console.log("id: " + id);
            if(id == (list_station_N[y])){
                
                combo.options[t].text = list_station_name[y];
                console.log(combo.options[t].text + " == " + list_station_name[y]);
            }
        }

        
    }    
}

/* END: Station name in select */
