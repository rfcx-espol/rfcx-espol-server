sensorsList = [];
dataL=[];

window.onload = getData();

setInterval(displayMonitor, 300000);
//get sensors from one device
function getData(){
    var idDevice = parseInt(document.getElementById("deviceId").innerHTML);
    $.get('api/Device/'+parseInt(idDevice)+'/Sensor/',getSensors);
}
//keep data on sensorsList
function getSensors(data){
    var sensors = JSON.parse(data);
    for( sensor of sensors){
        var sensorsInf = {};
        sensorsInf['id']=parseInt(sensor['Id']);
        sensorsInf['deviceId']=parseInt(sensor['DeviceId']);
        sensorsInf['type']=sensor['Type'];
        sensorsInf['location']=sensor['Location'];
        sensorsList.push(sensorsInf);
    }
    displayMonitor();
}
//Display the monitor
function displayMonitor() {
    for (sensors of sensorsList){
        //Collect data
        var idDevice = sensors['deviceId'];
        var idSensor = sensors['id'];

        var currentTime = new Date();
        var current = Math.round(currentTime.getTime()/1000);
        var lastTwoHours = Math.round(currentTime.setHours(currentTime.getHours() - 2)/1000);
        $.getJSON('api/Device/'+idDevice+'/Sensor/'+idSensor+'/Data/'+lastTwoHours+'/'+current, addData);       
    }

}
//Add data to list dataL
function addData(data) {
    var minValue = 50000;
    var maxValue = 0;
    var sumValue = 0;
    console.log("here");
    console.log(data)
    for(var i = 0; i<data.length; i++){
        var location = data[i].Location;
        var type = data[i].Type;
        var value = parseInt(data[i].Value);
        var timestamp = parseInt(data[i].Timestamp);
        console.log("here "+new Date(timestamp*1000));
        var colorP = "#424084"
        if(type == "Temperature" && location =="Device"){
            var colorP = "#424084"
        }else if(type == "Temperature" && location =="Ambiente"){
            var colorP = "orange"
        }else if(type == "Humedad" && location =="Ambiente"){
            var colorP = "LightSeaGreen"
        }
        sumValue = sumValue + value;
        if(value<minValue){
            minValue = value;
        }if(value>maxValue){
            maxValue = value;
        }
        console.log("here "+new Date(timestamp*1000));
        dataL.push({
            x: new Date(timestamp*1000),
            y: value,
            color: colorP
        });
    }
    var lengthChart = dataL.length;
    var avgValue=(sumValue/lengthChart).toFixed(2);
    createBoxes(type,location, minValue, maxValue, avgValue);
    
}

//Create divs to display charts
function createBoxes(type, location, minValue, maxValue, avgValue){
    if(type=="Humedad"){
        var idMin = "minMonHum";
        var idMax = "maxMonHum";
        var idAvg = "avgMonHum";
        var divIdChart = "chartMonitorHum"
    }else if(location == "Device"){
        var idMin = "minMonDis";
        var idMax = "maxMonDisp";
        var idAvg = "avgMonDisp";
        var divIdChart = "chartMonitorDisp"
    }else{
        var idMin = "minMonAmb";
        var idMax = "maxMonAmb";
        var idAvg = "avgMonAmb";
        var divIdChart = "chartMonitorAmb"
    }
    //Si ya existe no vuelve a crear el div
    if(document.getElementById(divIdChart)==null){
        var div = "<div class='col-sm-12 col-md-12 col-lg-12 sensores_monitor'>"+
                "<h4 class='titulo_sensor'>"+type+" - "+location+"</h4>"+
                "<div id='"+divIdChart+"' style='height: 320px'></div>"+
                "<div id='boxInfoValues'>"+
                "<p class='boxLetters initialMon'><i class='material-icons iconsMinMax'>&#xe15d;</i> Min </p><p class='boxLetters initialValue'  id="+idMin+">"+minValue+"</p>"+
                "<p class='boxLetters middle' ><i class='material-icons iconsMinMax'>&#xe148;</i> Max </p><p class='boxLetters middleValue' id="+idMax+">"+maxValue+"</p>"+
                "<p class='boxLetters last'><i class='fa  iconsAvg'>&#xf10c;</i> Avg</p><p class='boxLetters lastValue' id="+idAvg+">"+avgValue+"</p>"+
                "</div>"+
                "<hr>"+
            "</div>"
        $("#monitor").append(div);
    }
    
    displayChart(divIdChart);
}

//Display individual chart
function displayChart(divId){
    if(divId=="chartMonitorAmb"){
        color = "orange";
        titleVertical = "Temperatura °C";
    }else if(divId=="chartMonitorHum"){
        color = "LightSeaGreen";
        titleVertical = "Humedad %";
    }else if(divId=="chartMonitorDisp"){
        color = "#424084";
        titleVertical = "Temperatura °C";
    }
    var chartMon = new CanvasJS.Chart(divId, {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320, 
        theme: "light2",
        axisX:{      
            valueFormatString: "hh:mm TT" 
        },
        axisY: {
            title: titleVertical,
            titleFontSize: 18
        },
        data: [{
            type: "line",
            lineColor: color,
            dataPoints: dataL
        }]
    });
    chartMon.render();
    dataL=[]
}
//Open tab selected
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