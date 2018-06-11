dataTempDisp = [];
dataTempAmb = [];
dataHumedad = [];
/*
google.charts.load('current', {'packages':['annotatedtimeline']});
google.charts.setOnLoadCallback(drawChart);
function drawChart() {
    var data = new google.visualization.DataTable();
    data.addColumn('date', 'Date');
    data.addColumn('number', 'Temperatura');
    data.addRows(dataPoints);
    var options = {
        width: 1043
    };
    var chart = new google.visualization.AnnotatedTimeLine(document.getElementById('chart_div'));
    chart.draw(data, options);
}*/
/*
google.charts.load('visualization', '1.0', {'packages':['corechart']});
google.charts.setOnLoadCallback(drawChart);

      function drawChart() {
        var data = google.visualization.arrayToDataTable(dataPoints);

        var options = {
          title: 'Company Performance',
          curveType: 'function',
          legend: { position: 'bottom' }
        };

        var chart = new google.visualization.LineChart(document.getElementById('chart_div'));

        chart.draw(data, options);
      }
    */
/*
function addData(data) {
    for (var i = 0; i < data.length; i++) {
        var time = parseInt(data[i].Timestamp)
        dataPoints.push([
            new Date(time),
            parseInt(data[i].Value)
    ]);
}
    //drawChart();
}
$.getJSON('json/archivo.json', addData);*/

//ORIGINAL

window.onload = function() {
    
    var chartTempDisp = new CanvasJS.Chart("chartContainer2", {
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

    var chartHumedad = new CanvasJS.Chart("chartContainer3", {
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

    var chartTempAmb = new CanvasJS.Chart("chartContainer1", {
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

        var lengthAmb = chartTempAmb.options.data[0].dataPoints.length;
        var lengthDisp = chartTempDisp.options.data[0].dataPoints.length;
        var lengthHum = chartHumedad.options.data[0].dataPoints.length;
        
        chartTempDisp.render();
        chartHumedad.render();
        chartTempAmb.render();
        
        document.getElementById("minValueDisp").innerHTML = "   "+minDisp;
        document.getElementById("maxValueDisp").innerHTML = " "+maxDisp;

        document.getElementById("minValueAmb").innerHTML = "   "+minAmb;
        document.getElementById("maxValueAmb").innerHTML = " "+maxAmb;

        document.getElementById("minValueHum").innerHTML = "   "+minHumedad;
        document.getElementById("maxValueHum").innerHTML = " "+maxHumedad;

        document.getElementById("avgValueDisp").innerHTML = "   "+(avgDisp/lengthDisp).toFixed(2);;
       
        document.getElementById("avgValueAmb").innerHTML = "   "+(avgAmb/lengthAmb).toFixed(2);
        
        document.getElementById("avgValueHum").innerHTML = " "+(avgHumedad/lengthHum).toFixed(2);

        
    }
    $.getJSON('json/archivo.json', addData);
}