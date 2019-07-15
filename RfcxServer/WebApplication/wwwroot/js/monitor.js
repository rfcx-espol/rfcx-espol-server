var sensorsList = [] //[{'id':1,'type':'Temperature','location':'Environment'},{....}];
var idSensorDic = {};
//var dataL=[];
var ind=0, ind2=0;
var stationName=$("#stationName").text();
var stationId = parseInt($("#stationId").text());

charts = [];

//Individual
dataPoints = [];

//URLs
const sensorsByStationURL = `api/Station/${stationId}/Sensor/`;

//STARTS getting data...
window.addEventListener("load", getData);
//setInterval(displayMonitor, 300000);

//get sensors from one station
function getData(){
    $.get(sensorsByStationURL, getSensors);
}


//keep data on sensorsList
function getSensors(data){
    //console.log(data);
    var sensors = JSON.parse(data);

    //sets timestamps for getting data
    var current_date = moment();
    var current_timestamp = current_date.unix();
    var last_timestamp = current_date.clone().subtract(30,'days').unix();
    
    var idChartStat = "chart_statsTab";
    var idChartFilter = "chart_filter_statsTab";

    sensors.forEach(function(sensor){
        let s = {
            stationid : sensor.StationId,
            id : sensor.Id, 
            type : sensor.Type, 
            location :sensor.Location,
        }
        sensorsList.push(s);
        idSensorDic[`${s.type}_${s.location}`]=s.id;
        //var idMin = "minMon"+type+"_"+location;
        //var idMax = "maxMon"+type+"_"+location;
        //var idAvg = "avgMon"+type+"_"+location;
        //var nameDivTab = "tab_"+type+"_"+location;    
    });

    getDataFromSensors(idChartStat, last_timestamp, current_timestamp);
    getDataFromSensors(idChartFilter, last_timestamp, current_timestamp);
    displayMonitor();
}

//Display the monitor
function displayMonitor() {
    for (sensors of sensorsList){
        //Collect data
        var idSensor = sensors['id'];
        
        var firstHour = new Date();
        firstHour.setHours(0,0,0);
        var lastHour = new Date();
        lastHour.setHours(23,59,59);
        
        var startTimestamp = moment(firstHour).unix();
        var lastTimestamp = moment(lastHour).unix();

        var query = '/DataTimestamp?startTimestamp='+startTimestamp+'&endTimestamp='+lastTimestamp;
        $.getJSON('api/Station/' + stationId + '/Sensor/' + idSensor + query, addData);
    }

}
//Add data to list dataL
function addData(data) {
    var data_array = [];
    var unit;
    var station_id, sensor_id;
    //Initialize ind
    if(ind>=sensorsList.length){
        ind= 0;
    }    
    //If there isn't data, take the names from sensorsList
    if(data != null && data.length != 0){
        var typeS = data[0]['Type'];
        var locationS = data[0]['Location'];
        station_id = data[0]['StationId'];
        sensor_id = data[0]['SensorId'];
    }else{
        var typeS = sensorsList[ind]['type'];
        var locationS = sensorsList[ind]['location'];
        station_id = sensorsList[ind]['stationid'];
        sensor_id = sensorsList[ind]['id'];
    }
    var titleVertical = "Temperatura °C";
    var idMin = "minMon"+typeS+"_"+locationS;
    var idMax = "maxMon"+typeS+"_"+locationS;
    var idAvg = "avgMon"+typeS+"_"+locationS;
    var divIdChart = "chartMonitor"+typeS+"_"+locationS;
    colorP="green";
    unit="";
    titleVertical="vertical content";

    ind = ind+1;
    var minValue = 50000;
    var maxValue = 0;
    var sumValue = 0;

    if(data!=null && data.length != 0){
        for(var i = 0; i<data.length; i++){
            var value = parseFloat(data[i].Value);
            var timestamp = parseInt(data[i].Timestamp);            
            
            sumValue = sumValue + value;
            if(value<minValue){
                minValue = value;
            }if(value>maxValue){
                maxValue = value;
            }
            var date = new Date(timestamp*1000);
            var hours = date.getHours()+":"+(date.getMinutes()<10?'0':'') + date.getMinutes();
            data_array.push({
                x: new Date(timestamp*1000),
                y: value,
                hour: hours,
                color: colorP
            });
        }
        var lengthChart = data_array.length;
        var avgValue=(sumValue/lengthChart).toFixed(2);
        
    }else{
        minValue = 0;
        var avgValue = 0;
    }
    $("#"+idMin).text(parseFloat(minValue).toFixed(2));
    $("#"+idMax).text(parseFloat(maxValue).toFixed(2));
    $("#"+idAvg).text(parseFloat(avgValue).toFixed(2));
    displayChart(station_id, sensor_id, divIdChart, titleVertical, colorP, data_array, unit, "DDD/D HH:mm");
}

//Display individual chart
function displayChart(station, sensor, divId, titleVertical, colorL, data, unit, format){
    var chartMon = new CanvasJS.Chart(divId, {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320, 
        theme: "light2",
        toolTip:{   
			content: "<strong>{x}</strong>: {y} " + unit
		},
        axisX:{      
            valueFormatString: format
        },
        axisY: {
            title: titleVertical,
            titleFontSize: 18
        },
        data: [{
            type: "line",
            lineColor: colorL,
            dataPoints: data
        }]
    });

    //keeps the charts to render then
    var divIdList = divId.split("_");
    var typeS = divIdList[1]; locationS = divIdList[2];
    if(divId.includes("Monitor")){
        chartMon.render();
        if(charts["monitor"]==null || charts["monitor"].length==3){
            charts["monitor"]=[chartMon];
        }else{
            charts["monitor"].push(chartMon);
        }
        setInterval(updateChart, 10000, chartMon, station, sensor);
    }
    else{
        charts["tab_"+typeS+"_"+locationS]=chartMon;
    }
}

function updateChart(chartId, stationId, sensorId) {
    console.log("Updating chart");
    var chart_datapoints = chartId.options.data[0].dataPoints;
    $.getJSON('api/Station/' + stationId + '/Sensor/' + sensorId + '/Data/lastData', function(response){
        var timeStamp = parseInt(response.Timestamp);
        var value = parseFloat(response.Value);
        var date = new Date(timeStamp * 1000);
        var hours = date.getHours() + ":" + (date.getMinutes() < 10? '0' : '') + date.getMinutes();

        chart_datapoints.unshift({
            x: date,
            y: value,
            hour: hours,
            color: "#424084"
        });
    });
    chartId.render();
}

// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();

setTimeout(function(){setInputDates()},5000);//wait for page to be rendered before apply this function

//------------------------------------DATES----------------------------------------------

//Set input type dates with actual date to finish input and 6 days before to start input
function setInputDates(){
    var elements=document.getElementsByClassName("start");
    for(element of elements){
        var actual = moment();
        var last = new Date(actual.clone().subtract(1,'week').format("YYYY/MM/DD"));
        element.value= formatDate(last);
        
        var befDay = new Date(actual.clone().subtract(1,'day').format("YYYY/MM/DD"));
        element.setAttribute("max", formatDate(befDay));
    }
    var elements=document.getElementsByClassName("finish");
    for(element of elements){
        element.setAttribute("max", formatDate(new Date()));
        element.value = formatDate(new Date());
    }
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
//---------------------------------INDIVIDUAL CHARTS--------------------------------------

// Pone data en el chart principal de la pestaña de datos actuales 
function getDataFromSensors(idDiv, start, finish) {
    var sensors_series = [];

    for (sensor of sensorsList) {
        //getting basic info of sensors
        var station_id = sensor['stationid'];
        var sensor_id = sensor['id'];

        var query = `api/Station/${station_id}/Sensor/${sensor_id}/DataTimestamp?StartTimestamp=${start}&EndTimestamp=${finish}`;
        //request values by filtering with timestamp, from a specific sensor
        $.getJSON(query, function(result) {
            var sensor_datapoints = [];

            $.each(result, function(i, field) {
                sensor_datapoints.push({
                    x: new Date(field.Timestamp * 1000),
                    y: parseFloat(field.Value)
                });
            });

            sensors_series.push({
                showInLegend: true,
                name: result[0].Type + " " + result[0].Location,
                type: "spline",
                dataPoints: sensor_datapoints
            });
        });
    }
    displayStatsChart(idDiv, sensors_series);
}

function displayStatsChart(div, data_series) {
    var chartStat = new CanvasJS.Chart(div, {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320,
        theme: "light2",
        data: data_series
    });

    if (div.includes("filter")) {
        charts['filtered'] = chartStat
    } else {
        charts['individual'] = chartStat;
    }
}

//Open tab selected
function openDevice(evt, sensor) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
       }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");

    }
    document.getElementById(sensor).style.display = "block";
    evt.currentTarget.className += " active";
    var idC=document.getElementById(sensor).getAttribute("id");
    var chartToRender = charts[idC];

    
    if(chartToRender !== undefined){
        if(idC=="monitor"){
            for(chart of chartToRender){
                chart.render();
            }
        }else{
            chartToRender.render();
            charts['filtered'].render();
            $("#current_tab").trigger('click');
        }
        
    }

}

// Pone data en el chart de la peseña rango de fechas
function getDataByDates(event) {
    var start = moment($(".start").val());
    var finish = moment($("finish").val());
    var updated_axis = [];
    var new_dataseries = [];

    for (key in idSensorDic) {
        var query = "api/Station/" + stationId + "/Sensor/" + idSensorDic[key] + "/DataTimestamp?StartTimestamp=" + start.unix() + "&EndTimestamp=" + finish.unix();
        var sensor_type = sensor['type'];
        var sensor_location = sensor['location'];
        //var nameDataSeries = sensor_type + ' ' + sensor_location;
        var unit = "";
        var axis_type = "";
        //console.log(query);
        /*
        updated_axis.push({
            visible: true,
            title: nameDataSeries,
            lineThickness: 2
        })*/

        $.getJSON(query, function(data) {
            var new_datapoints = [];

            $.each(data, function(i, field) {
                var timestamp = parseInt(field.Timestamp);
                var value = parseFloat(field.Value);
                var date = new Date(timestamp * 1000);
                var hours = date.getHours() + ":" + (date.getMinutes() < 10? '0' : '') + date.getMinutes();
                
                new_datapoints.push({
                    x: date,
                    y: value,
                    hour: hours,
                });
            });
            
            if (data[0].Type == "Temperature") {
                unit = "°C";
                axis_type = "primary";
            } else if (data[0].Type == "Humidity") {
                unit = "%";
                axis_type = "secondary";
            }

            new_dataseries.push({
                showInLegend: true,
                name: data[0].Type + ' ' + data[0].Location,
                axisYType: axis_type,
                type: "line",
                toolTipContent: "<strong>{x}</strong>: {y} " + unit,
                dataPoints: new_datapoints
            });
            charts['filtered'].options.data = new_dataseries;
            charts['filtered'].render();
        });
    }
}

function showChart(show, hide) {
    document.getElementById(show).style.display = "block";
    document.getElementById(hide).style.display = "none";
}

/*
function displayDataSeries(checkbox) {
    var sensor = checkbox.value;
    var individual_chart = charts['individual'];
    if (checkbox.checked) {
        individual_chart.data[sensor - 1].options.visible = true;
    } else { 
        individual_chart.data[sensor - 1].options.visible = false;
    }
    individual_chart.render();
}*/

   /*
    let boxInfoValues  =  '<div class="boxInfoValues">'+
            '<p class="boxLetters  initialMon"><i class="material-icons iconsMinMax">&#xe15d;</i> Min </p><p class="boxLetters initialValue"  id="minVal"></p>'+
            '<p class="boxLetters middle"><i class="material-icons iconsMinMax">&#xe148;</i> Max </p><p class="boxLetters middleValue" id="maxVal"></p>'+
            '<p class="boxLetters last"><i class="fa iconsAvg">&#xf10c;</i> Avg</p><p class="boxLetters lastValue" id="avgVal" ></p>'+
            '</div>'+
        '</div>'+
    '</div>';
    */

    //parameters should be easy to follow up and if constructed, they should be placed in functions
    /*
    if(typeS.toUpperCase().includes("TEMP") && (locationS.includes("DEV") || locationS.includes("STA"))){
        var colorP = "#424084";
        unit = "°C";
    }else if(typeS.toUpperCase().includes("TEMP") && (locationS.toUpperCase().includes("AMB") || locationS.toUpperCase().includes("ENV"))){
        var colorP = "orange";
        unit = "°C";
    }else if(typeS.toUpperCase().includes("HUM") && (locationS.toUpperCase().includes("AMB") || locationS.toUpperCase().includes("ENV"))){
        var colorP = "LightSeaGreen";
        var titleVertical = "Humedad %";
        unit = "%";
    }
    */   
//after creation class fa-tint or fa-thermometer could be added through jquery instead of making these long validations
        /*
        if(type.toUpperCase().includes("HUM") && (location.toUpperCase().includes("AMB") || location.toUpperCase().includes("ENV"))) {
            // var iconTab = '<i class="fa fa-tint tabL"></i> <p class="nameHum">'+type+'-'+location+'</p>';
            var iconTitle = '<i class="fa fa-tint"></i> '+type+'-'+location;
            //idSensorDic[type+"_"+location]=idSensor;
        }else if(type.toUpperCase().includes("TEMP") && (location.toUpperCase().includes( "DEV") || location.toUpperCase().includes("STA"))){
            // var iconTab='<i class="fa" id="mobil">&#xf10b;</i><i class="fa fa-thermometer tabL" ></i> <p class="nameTempDisp">'+type+'-'+location+'</p>';
            var iconTitle = '<i class="fa fa-thermometer"></i> '+type+'-'+location;
            //idSensorDic[type+"_"+location]=idSensor;
            
        }else if(type.toUpperCase().includes("TEMP") && (location.toUpperCase().includes("AMB") || location.toUpperCase().includes("ENV"))){
            // var iconTab = '<i class="fa fa-thermometer tabL"></i> <p class="nameTempAmb">'+type+'-'+location+'</p>';
            var iconTitle = '<i class="fa fa-thermometer"></i> '+type+'-'+location;            
        }*/


//Divs of individual charts
/*
function individualChart(nameChart){
    nameC=nameChart.split("-");
    var type=nameC[0].trim(); var location = nameC[1].trim();
    
    name=type+"-"+location
    var minVal = "minValue"+type+"_"+location;
    var maxVal = "maxValue"+type+"_"+location;
    var avgVal = "avgValue"+type+"_"+location;
    var idDiv = type+"_"+location, idTab = "tab_"+type+"_"+location;
    var idChart = "chart_"+idDiv;
    
    var divEachChart = 
    '<div id='+idTab+' class="col-sm-12 col-md-12 col-lg-12 sensores_monitor" style="display: none;">'+
        '<div id='+idDiv+' style="height: 320px">'+
        '<h4 class="chart-title"> '+stationName+"/ "+name+' </h4>'+
        '<div class="Dates col-lg-12 col-md-12 col-sm-12">'+
            '<a class="exportcsv" id="export_'+idDiv+'" href="#" onclick="downloadCSV(this.id);">EXPORTAR</a>'+
            '<ul class="nav nav-tabs">'+
                '<li class="active"><a data-toggle="tab" href="#data-act-'+idDiv+'">Datos actuales</a></li>'+
                '<li><a data-toggle="tab" href="#date-range-'+idDiv+'">Rango de fechas</a></li>'+
            '</ul>'+
            '<div class="tab-content">'+
                '<div id="data-act-'+idDiv+'" class="tab-pane fade in active">'+
                    '<select id="selectBox'+idDiv+'1" class="selectDiv" onchange="changeFunc(this.id);">'+
                    '<option value="hora">Hora</option>'+
                    '<option value="12horas">12 horas</option>'+
                    '<option value="dia">Día</option>'+
                    '<option value="semana">Semana</option>'+
                    '<option value="mes">Mes</option>'+
                    '</select>'+
                '</div>'+
                '<div id="date-range-'+idDiv+'" class="tab-pane fade">'+
                    '<label>Inicio</label>'+
                    '<input type="date" name="start'+idDiv+'" class="start" min="1899-01-01" max="2000-13-13">'+
                    '<label id="fin"> Fin   </label>'+
                    '<input type="date" name="finish'+idDiv+'" class="finish" >'+
                    '<input type="hidden" id="id'+idDiv+'" value='+idSensorDic[idDiv]+' >'+
                    '<button id="filter_'+idDiv+'" onclick="getDates(this.id)" class="filter">Filtrar</button>'+
                '</div>'+
            '</div>'+
        '</div>'+
        '<div id='+idChart+' class="col-lg-12 col-md-12 col-sm-12" style="height: 320px; clear: right; margin-top: 20px;"></div>'+
        '<div class="boxInfoValues">'+
            '<p class="boxLetters  initialMon"><i class="material-icons iconsMinMax">&#xe15d;</i> Min </p><p class="boxLetters initialValue"  id='+minVal+'></p>'+
            '<p class="boxLetters middle"><i class="material-icons iconsMinMax">&#xe148;</i> Max </p><p class="boxLetters middleValue" id='+maxVal+'></p>'+
            '<p class="boxLetters last"><i class="fa iconsAvg">&#xf10c;</i> Avg</p><p class="boxLetters lastValue" id= '+avgVal+' ></p>'+
            '</div>'+
        '</div>'+
    '</div>';

    $("#individual").append(divEachChart); 
    setInputDates();
}
*/