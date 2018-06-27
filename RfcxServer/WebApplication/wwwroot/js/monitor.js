var sensorsList = [] //[{'id':1,'type':'Temperature','location':'Environment'},{....}];
var dataL=[];
var ind=0;
var stationName=$("#stationName").text();
var stationId = parseInt($("#stationId").text());

//Individual
dataPoints = [];

window.onload = getData();

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

        if(type.includes("Hum") && (location.includes("Amb")) || location.includes("Env")) {
            var idMin = "minMonHum";
            var idMax = "maxMonHum";
            var idAvg = "avgMonHum";
            var divIdChart = "chartMonitorHum";
            var nameDivTab = "'humedad'";
            var iconTab = '<i class="fa fa-tint"></i> Humedad - Ambiente';

        }else if(type.includes("Temp") && (location.includes( "Dev")) || location.includes("Sta")){
            var idMin = "minMonDis";
            var idMax = "maxMonDisp";
            var idAvg = "avgMonDisp";
            var divIdChart = "chartMonitorDisp";
            var nameDivTab = "'temp_disp'";
            var iconTab='<i class="fa fa-thermometer" ></i> Temperatura - Dispositivo';
            
        }else if(type.includes("Temp") && (location.includes("Amb")) || location.includes("Env")){
            var idMin = "minMonAmb";
            var idMax = "maxMonAmb";
            var idAvg = "avgMonAmb";
            var divIdChart = "chartMonitorAmb";
            var nameDivTab = "'temp_amb'";
            var iconTab = '<i class="fa fa-thermometer"></i> Temperatura - Ambiente'
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
    if(typeS.includes("Temp") && (locationS.includes("Dev"))|| locationS.includes("Sta")){
        var colorP = "#424084";
        var idMin = "minMonDis";
        var idMax = "maxMonDisp";
        var idAvg = "avgMonDisp";
        var divIdChart = "chartMonitorDisp";
    }else if(typeS.includes("Temp") && (locationS.includes("Amb"))|| locationS.includes("Env")){
        var colorP = "orange";
        var idMin = "minMonAmb";
        var idMax = "maxMonAmb";
        var idAvg = "avgMonAmb";
        var divIdChart = "chartMonitorAmb";
    }else if(typeS.includes("Hum") && (locationS.includes("Amb"))|| locationS.includes("Env")){
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
    chartMon.render();
    dataL=[];
    dataPoints = [];
    
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
}

// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();

//Divs of individual charts
function individualChart(nameChart){
    if(nameChart.includes("Temp") && (nameChart.includes("Amb")) || nameChart.includes("Env")){
        var minVal = "minValueAmb";
        var maxVal = "maxValueAmb";
        var avgVal = "avgValueAmb";
        var idDiv = "temp_amb";
    }else if(nameChart.includes("Temp") && (nameChart.includes("Sta")) || nameChart.includes("Dev")){
        var minVal = "minValueTemp";
        var maxVal = "maxValueTemp";
        var avgVal = "avgValueTemp";  
        var idDiv = "temp_disp";  
    }else if(nameChart.includes("Hum") && (nameChart.includes("Amb")) || nameChart.includes("Env")){
        var minVal = "minValueHum";
        var maxVal = "maxValueHum";
        var avgVal = "avgValueHum";
        var idDiv = "humedad";
    }
    var idChart = "chart_"+idDiv;
    //If there is another sensor, add else if
    var divEachChart = '<div id='+idDiv+' class="col-sm-12 col-md-12 col-lg-12 tabcontent activo row-fluid" width="1000" style="display: none;">'+
    '<h4 class="chart-title"> '+stationName+"/ "+nameChart+' </h4>'+
        '<div id='+idChart+' style="height: 320px;"></div>'+
           '<div id="boxInfoValues">'+
            '<p class="boxLetters  initial"><i class="material-icons iconsMinMax">&#xe15d;</i> Min </p><p class="boxLetters initialValue"  id='+minVal+'></p>'+
            '<p class="boxLetters middle"><i class="material-icons iconsMinMax">&#xe148;</i> Max </p><p class="boxLetters middleValue" id='+maxVal+'></p>'+
            '<p class="boxLetters last"><i class="fa iconsAvg">&#xf10c;</i> Avg</p><p class="boxLetters lastValue" id= '+avgVal+' ></p>'+
            '</div>'+
    '</div>';

    $("#individual").append(divEachChart); 
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
    //Boxes min, max, avg
    var minV = 5000; var maxV = 0; var sumV = 0;
    for(points of data){
        var type = points['Type'];
        var time = parseInt(points['Timestamp']);
        var value = parseInt(points['Value']);
        var location = points['Location'];
        var titleVertical = "Temperatura °C";
        if(type.includes("Temp") && (location.includes("Sta") || location.includes("Dev"))){
            var colorP= "#424084";
            var minValId = "minValueTemp";
            var maxValId = "maxValueTemp";
            var avgValId = "avgValueTemp";
            var chartId = "chart_temp_disp";
        }else if(type.includes("Temp") && (location.includes("Env") || location.includes("Amb"))){
            var colorP= "orange";
            var minValId = "minValueAmb";
            var maxValId = "maxValueAmb";
            var avgValId = "avgValueAmb";
            var chartId = "chart_temp_amb";
        }else if(type.includes("Hum") && (location.includes("Env") || location.includes("Amb"))){
            var colorP= "LightSeaGreen";
            var minValId = "minValueHum";
            var maxValId = "maxValueHum";
            var avgValId = "avgValueHum";
            var chartId = "chart_humedad";
            var titleVertical = "Humedad °H";
        }
        
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
    var lengthChart = dataPoints.length;
    var avgV = (sumV/lengthChart).toFixed(2);

    $("#"+minValId).text(minV);
    $("#"+maxValId).text(maxV);
    $("#"+avgValId).text(avgV);
    
    displayChart(chartId, titleVertical, colorP, dataPoints, "DDD DD");
}