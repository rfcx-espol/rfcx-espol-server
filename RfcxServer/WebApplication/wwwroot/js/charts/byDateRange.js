//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();
    date_picker.value = now.subtract(7,'days').format('YYYY-MM-DD');
});
//console.log(date_pickers[0]);

//get Id of Station
var stationId = $("#stationId").text();

/*** get sensors info, this depends on the html response of ~/ByDateRange ***/
var sensorDivs = Array.from(document.querySelectorAll("div.sensor"));

var sensors  = sensorDivs.map(function(sensorDiv){
    
    let sensorId = sensorDiv.querySelector("div.sensorId").textContent;    
    let sensorType = sensorDiv.querySelector("div.sensorType").textContent;
    let startTimestamp = moment(document.querySelector("input.date-picker").value).unix();
    let initialDataUrl = `api/Station/${stationId}/Sensor/${sensorId}/AvgPerDate?StartTimestamp=${startTimestamp}`;
    return {
        sensorId : sensorId,
        sensorType : sensorType,       
        initialDataUrl : initialDataUrl
    }
});

//In this view, we use only one chart
//get div id to build a CanvasJs chart
var canvasJsChart = document.querySelector("div.canvasJsChart");
var canvasJsChartDivId = canvasJsChart.getAttribute("id");

//create chart
var chart = avgPerDateChart(canvasJsChartDivId);

var faked_temp_dataPoints = [
    {x : new Date(2019,6,17), y : 25.09 },    
    {x : new Date(2019,6,18), y : 26.78 },    
    {x : new Date(2019,6,19), y : 35.00 },    
    {x : new Date(2019,6,20), y : 25.68 },    
    {x : new Date(2019,6,21), y : 26.21 },    
    {x : new Date(2019,6,22), y : 28.62 },    
    {x : new Date(2019,6,23), y : 22.34 },    
    {x : new Date(2019,6,24), y : 24.01 },
];

var faked_hum_dataPoints = [
    {x : new Date(2019,6,17) , y : 35.09 },    
    {x : new Date(2019,6,18) , y : 33.78 },    
    {x : new Date(2019,6,19) , y : 40.00 },    
    {x : new Date(2019,6,20) , y : 36.68 },    
    {x : new Date(2019,6,21) , y : 36.21 },    
    {x : new Date(2019,6,22) , y : 39.62 },    
    {x : new Date(2019,6,23) , y : 32.34 },    
    {x : new Date(2019,6,24) , y : 35.01 },
];

sensors.forEach(function(sensor){
    if (sensor.sensorType == "Temperature"){

        //update the chart
        chart.options.data.push({            
            legendMarkerType: "circle",
            toolTipContent: "{y} C°",
            name : sensor.sensorType,
            showInLegend: true,
            xValueType: "dateTime",
            type : "stackedArea",
            dataPoints: faked_temp_dataPoints
        });

        //render changes
        chart.render();

    } else if (sensor.sensorType == "Humidity") {

        //update the chart
        chart.options.data.push({            
            legendMarkerType: "circle",
            toolTipContent: "{y} %",
            name : sensor.sensorType,
            showInLegend: true,
            xValueType: "dateTime",
            type : "stackedArea",
            dataPoints: faked_hum_dataPoints
        });

        //render changes
        chart.render(); 
    } else {
        //update the chart
        chart.options.data.push({            
            legendMarkerType: "circle",
            toolTipContent: "{y}",
            name : sensor.sensorType,
            showInLegend: true,
            xValueType: "dateTime",
            type : "stackedArea",
            dataPoints: faked_hum_dataPoints
        });

        //render changes
        chart.render(); 
    }
     
});

/*
//iterate over sensors to obtain agreggated data
sensors.forEach(function(sensor){
    //make request
    //MUST VALIDATE WHEN THERE IS NO VALUE IN THE LAST K HOURS
    $.getJSON(sensor.initialDataUrl, function(data){
        //process response
        let dataPoints = data.map(function(responseElement){            
            let datePart = responseElement[0];
            let avgPart = responseElement[1];

            let year = datePart.Value[0].Value;            
            let month = datePart.Value[1].Value;            
            let day = datePart.Value[2].Value;
            //format x value
            //let x = new Date(parseInt(timestamp)*1000);                
            let x_= `${year}-${month}-${day}`;
            let x = new Date(x_);

            //format y value
            var y = Number(parseFloat(avgPart.Value).toFixed(2));            
            console.log(`x:${x} , y:${y}`);
            return {
                "x" : x,
                "y" : y 
            }
        })
        .reverse();
        //update the chart
        chart.options.data.push({            
            legendMarkerType: "circle",
            toolTipContent: "{y} $",
            name : sensor.sensorType,
            showInLegend: true,
            xValueType: "dateTime",
            type : "stackedArea",
            dataPoints: dataPoints
        });

        //render changes
        chart.render();   
    });
});
*/

//set behaviour to obtain agreggated data...


/*** Aditional webpage behaviour ***/
setGoToLinkBehaviourInTabs();


/*** Functions for charts logic ***/
function avgPerDateChart(divId){
    return new CanvasJS.Chart(divId, {
        animationEnabled:true,
        height: 320,
        theme: "light2",
        legend: {
            horizontalAlign: "right", // "center" , "right"
            verticalAlign: "top",  // "top" , "bottom"
            cursor: "pointer",
            itemclick: function (e) {
                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                    e.dataSeries.visible = false;
                } else {
                    e.dataSeries.visible = true;
                }
                e.chart.render();
            }
        },
        axisX:{
            valueFormatString: "DD MMM YY" ,
            labelAngle: -50
        },
        axisY:{            
            titleFontSize: 18
        },
        data:[]
    });
}


/*** Functions for additional webpage behaviour ***/

function setGoToLinkBehaviourInTabs(){        
    var tabs=document.querySelectorAll("button.tablinks");
    tabs.forEach(function(tab){
        //when click on tab, go to specific view
        tab.addEventListener("click",function(){
            window.location=tab.getAttribute("url");
        })
    })
}