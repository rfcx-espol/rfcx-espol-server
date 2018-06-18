dataTempDisp = [];
dataTempAmb = [];
dataHumedad = [];

window.onload = displayMonitor();

function displayMonitor() {
    
    var chartMonTempDisp = new CanvasJS.Chart("chartMonitorAmb", {
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
            dataPoints: dataTempDisp
        }]
    });

    var chartMonHumedad = new CanvasJS.Chart("chartMonitorDisp", {
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
            dataPoints: dataHumedad
        }]
    });

    var chartMonTempAmb = new CanvasJS.Chart("chartMonitorHum", {
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
        console.log("here");
        for (var i = 0; i < data.length; i++) {
            console.log(data[i].Type);
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
                    y: value
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
                    y: value
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
                    y: value
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
    $.getJSON('json/monitorData.json', addData);

    setInterval(displayMonitor, 300000000);

    //displayEachChart();
}