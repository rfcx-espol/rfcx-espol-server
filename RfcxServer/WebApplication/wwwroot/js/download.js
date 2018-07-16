
$(document).ready(function(){
    $("#selectAll").change(function(){
        var cbs = $("input.check");
        if($("#selectAll").is(":checked")){
        cbs.each(function(){
        $(this).prop("checked", true);
        });
    } else {
        cbs.each(function(){
        $(this).prop("checked", false);
        });
    }
    });

    $("#dl").click(function(){
        var cbs = $("input.check");
    var lista_check = "";
    cbs.each(function(){
        if($(this).is(":checked")){
        lista_check = lista_check + $(this).val() + ",";
        }
    });
    lista_check = lista_check.substring(0, lista_check.length - 1);
    $("#lista").attr("value", lista_check);
    if(lista_check.length != 0){
        $("#form2").submit();        
    } else {
        alert("No ha seleccionado ningún archivo");
    }
    });
    
});

var station;


$(window).on("load", function(){
    setInputDates();
    $.ajax({
        url : 'api/Station/',
        type: 'GET',
        async: false,
        success : getStationsList
    })
});
//REVISA LA FUNCION BUSCARAUDIOS()
function getStationsList(data) {
    var data_dic = JSON.parse(data);
    console.log(data_dic);
    for(station of data_dic){
    $("#select").append('<option value='+station["Id"]+'>' + station['Name']+ '</option>');	
    }
    buscarAudios()
}



$(window).on("load", function(){
    $.ajax({
        url : 'api/Audio/',
        type: 'GET',
        async: false,
        success : getAudiosList
    })
});

function getAudiosList(data) {
    var audio_dic = JSON.parse(data);
    for(audio of audio_dic){
    $('#rows-table').append('<tr><td>' + '<input type="checkbox" class="check" />' + '</td><td>' + "audio['Filename']" + '</td><td>' + "audio['RecordingDate'] - audio['Duration']" + '</td><td>' + '<audio controls><source src="/files/audios/Name" type="audio/x-m4a"></audio>' + '</td><td>' + station + '</td></tr>');

    }
}

$(window).on("load", function(){
    $.ajax({
    url : '/api/Station/1/Audio/',
    type: 'GET',
    async: false,
    sucess: getAllAudiosList
    })
});

function getAllAudiosList(data){
    var data_dicAllAudios = JSON.parse(data);
    console.log(data_dicAllAudios);

}

//DATES---------------
//Aparecen las fechas de hoy al cargar la página
function setInputDates(){
    var start = document.getElementById("start");
    var finish = document.getElementById("end");

    start.setAttribute("max", formatDate(new Date()));
    start.value = formatDate(new Date());

    finish.setAttribute("max", formatDate(new Date()));
    finish.value = formatDate(new Date());
}

//Return string of Date with format yyyy-mm-dd
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}

//AQUÍ HAGO POST EN DOWNLOAD Y ENVÍO EL ID DE LA ESTACIÓN, FECHA INI, FECHA FIN 
function buscarAudios(){
    var selectFilter = $("#select")[0];
    var selectValue = selectFilter.options[selectFilter.selectedIndex].value;

    var startInput = $("#start").val();
    var endInput = $("#end").val();
    //Console.log(selectValue+" "+startInput+" "+endInput);
    $.post("/Download", {
        selected: selectValue, 
        start: startInput, 
        end: endInput 
    } , function(data){});
}