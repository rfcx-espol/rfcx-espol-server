var stationId = parseInt($("#stationId").text());

var dataPoints = [];
var dataSensor = {};

function getDataSensor(idSensor, query){
    $.getJSON('api/Sensor/'+idSensor, function(data){
        dataSensor['location'] = data['Location'];
        dataSensor['type'] = data['Type'];

        $.getJSON('api/Station/'+stationId+'/Sensor/'+idSensor+query, addDataS);
    });
}
function getDates(id){
    var idButton = id.substring(7,id.length);
    var nameStart = "start"+idButton;
    var nameFinish = "finish"+idButton; 
    var nameId = "id"+idButton;
    var start = $("input[name="+nameStart+"]").val();  
    var finish = $("input[name="+nameFinish+"]").val(); 
    var idSensor = parseInt($("#"+nameId).val());     

    var startDate = new Date(start);
    startTimeStamp = Math.round(startDate.getTime()/1000);
    var finishDate = new Date(finish);
    finishTimeStamp = Math.round(finishDate.getTime()/1000);
    var query = '/DataTimestamp?startTimestamp='+startTimeStamp+'&endTimestamp='+finishTimeStamp;
    getDataSensor(idSensor, query)
    
    
}

function addDataS(data){
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
    console.log(chartId);
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
                x: new Date(time*1000),
                y: value,
                color: colorP
            });
        }
        var lengthChart = dataPoints.length;
        var avgV = (sumV/lengthChart).toFixed(2);
        if (isNaN(avgV)){
            avgV = 0;
        }
        $("#"+minValId).text(minV);
        $("#"+maxValId).text(maxV);
        $("#"+avgValId).text(avgV);
        
    }else{
        $("#"+minValId).text(0);
        $("#"+maxValId).text(0);
        $("#"+avgValId).text(0);
        
    }
    console.log($("#"+minValId));
    displayChartInd(chartId, titleVertical, colorP, dataPoints, "DDD DD");

}

function displayChartInd(divId, titleVertical, colorL, data, format){
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
    dataPoints = [];
    
}