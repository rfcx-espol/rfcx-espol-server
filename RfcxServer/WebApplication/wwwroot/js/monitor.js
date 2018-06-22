sensorsInf = {};
sensorsList = [];
dataTempDisp = [];
dataTempAmb = [];
dataHumedad = [];

window.onload = getData();
//get sensors from one device
function getData(){
    var idDevice = parseInt(document.getElementById("deviceId").innerHTML);
    $.get('api/Device/'+parseInt(idDevice)+'/Sensor/',getSensors);
}
//keep data on sensorsList
function getSensors(data){
    var sensors = JSON.parse(data);
    for( sensor of sensors){
        sensorsInf['id']=sensor['Id'];
        sensorsInf['deviceId']=sensor['DeviceId'];
        sensorsInf['type']=sensor['Type'];
        sensorsInf['location']=sensor['Location'];
        sensorsList.push(sensorsInf);
    }
    displayMonitor();
}

//Obtener los datos de un sensor en un timepo
//$.get('api/Device/DeviceId/Sensor/SensorId/Data/StartTimestamp/EndTimestamp')
function displayMonitor() {
    console.log(sensorsList);
    var chartMonTempDisp = new CanvasJS.Chart("chartMonitorDisp", {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320, 
        theme: "light2",
        axisY: {
            title: "Temperatura °C",
            titleFontSize: 18
        },
        data: [{
            type: "line",
            lineColor:"#424084",
            dataPoints: dataTempDisp
        }]
    });

    var chartMonHumedad = new CanvasJS.Chart("chartMonitorHum", {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320, 
        theme: "light2",
        axisY: {
            title: "humedad H",
            titleFontSize: 18
        },
        data: [{
            type: "line",
            lineColor: "LightSeaGreen",
            dataPoints: dataHumedad
        }]
    });

    var chartMonTempAmb = new CanvasJS.Chart("chartMonitorAmb", {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320, 
        theme: "light2",
        axisY: {
            title: "Temperatura °C",
            titleFontSize: 18
        },
        data: [{
            type: "line",
            lineColor: "orange",
            dataPoints: dataTempAmb
        }]
    });
    
    function addData(data) {
        var minDisp= 50000;
        var maxDisp=0;
        var avgDisp=0;
        var minHumedad= 50000;
        var maxHumedad=0;
        var avgHumedad=0;
        var minAmb= 50000;
        var maxAmb=0;
        var avgAmb=0;
        for (var i = 0; i < data.length; i++) {
            if(data[i].Type=="Temperature" && data[i].Location=="Dispositivo"){
                var time = parseInt(data[i].Timestamp);
                var value = parseInt(data[i].Value);
                avgDisp = avgDisp + value;
                if(value<minDisp){
                    minDisp = value;
                }if(value>maxDisp){
                    maxDisp = value;
                }
                dataTempDisp.push({
                    x: new Date(time),
                    y: value,
                    color: "#424084"
                });
            }
            else if(data[i].Type=="Humedad"){
                var time = parseInt(data[i].Timestamp);
                var value = parseInt(data[i].Value);
                avgHumedad = avgHumedad + value;
                if(value<minHumedad){
                    minHumedad = value;
                }if(value>maxHumedad){
                    maxHumedad = value;
                }
                dataHumedad.push({
                    x: new Date(time),
                    y: value,
                    color: "LightSeaGreen"
                });
            }else{
                var time = parseInt(data[i].Timestamp);
                var value = parseInt(data[i].Value);
                avgAmb = avgAmb + value;
                if(value<minAmb){
                    minAmb = value;
                }if(value>maxAmb){
                    maxAmb = value;
                }
                dataTempAmb.push({
                    x: new Date(time),
                    y: value,
                    color: "orange"
                });
            }
        }
        
        var lengthAmb = chartMonTempAmb.options.data[0].dataPoints.length;
        var lengthDisp = chartMonTempDisp.options.data[0].dataPoints.length;
        var lengthHum = chartMonHumedad.options.data[0].dataPoints.length;
        
        chartMonTempDisp.render();
        chartMonHumedad.render();
        chartMonTempAmb.render();
        
        document.getElementById("minMonDisp").innerHTML = "   "+minDisp;
        document.getElementById("maxMonDisp").innerHTML = " "+maxDisp;

        document.getElementById("minMonAmb").innerHTML = "   "+minAmb;
        document.getElementById("maxMonAmb").innerHTML = " "+maxAmb;

        document.getElementById("minMonHum").innerHTML = "   "+minHumedad;
        document.getElementById("maxMonHum").innerHTML = " "+maxHumedad;

        document.getElementById("avgMonDisp").innerHTML = "   "+(avgDisp/lengthDisp).toFixed(2);;
       
        document.getElementById("avgMonAmb").innerHTML = "   "+(avgAmb/lengthAmb).toFixed(2);
        
        document.getElementById("avgMonHum").innerHTML = " "+(avgHumedad/lengthHum).toFixed(2);        
    }

    //$.get('api/Device/2/Sensor/2/Data/1528900018/1528900263', addData);
    $.getJSON('json/monitorData.json', addData);

    setInterval(displayMonitor, 300000000);

    //displayEachChart();
}

function openDevice(evt, cityName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";
}

// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();