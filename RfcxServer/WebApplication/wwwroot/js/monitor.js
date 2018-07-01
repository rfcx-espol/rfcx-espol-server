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

setInterval(displayMonitor, 300000);
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

        sensorsInf['id']= idSensor;
        sensorsInf['type']=type;
        sensorsInf['location']=location;
        sensorsList.push(sensorsInf);
        
        if(type.includes("Hum") && (location.includes("Amb") || location.includes("Env"))) {
            var idMin = "minMonHum";
            var idMax = "maxMonHum";
            var idAvg = "avgMonHum";
            var divIdChart = "chartMonitorHum";
            var nameDivTab = "'tab_Hum_Env'";
            var iconTab = '<i class="fa fa-tint"></i> Humedad - Ambiente';
            idSensorDic["Humedity"]=idSensor;


        }else if(type.includes("Temp") && (location.includes( "Dev") || location.includes("Sta"))){
            var idMin = "minMonDis";
            var idMax = "maxMonDisp";
            var idAvg = "avgMonDisp";
            var divIdChart = "chartMonitorSta";
            var nameDivTab = "'tab_Temp_Sta'";
            var iconTab='<i class="fa fa-thermometer" ></i> Temperatura - Dispositivo';
            idSensorDic["Temp_Sta"]=idSensor;
            
        }else if(type.includes("Temp") && (location.includes("Amb") || location.includes("Env"))){
            var idMin = "minMonAmb";
            var idMax = "maxMonAmb";
            var idAvg = "avgMonAmb";
            var divIdChart = "chartMonitorEnv";
            var nameDivTab = "'tab_Temp_Env'";
            var iconTab = '<i class="fa fa-thermometer"></i> Temperatura - Ambiente';
            idSensorDic["Temp_Env"]=idSensor;
        }
        
        //Create divs
        createDivsMonitor(iconTab, divIdChart, idMin, idMax, idAvg);
        createTabs(idSensor, iconTab, nameDivTab);
        individualChart(type+" - "+location);
        startDisplayEachChart(idSensor);

    }
    displayMonitor();
}

//Create divs of tabs
function createTabs(idSensor, iconTab, nameDivTab){
    var divTab = '<button class="tablinks" name='+nameDivTab+' id='+idSensor+' onclick="openDevice(event, '+nameDivTab+')">'+iconTab+'</button>';
    $("#tab").append(divTab);
}

//Divs of monitor
function createDivsMonitor(iconTab, divIdChart, idMin,  idMax,  idAvg) {
    var div = "<div class='col-sm-12 col-md-12 col-lg-12 sensores_monitor'>"+
                "<h4 class='titulo_sensor'>"+iconTab+"</h4>"+
                "<div id='"+divIdChart+"' style='height: 320px'></div>"+
                "<div id='boxInfoValues'>"+
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
        var currentTime = new Date(1529794207*1000);//Erase 1529794207*1000
        var current = Math.round(currentTime.getTime()/1000);
        var lastTimestamp = Math.round(getLastTimeStampHour(current*1000, 2)/1000);
        var query = '/DataTimestamp?startTimestamp='+lastTimestamp+'&endTimestamp='+current;
        
        $.getJSON('api/Station/'+stationId+'/Sensor/'+idSensor+query, addData);
    }

}
//Add data to list dataL
function addData(data) {
    //Initialize ind
    if(ind>=sensorsList.length){
        ind= 0;
    }
    //If there isn't data, take the names from sensorsList
    if(data.length != 0 ){
        var typeS = data[0]['Type'];
        var locationS = data[0]['Location'];
    }else{
        var typeS = sensorsList[ind]['type'];
        var locationS = sensorsList[ind]['location'];
    }
    var titleVertical = "Temperatura °C";
    if(typeS.includes("Temp") && (locationS.includes("Dev") || locationS.includes("Sta"))){
        var colorP = "#424084";
        var idMin = "minMonDis";
        var idMax = "maxMonDisp";
        var idAvg = "avgMonDisp";
        var divIdChart = "chartMonitorSta";
    }else if(typeS.includes("Temp") && (locationS.includes("Amb") || locationS.includes("Env"))){
        var colorP = "orange";
        var idMin = "minMonAmb";
        var idMax = "maxMonAmb";
        var idAvg = "avgMonAmb";
        var divIdChart = "chartMonitorEnv";
    }else if(typeS.includes("Hum") && (locationS.includes("Amb") || locationS.includes("Env"))){
        var colorP = "LightSeaGreen";
        var titleVertical = "Humedad °H";
        var idMin = "minMonHum";
        var idMax = "maxMonHum";
        var idAvg = "avgMonHum";
        
        var divIdChart = "chartMonitorHum";
    }

    ind = ind+1;
    var minValue = 50000;
    var maxValue = 0;
    var sumValue = 0;

    if(data.length != 0){
        for(var i = 0; i<data.length; i++){
            var value = parseInt(data[i].Value);
            var timestamp = parseInt(data[i].Timestamp);            
            
            sumValue = sumValue + value;
            if(value<minValue){
                minValue = value;
            }if(value>maxValue){
                maxValue = value;
            }
            dataL.push({
                x: new Date(timestamp*1000),
                y: value,
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

    displayChart(divIdChart, titleVertical, colorP, dataL, "hh:mm TT");
}

//Display individual chart
function displayChart(divId, titleVertical, colorL, data, format){
    var chartMon = new CanvasJS.Chart(divId, {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320,
        theme: "light2",
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
    if(divId.includes("Monitor")){
        chartMon.render();
        if(charts["monitor"]==null || charts["monitor"].length==3){
            charts["monitor"]=[chartMon];
        }else{
            charts["monitor"].push(chartMon);
        }
    }
    else if(divId.includes("Hum")){
        charts["tab_Hum_Env"]=chartMon;
    }else if(divId.includes("Sta")){
        charts["tab_Temp_Sta"]=chartMon;
    }else if(divId.includes("Env")){
        charts["tab_Temp_Env"]=chartMon;
    }

    //chartMon.render();
    dataL=[];
    dataPoints = [];
    
}

// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();

//Divs of individual charts
function individualChart(nameChart){
    if(nameChart.includes("Temp") && nameChart.includes("Env")){
        var name = "Temperatura - Ambiente";
        var minVal = "minValueAmb";
        var maxVal = "maxValueAmb";
        var avgVal = "avgValueAmb";
        var idDiv = "Temp_Env", idTab = "tab_Temp_Env";
    }else if(nameChart.includes("Temp") && nameChart.includes("Sta")){
        var name = "Temperatura - Dispositivo";
        var minVal = "minValueTemp";
        var maxVal = "maxValueTemp";
        var avgVal = "avgValueTemp";  
        var idDiv = "Temp_Sta", idTab = "tab_Temp_Sta";  
    }else if(nameChart.includes("Hum") && nameChart.includes("Env")){
        var name = "Humedad - Ambiente";
        var minVal = "minValueHum";
        var maxVal = "maxValueHum";
        var avgVal = "avgValueHum";
        var idDiv = "Humedity", idTab = "tab_Hum_Env";
    }
    var idChart = "chart_"+idDiv;
    //If there is another sensor, add else if
    var divEachChart = 
    '<div id='+idTab+' class="col-sm-12 col-md-12 col-lg-12 tabcontent" style="display: none;">'+
        '<div id='+idDiv+'>'+
        '<h4 class="chart-title"> '+stationName+"/ "+name+' </h4>'+
        '<div class="Dates">'+
            '<label>Inicio</label>'+
            '<input type="date" name="start'+idDiv+'" class="start" min="1899-01-01" max="2000-13-13">'+
            '<label id="fin"> Fin   </label>'+
            '<input type="date" name="finish'+idDiv+'" class="finish" ><br>'+
            '<input type="hidden" id="id'+idDiv+'" value='+idSensorDic[idDiv]+' ><br>'+
            '<button id="filter_'+idDiv+'" onclick="getDates(this.id)" class="filter" >Filtrar</button>'+
            
        '</div>'+
        '<div id='+idChart+' style="height: 320px; clear: right;"></div>'+
        '<div id="boxInfoValues">'+
            '<p class="boxLetters  initial"><i class="material-icons iconsMinMax">&#xe15d;</i> Min </p><p class="boxLetters initialValue"  id='+minVal+'></p>'+
            '<p class="boxLetters middle"><i class="material-icons iconsMinMax">&#xe148;</i> Max </p><p class="boxLetters middleValue" id='+maxVal+'></p>'+
            '<p class="boxLetters last"><i class="fa iconsAvg">&#xf10c;</i> Avg</p><p class="boxLetters lastValue" id= '+avgVal+' ></p>'+
            '</div>'+
        '</div>'+
    '</div>';

    $("#individual").append(divEachChart); 
    setInputDates();
}

//------------------------------------DATES----------------------------------------------

//Return timestamp of n hours before
function getLastTimeStampHour(timestamp, hours){
    var currentDate = new Date(timestamp);
    var newDate = currentDate.setHours(currentDate.getHours() - hours);
    return newDate;
}

//Return timestamp of n days before
function getLastTimeStampDay(timestamp, days){
    var currentDate = new Date(timestamp);
    var newDate = currentDate.setDate(currentDate.getDate() - days);
    return newDate;
}
function setMaxDateNow(){
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth()+1; //January is 0!
    var yyyy = today.getFullYear();
     if(dd<10){
            dd='0'+dd
        } 
        if(mm<10){
            mm='0'+mm
        } 
  
    today = yyyy+'-'+mm+'-'+dd;
    return today;
}
function setInputDates(){
    var elements=document.getElementsByClassName("start");
    for(element of elements){
        element.setAttribute("max", setMaxDateNow());
        
        var currentTime = new Date();
        var dateN = Math.round(currentTime.getTime()/1000);
        var last = new Date(Math.round(getLastTimeStampDay(dateN*1000, 6)));
        element.valueAsDate = last;
    }
    var elements=document.getElementsByClassName("finish");
    for(element of elements){
        element.setAttribute("max", setMaxDateNow());
        element.value = setMaxDateNow();
    }
}

//---------------------------------INDIVIDUAL CHARTS--------------------------------------

//ask for the data according to a timestamp
function startDisplayEachChart(id) {
    var currentTime = new Date(1529794207*1000);//Erase 1529794207*1000
    var current = Math.round(currentTime.getTime()/1000);
    var lastTimestamp = Math.round(getLastTimeStampDay(current*1000, 2)/1000);
    var query = '/DataTimestamp?startTimestamp='+lastTimestamp+'&endTimestamp='+current;
    $.getJSON('api/Station/'+stationId+'/Sensor/'+id+query, addDataEachChart);
}

//Add the data for the graph
function addDataEachChart(data){
    if(ind2>=sensorsList.length){
        ind2= 0;
    }
    //If there isn't data, take the names from sensorsList
    if(data.length != 0 ){
        var typeS = data[0]['Type'];
        var locationS = data[0]['Location'];
        //Boxes min, max, avg
        var minV = 5000; var maxV = 0; var sumV = 0;
    }else{
        var typeS = sensorsList[ind2]['type'];
        var locationS = sensorsList[ind2]['location'];
        //Boxes min, max, avg
        var minV = 0; var maxV = 0; var avgV = 0;
    }
    var titleVertical = "Temperatura °C";
    if(typeS.includes("Temp") && (locationS.includes("Dev") || locationS.includes("Sta"))){
        var colorP = "#424084";
        var minValId = "minValueTemp";
        var maxValId = "maxValueTemp";
        var avgValId = "avgValueTemp";
        var chartId = "chart_Temp_Sta";
    }else if(typeS.includes("Temp") && (locationS.includes("Amb") || locationS.includes("Env"))){
        var colorP = "orange";
        var minValId = "minValueAmb";
        var maxValId = "maxValueAmb";
        var avgValId = "avgValueAmb";
        var chartId = "chart_Temp_Env";
    }else if(typeS.includes("Hum") && (locationS.includes("Amb") || locationS.includes("Env"))){
        var colorP = "LightSeaGreen";
        var titleVertical = "Humedad °H";
        var minValId = "minValueHum";
        var maxValId = "maxValueHum";
        var avgValId = "avgValueHum";
        var chartId = "chart_Humedity";
    }
    
    for(points of data){
        var type = points['Type'];
        var time = parseInt(points['Timestamp']);
        var value = parseInt(points['Value']);
        var location = points['Location'];
        
        sumV = sumV + value;
        if(value<minV){
            minV = value;
        }if(value>maxV){
            maxV = value;
        }
        var date = new Date(time*1000);
        dataPoints.push({
            x: new Date(time*1000),
            y: value,
            color: colorP
        });
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
    
    displayChart(chartId, titleVertical, colorP, dataPoints, "DDD DD");
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
        }
        
    }
    
}