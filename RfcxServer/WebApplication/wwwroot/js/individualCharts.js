var stationId = parseInt($("#stationId").text());
var dataPoints = [];
var dataSensor = {};
//get location and type of actual sensor

function getDataSensor(idSensor){
    $.getJSON('api/Sensor/'+idSensor, function(data){
        dataSensor['location'] = data['Location'];
        dataSensor['type'] = data['Type'];
        dataSensor['id'] = data['Id'];
    });
}
//get the dates from inputs and make the query
function getDates(id){
    var idButton = id.substring(7,id.length);
    var nameStart = "start"+idButton;
    var nameFinish = "finish"+idButton;
    var start = moment($("input[name="+nameStart+"]").val());  
    var finish = moment($("input[name="+nameFinish+"]").val()); 
    var startUnixTimestamp = start.unix();
    var finishUnixTimestamp = finish.unix();

    var n = (finish.date() - start.date());
    if(n!=0){
        filterByTimeStamp(n, startUnixTimestamp, finishUnixTimestamp);
    }
    
}
function getLegends(){
    var location = dataSensor["location"];
    var type = dataSensor["type"];
    var titleVertical = "Temperatura °C";
    if(type=="Humidity" && location=="Environment"){
        type=type.replace("Humidity", "Humedad");
        location = location.replace("Environment", "Ambiente");

        var colorP = "#424084";
        var titleVertical = "Humedad °H";
        var minValId = "minValueHum";
        var maxValId = "maxValueHum";
        var avgValId = "avgValueHum";
        var chartId = "chart_Humedity";
    }
    else if(type=="Temperature" && location=="Environment"){
        type=type.replace("Temperature", "Temperatura");
        location = location.replace("Environment", "Ambiente");

        var colorP = "orange";
        var minValId = "minValueAmb";
        var maxValId = "maxValueAmb";
        var avgValId = "avgValueAmb";
        var chartId = "chart_Temp_Env";
    }
    else if(type=="Temperature" && location=="Station"){
        type=type.replace("Temperature", "Temperatura");
        location = location.replace("Station", "Estación");

        var colorP = "LightSeaGreen";
        var minValId = "minValueTemp";
        var maxValId = "maxValueTemp";
        var avgValId = "avgValueTemp";
        var chartId = "chart_Temp_Sta";
    }
    return [colorP, titleVertical, minValId, maxValId, avgValId, chartId]
}
function changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV){
    $("#"+minValId).text(minV);
    $("#"+maxValId).text(maxV);
    $("#"+avgValId).text(avgV);
}


function displayChartInd(divId, titleVertical, colorL, data, format, contentTool){
    var chartMon = new CanvasJS.Chart(divId, {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320,
        theme: "light2",
        toolTip:{   
			content: contentTool   
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


    chartMon.render();
    dataPoints = [];
    
}

function addDataHours(data){
    data = JSON.parse(data);
    var colorP = getLegends()[0], titleVertical = getLegends()[1];
    var minValId = getLegends()[2], maxValId= getLegends()[3];
    var avgValId= getLegends()[4], chartId= getLegends()[5];
    if(data.length!=0){
        //Boxes min, max, avg
        var minV = 5000; var maxV = 0; var sumV = 0;
        for(points of data){
            var time = parseInt(points['Timestamp']);
            var value = parseInt(points['Value']);
            sumV = sumV + value;
            if(value<minV){
                minV = value;
            }if(value>maxV){
                maxV = value;
            }
            var date = new Date(time*1000);
            var hours = date.getHours()+" horas";
            dataPoints.push({
                x: date,
                y: value,
                hour: hours,
                color: colorP
            });
        }
        var lengthChart = dataPoints.length;
        var avgV = (sumV/lengthChart).toFixed(2);
        if (isNaN(avgV)){
            avgV = 0;
        }
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV);
        
    }else{
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, 0, 0, 0);
        
    }
    displayChartInd(chartId, titleVertical, colorP, dataPoints, "DDD/D HH:mm", "<strong> {hour}</strong>: {y}");

}

function addDataDays(data){
    data = JSON.parse(data);
    var colorP = getLegends()[0], titleVertical = getLegends()[1];
    var minValId = getLegends()[2], maxValId= getLegends()[3];
    var avgValId= getLegends()[4], chartId= getLegends()[5];
    if(data.length!=0){
        //Boxes min, max, avg
        var minV = 5000; var maxV = 0; var sumV = 0;
        for(points of data){
            var time = parseInt(points['Timestamp']);
            var value = parseInt(points['Value']);
            sumV = sumV + value;
            if(value<minV){
                minV = value;
            }if(value>maxV){
                maxV = value;
            }
            var date = new Date(time*1000);
            dataPoints.push({
                x: date,
                y: value,
                color: colorP
            });
        }
        var lengthChart = dataPoints.length;
        var avgV = (sumV/lengthChart).toFixed(2);
        if (isNaN(avgV)){
            avgV = 0;
        }
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV);
        
    }else{
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, 0, 0, 0);
        
    }
    displayChartInd(chartId, titleVertical, colorP, dataPoints, "DD-DDD", "<strong> {x} </strong>:{y}");

}

function addDataOneDay(data){
    var colorP = getLegends()[0], titleVertical = getLegends()[1];
    var minValId = getLegends()[2], maxValId= getLegends()[3];
    var avgValId= getLegends()[4], chartId= getLegends()[5];
    if(data != "null"){
        data = JSON.parse(data);
        
        if(data.length!=0){
            //Boxes min, max, avg
            var minV = 5000; var maxV = 0; var sumV = 0;
            for(points of data){
                var time = parseInt(points['Timestamp']);
                var value = parseInt(points['Value']);
                sumV = sumV + value;
                if(value<minV){
                    minV = value;
                }if(value>maxV){
                    maxV = value;
                }
                var date = new Date(time*1000);
                var hours = date.getHours()+":"+date.getMinutes();
                dataPoints.push({
                    x: date,
                    y: value,
                    hour: hours,
                    color: colorP
                });
            }
            var lengthChart = dataPoints.length;
            var avgV = (sumV/lengthChart).toFixed(2);
            if (isNaN(avgV)){
                avgV = 0;
            }
            changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV);
            
        }else{
            changeValuesMinMaxAvg(minValId, maxValId, avgValId, 0, 0, 0);
        }
    }
    else{
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, 0, 0, 0);
        
    }
    displayChartInd(chartId, titleVertical, colorP, dataPoints, "hh:mm TT", "<strong> {hour}</strong> {y}");

}
//Filter by n days
function filterByTimeStamp(n, startTimeStamp, finishTimeStamp){
    var idSensor = parseInt(dataSensor['id']);
    if(n>0 && n<=2){
        $.get("api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimeStamp+"&EndTimestamp="+finishTimeStamp+"&Filter=Hours&FilterValue=1", addDataHours);
    }else if(n>2 && n<=6){
        $.get("api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimeStamp+"&EndTimestamp="+finishTimeStamp+"&Filter=Hours&FilterValue=4", addDataHours);
    }else if(n>6 && n<=25){
        $.get("api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimeStamp+"&EndTimestamp="+finishTimeStamp+"&Filter=Days&FilterValue=1", addDataDays);
    }else{
        $.get("api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimeStamp+"&EndTimestamp="+finishTimeStamp+"&Filter=Weeks&FilterValue=1", addDataDays);
    }
}

function filterByWeek(){
    var idSensor = parseInt(dataSensor['id']);
    var actual = moment();
    var actualTimestamp = actual.unix();
    var lastTimestamp = actual.clone().subtract(1,'week').unix();
    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+lastTimestamp+"&EndTimestamp="+actualTimestamp+"&Filter=Days&FilterValue=1";
    $.get(query, addDataDays);
}
function filterByHour(){
    var idSensor = parseInt(dataSensor['id']);
    var actual = moment();
    var actualTimestamp = actual.unix();
    var lastTimestamp = actual.clone().subtract(1,'hour').unix();

    /*var currentTime = new Date();//Erase 1530409720*1000*/
    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp?StartTimestamp="+lastTimestamp+"&EndTimestamp="+actualTimestamp;
    $.get(query, addDataOneDay);
}

function filterByTwelveHours(){
    var idSensor = parseInt(dataSensor['id']);
    var actual = moment();
    var actualTimestamp = actual.unix();
    var lastTimestamp = actual.clone().subtract(12,'hours').unix();

    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+lastTimestamp+"&EndTimestamp="+actualTimestamp+"&Filter=Days&FilterValue=1";
    $.get(query, addDataOneDay);
}

function filterByDay(){
    var idSensor = parseInt(dataSensor['id']);
    var actual = moment();
    var actualTimestamp = actual.unix();
    var lastTimestamp = actual.clone().subtract(1,'day').unix();

    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+lastTimestamp+"&EndTimestamp="+actualTimestamp+"&Filter=Hours&FilterValue=1";
    $.get(query, addDataOneDay);
}
function filterByMonth(){
    var idSensor = parseInt(dataSensor['id']);

    var actual = moment();
    var actualTimestamp = actual.unix();
    var lastTimestamp = actual.clone().subtract(1,'month').unix();

    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+lastTimestamp+"&EndTimestamp="+actualTimestamp+"&Filter=Weeks&FilterValue=1";
    $.get(query, addDataOneDay);
}

/*-----SELECT---------*/
function changeFunc(id) {
    var selectBox = document.getElementById(id);
    var selectedValue = selectBox.options[selectBox.selectedIndex].value;
    if(selectedValue=="hora"){
        filterByHour();
    }
    else if(selectedValue=="12horas"){
        filterByTwelveHours();
    }
    else if(selectedValue=="dia"){
        filterByDay();
    }
    else if(selectedValue=="semana"){
        filterByWeek();
    }else{
        filterByMonth();
    }
}

