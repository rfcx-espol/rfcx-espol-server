//get Id of Station
var stationId = $("#stationId").text();


/*** place empty CanvasJs charts, given the div IDs ***/

var chartContainers = Array.from(document.querySelectorAll("div.sensores_monitor div.chartContainer"));

//create a list of objects, to iterate later
var charts = chartContainers.map(function(chartContainer){
    //get sensor Id
    //this depends on the html response of ~/StationView
    let sensor = chartContainer.nextElementSibling;
    let sensorId = sensor.getAttribute("id");

    //build url to retrieve initial Data
    let query =  timeStampQuery({
        momentJsObject : moment(),
        hoursAgo : 1
    })  
    let initialDataUrl = `api/Station/${stationId}/Sensor/${sensorId}/${query}`;    

    //get div id to build a CanvasJs chart
    let chartContainerDivId = chartContainer.getAttribute("id");

    return {
        sensorId : sensorId,
        canvasJsChart : realTimeChart(chartContainerDivId),
        initialDataUrl : initialDataUrl
    }
})

/*** fill charts with initial data ***/
charts.forEach(function(chart){
    //make request
    $.getJSON(chart.initialDataUrl, function(data){
        //process response       
        let dataPoints = data.map(toDataPoint).reverse();

        //update the chart
        chart.canvasJsChart.options.data[0].dataPoints=dataPoints;

        //render changes
        chart.canvasJsChart.render();
    });
});

/*** set a job to update charts for certain time period, this makes the chart realtime ***/

charts.forEach(function(chart){
    //build url to retrieve last Data    
    let sensorId = chart.sensorId;
    let lastDataUrl = `api/Station/${stationId}/Sensor/${sensorId}/Data/LastData`;

    //set the job
    setInterval(updateChart, 1000, chart.canvasJsChart, lastDataUrl, 15);
})

/*** Aditional webpage behaviour ***/
setGoToLinkBehaviourInTabs();


/*** Functions for charts logic ***/

//create a  CanvasJs chart object, given a divId
function realTimeChart(divId){
    return new CanvasJS.Chart(divId, {
        height: 320,
        theme: "light2",
        axisX:{
            valueFormatString: "hh:mm:ss TT" ,
            labelAngle: -50
        },
        data:[{
            xValueType: "dateTime",
            type : "spline",
            dataPoints: []
        }]
    });
}

//build a timeStampQuery, check https://momentjs.com/docs
function timeStampQuery( { momentJsObject, hoursAgo } ){
    let now = momentJsObject.clone();//I make a clone to avoid modify the original object
    let endTimestamp  = now.unix();//unix() function gives the timestamp
    let startTimestamp = now.subtract(hoursAgo,'hours').unix();
    return `DataTimestamp?startTimestamp=${startTimestamp}&endTimestamp=${endTimestamp}`;
}

function toDataPoint(responseElement){
    let timestamp = responseElement.Timestamp;
    let value = responseElement.Value;

    //format x value
    let x = new Date(parseInt(timestamp)*1000);

    //format y value
    var y = Number(parseFloat(value).toFixed(2));  
    
    return {
        "x" : x,
        "y" : y
    };
}

function updateChart( canvasJsChart, lastDataURL, maxDataPointsAllowed ) {    
    $.getJSON(lastDataURL, function(data) {
        //dataPoint to add
        let dataPoint;

        //process response            
        let newDataPoint = toDataPoint(data);

        //compare new value against last value
        let dataPointsLength = canvasJsChart.options.data[0].dataPoints.length;
        //VALIDATE EMPTY ARRAYS
        let lastDataPoint = canvasJsChart.options.data[0].dataPoints[dataPointsLength - 1];
        let isSamePoint = JSON.stringify(lastDataPoint) === JSON.stringify(newDataPoint);
        
        if (isSamePoint) {         
            //set a null point
            dataPoint = {
                "x" : newDataPoint.x,
                "y" : null
            }
        } else {
            //use the new point
            dataPoint = newDataPoint;
        }       
                
        //update chart
        canvasJsChart.options.data[0].dataPoints.push(dataPoint);

        //avoids accumulate points in the chart
        if(canvasJsChart.options.data[0].dataPoints.length > maxDataPointsAllowed ){
            canvasJsChart.options.data[0].dataPoints.shift();
        }

        //render changes
        canvasJsChart.render();
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