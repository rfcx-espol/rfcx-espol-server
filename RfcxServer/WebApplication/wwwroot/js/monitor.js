var sensorsList = [] //[{'id':1,'type':'Temperature','location':'Environment'},{....}];
var idSensorDic = {};
var dataL=[];
var ind=0, ind2=0;
var stationName=$("#stationName").text();
var stationId = parseInt($("#stationId").text());

charts = [];

//Individual
dataPoints = [];
window.addEventListener("load", getData);
//setInterval(displayMonitor, 300000);

//get sensors from one station
function getData(){
    $.get('api/Station/'+parseInt(stationId)+'/Sensor/',getSensors);
}
//keep data on sensorsList
function getSensors(data){
    var sensors = JSON.parse(data);
    for( sensor of sensors){
        var sensorsInf = {};
        var idSensor = parseInt(sensor['Id']);
        var type = sensor['Type'];
        var location = sensor['Location'];
        var idStation = sensor['StationId'];

        sensorsInf['stationid'] = idStation;
        sensorsInf['id']= idSensor;
        sensorsInf['type']=type;
        sensorsInf['location']=location;
        sensorsList.push(sensorsInf);
        var idMin = "minMon"+type+"_"+location;
        var idMax = "maxMon"+type+"_"+location;
        var idAvg = "avgMon"+type+"_"+location;
        var divIdChart = "chartMonitor"+type+"_"+location;
        var nameDivTab = "tab_"+type+"_"+location;
        if(type.toUpperCase().includes("HUM") && (location.toUpperCase().includes("AMB") || location.toUpperCase().includes("ENV"))) {
            // var iconTab = '<i class="fa fa-tint tabL"></i> <p class="nameHum">'+type+'-'+location+'</p>';
            var iconTitle = '<i class="fa fa-tint"></i> '+type+'-'+location;
            idSensorDic[type+"_"+location]=idSensor;
        }else if(type.toUpperCase().includes("TEMP") && (location.toUpperCase().includes( "DEV") || location.toUpperCase().includes("STA"))){
            // var iconTab='<i class="fa" id="mobil">&#xf10b;</i><i class="fa fa-thermometer tabL" ></i> <p class="nameTempDisp">'+type+'-'+location+'</p>';
            var iconTitle = '<i class="fa fa-thermometer"></i> '+type+'-'+location;
            idSensorDic[type+"_"+location]=idSensor;
            
        }else if(type.toUpperCase().includes("TEMP") && (location.toUpperCase().includes("AMB") || location.toUpperCase().includes("ENV"))){
            // var iconTab = '<i class="fa fa-thermometer tabL"></i> <p class="nameTempAmb">'+type+'-'+location+'</p>';
            var iconTitle = '<i class="fa fa-thermometer"></i> '+type+'-'+location;
            idSensorDic[type+"_"+location]=idSensor;
        }
        //Create divs
        createDivsMonitor(iconTitle, divIdChart, idMin, idMax, idAvg);
        //createTabs(idSensor, iconTab, nameDivTab);
        //startDisplayEachChart(idSensor);

    }
    createTab();
    indvChart("statsTab", stationName);
    getDataFromSensors();
    displayMonitor();
}

//Create divs of tabs
function createTabs(idSensor, iconTab, nameDivTab){
    name = "'"+nameDivTab+"'";
    var divTab = '<button class="tablinks" name='+nameDivTab+' id='+idSensor+' onclick="getDataSensor(this.id)" onfocus="openDevice(event, '+name+')">'+iconTab+'</button>';
    $("#tab").append(divTab);
}

function createTab() {
    name = "'individual'";
    var divTab = '<button class="tablinks" name="statsTab" id="stats" onfocus="openDevice(event,' + name + ')"><i class="fa fa-bar-chart tabL" aria-hidden="true"></i><p class="tabMonitor">Estadisticas</p></button>'; 
    $("#tab").append(divTab);
}

//Divs of monitor
function createDivsMonitor(iconTab, divIdChart, idMin,  idMax,  idAvg) {
    var div = "<div class='col-sm-12 col-md-12 col-lg-12 sensores_monitor'>"+
                "<h4 class='titulo_sensor'>"+iconTab+"</h4>"+
                "<div id='"+divIdChart+"' style='height: 320px'></div>"+
                "<div class='boxInfoValues'>"+
                "<p class='boxLetters initialMon'><i class='material-icons iconsMinMax'>&#xe15d;</i> Min </p><p class='boxLetters initialValue'  id="+idMin+"></p>"+
                "<p class='boxLetters middle' ><i class='material-icons iconsMinMax'>&#xe148;</i> Max </p><p class='boxLetters middleValue' id="+idMax+"></p>"+
                "<p class='boxLetters last'><i class='fa  iconsAvg'>&#xf10c;</i> Avg</p><p class='boxLetters lastValue' id="+idAvg+"></p>"+
                "</div>"+
                "<hr>"+
            "</div>"
    $("#monitor").append(div);
} 

//Display the monitor
function displayMonitor() {
    for (sensors of sensorsList){
        //Collect data
        var idSensor = sensors['id'];

        /*
        var actual = moment();
        var actualTimestamp = actual.unix();
        var lastTimestamp = actual.clone().subtract(2,'hours').unix();
        */

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
            dataL.push({
                x: new Date(timestamp*1000),
                y: value,
                hour: hours,
                color: colorP
            });
        }
        var lengthChart = dataL.length;
        var avgValue=(sumValue/lengthChart).toFixed(2);
        
    }else{
        minValue = 0;
        var avgValue = 0;
    }
    $("#"+idMin).text(minValue);
    $("#"+idMax).text(maxValue);
    $("#"+idAvg).text(avgValue);
    displayChart(station_id, sensor_id, divIdChart, titleVertical, colorP, dataL, unit, "DDD/D HH:mm");
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
        setInterval(updateChart, 10000, chartMon, data, station, sensor);
    }
    else{
        charts["tab_"+typeS+"_"+locationS]=chartMon;
    }

    dataL=[];
    dataPoints = [];
}

function updateChart(chartId, data_points, stationId, sensorId) {
    console.log("Updating chart");
    $.getJSON('api/Station/' + stationId + '/Sensor/' + sensorId + '/Data/lastData', function(response){
        var timeStamp = parseInt(response.Timestamp);
        var value = parseInt(response.Value);
        var date = new Date(timeStamp * 1000);
        var hours = date.getHours() + ":" + (date.getMinutes() < 10? '0' : '') + date.getMinutes();

        data_points.push({
            x: date,
            y: value,
            hour: hours,
            color: "#424084"
        });
        chartId.render();
    });
}

// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();

//Divs of individual charts
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
                    '<select id="selectBox'+idDiv+'2" class="selectDiv" onchange="changeFunc(this.id);">'+
                    '<option disabled selected value> -- Escoge una opción -- </option>'+
                    '<option value="hora">Hora</option>'+
                    '<option value="12horas">12 horas</option>'+
                    '<option value="dia">Día</option>'+
                    '<option value="semana">Semana</option>'+
                    '<option value="mes">Mes</option>'+
                    '</select>'+
                    '<button id="filter_'+idDiv+'" disabled onclick="getDates(this.id)" class="filter">Filtrar</button>'+
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

function indvChart(ChartDivId, stationName) {
    var checkboxes = '';
    for (var [sensor, id] of Object.entries(idSensorDic)) {
        checkboxes +=
        '<input checked type="checkbox" value="'+ id + '" onchange="displayDataSeries(this)">'+
        '<label>' + sensor + '</label>'
    }
    var idTab = "tab_" + ChartDivId;
    var idChart = "chart_" + ChartDivId;
    var divEachChart = 
    '<div id="'+ idTab +'" class="col-sm-12 col-md-12 col-lg-12">'+
        '<div id='+ ChartDivId +' style="height: 320px;">'+
        '<h3 class="titulo_sensor">Estadísticas diarias de '+ stationName +'</h3>'+
        '<div class="Dates col-lg-12 col-md-12 col-sm-12">'+
            '<a class="exportcsv" id="export_'+ ChartDivId +'" href="#" onclick="downloadCSV(this.id);">EXPORTAR</a>'+
            '<ul class="nav nav-tabs">'+
                '<li class="active"><a data-toggle="tab" href="#data-act-'+ ChartDivId +'">Datos actuales</a></li>'+
                '<li><a data-toggle="tab" href="#date-range-'+ ChartDivId +'">Rango de fechas</a></li>'+
            '</ul>'+
            '<div class="tab-content">'+
                '<div id="data-act-' + ChartDivId + '" class="tab-pane fade in active">'+
                    checkboxes+
                '</div>'+
                '<div id="date-range-'+ ChartDivId +'" class="tab-pane fade">'+
                    '<label>Inicio</label>'+
                    '<input type="date" name="start" class="start" min="1899-01-01" max="2000-13-13">'+
                    '<label id="fin"> Fin   </label>'+
                    '<input type="date" name="finish" class="finish" >'+
                    '<input type="hidden" id="id'+idTab+'" value='+idSensorDic[idTab]+' >'+
                    '<select id="selectBox" class="selectDiv" onchange="changeFunc(this.id);">'+
                    '<option disabled selected value> -- Escoge una opción -- </option>'+
                    '<option value="hora">Hora</option>'+
                    '<option value="12horas">12 horas</option>'+
                    '<option value="dia">Día</option>'+
                    '<option value="semana">Semana</option>'+
                    '<option value="mes">Mes</option>'+
                    '</select>'+
                    '<button id="filter_'+idTab+'" disabled onclick="getDates(this.id)" class="filter">Filtrar</button>'+
                '</div>'+
            '</div>'+
        '</div>'+
        '<div id='+idChart+' class="col-lg-12 col-md-12 col-sm-12" style="height: 320px; clear: right; margin-top: 20px;"></div>'+
        '<div class="boxInfoValues">'+
            '<p class="boxLetters  initialMon"><i class="material-icons iconsMinMax">&#xe15d;</i> Min </p><p class="boxLetters initialValue"  id="minVal"></p>'+
            '<p class="boxLetters middle"><i class="material-icons iconsMinMax">&#xe148;</i> Max </p><p class="boxLetters middleValue" id="maxVal"></p>'+
            '<p class="boxLetters last"><i class="fa iconsAvg">&#xf10c;</i> Avg</p><p class="boxLetters lastValue" id="avgVal" ></p>'+
            '</div>'+
        '</div>'+
    '</div>';
    $("#individual").append(divEachChart);
    setInputDates();
}

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

//ask for the data according to a timestamp
function startDisplayEachChart(id) {
    var actual = moment();
    var actualTimestamp = actual.unix();
    var lastTimestamp = actual.clone().subtract(1,'hour').unix();

    //var query = 'api/Station/'+stationId+'/Sensor/'+id+'/DataTimestamp/Filter?StartTimestamp='+lastTimestamp+'&endTimestamp='+actualTimestamp+"&Filter=Days&FilterValue=1";
    var query = "api/Station/"+stationId+"/Sensor/"+id+"/DataTimestamp?StartTimestamp="+lastTimestamp+"&EndTimestamp="+actualTimestamp;
    $.getJSON(query, addDataEachChart);
}

function getDataFromSensors() {
    var sensors_series = [];
    var sensors_axis = [];

    for (sensor of sensorsList) {
        var station_id = sensor['stationid'];
        var sensor_id = sensor['id'];
        var sensor_type = sensor['type'];
        var sensor_location = sensor['location'];

        var nameDataSeries = sensor_type + ' ' + sensor_location;

        sensors_axis.push({
            visible: true,
            title: nameDataSeries,
            lineThickness: 2
        })

        var query = 'api/Station/' + station_id + '/Sensor/' + sensor_id + '/Data/';
        $.getJSON(query, function(result) {
            var sensor_datapoints = [];

            $.each(result, function(i, field) {
                sensor_datapoints.push({
                    x: new Date(field.Timestamp * 1000),
                    y: parseInt(field.Value)
                });
            });
            sensors_series.push({
                type: "line",
                dataPoints: sensor_datapoints
            });
        });
    }

    displayStatsChart(sensors_series, sensors_axis);
}

function displayStatsChart(data_series, data_axis) {
    var chartStat = new CanvasJS.Chart("chart_statsTab", {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320,
        theme: "light2",
        axisY: data_axis,
        data: data_series
    });

    charts['individual'] = chartStat;
}

//Add the data for the graph
function addDataEachChart(data){
    if(ind2>=sensorsList.length){
        ind2= 0;
    }
    //If there isn't data, take the names from sensorsList
    if(data!=null && data.length != 0 ){
        var typeS = data[0]['Type'];
        var locationS = data[0]['Location'];
        //Boxes min, max, avg
        var minV = 5000; var maxV = 0; var sumV = 0;
    }
    else{
        var typeS = sensorsList[ind2]['type'];
        var locationS = sensorsList[ind2]['location'];
        //Boxes min, max, avg
        var minV = 0; var maxV = 0; var avgV = 0;
    }
    var minValId = "minValue"+typeS+"_"+locationS;
    var maxValId = "maxValue"+typeS+"_"+locationS;
    var avgValId = "avgValue"+typeS+"_"+locationS;
    var chartId = "chart_"+typeS+"_"+locationS;
    var titleVertical = "Temperatura °C";
    if(typeS.toUpperCase().includes("TEMP") && (locationS.toUpperCase().includes("DEV") || locationS.toUpperCase().includes("STA"))){
        var colorP = "#424084";
    }else if(typeS.toUpperCase().includes("TEMP") && (locationS.toUpperCase().includes("AMB") || locationS.toUpperCase().includes("ENV"))){
        var colorP = "orange";
    }else if(typeS.toUpperCase().includes("HUM") && (locationS.toUpperCase().includes("AMB") || locationS.toUpperCase().includes("ENV"))){
        var colorP = "LightSeaGreen";
        var titleVertical = "Humedad %";
    }
    if(data!=null){
        for(points of data){
        var type = points['Type'];
        var time = parseInt(points['Timestamp']);
        var value = parseFloat(points['Value']);
        
        sumV = sumV + value;
        if(value<minV){
            minV = value;
        }if(value>maxV){
            maxV = value;
        }
        var date = new Date(time*1000);
        var hours = date.getHours()+":"+(date.getMinutes()<10?'0':'') + date.getMinutes();
        dataPoints.push({
            x: new Date(time*1000),
            y: value,
            hour: hours,
            color: colorP
        });
    }
    }
    
    ind2 = ind2 + 1;
    var lengthChart = dataPoints.length;
    var avgV = (sumV/lengthChart).toFixed(2);
    if (isNaN(avgV)){
        avgV = 0;
    }
    $("#"+minValId).text(minV);
    $("#"+maxValId).text(maxV);
    $("#"+avgValId).text(avgV);
    
    displayChart(chartId, titleVertical, colorP, dataPoints, "DDD/D HH:mm", "<strong> {hour}</strong>: {y}");
}

//Open tab selected
function openDevice(evt, sensor) {
    console.log("Opending device");
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
        }
        
    }

}

function displayDataSeries(checkbox) {
    var sensor = checkbox.value;
    var individual_chart = charts['individual'];
    if (checkbox.checked) {
        individual_chart.data[sensor - 1].options.visible = true;
    } else { 
        individual_chart.data[sensor - 1].options.visible = false;
    }
    individual_chart.render();
}

function getDataByDates() {
    console.log(idSensorDic);
}