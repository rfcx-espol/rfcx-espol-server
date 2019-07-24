//get Id of Station
var stationId = $("#stationId").text();

var CONFIG = {
    stationId : stationId,
    hoursAgo : 1 , //From how many hours ago, I want retrieve first batch of data
    timeInterval : 10000, //At which rate I want to request for new single data. Milliseconds
    maxDataPointsAllowed : 15, //How many points I want to keep in the chart    
}

/*** place empty CanvasJs charts, given the div IDs ***/

var chartContainers = Array.from(document.querySelectorAll("div.sensores_monitor"));

//create a list of objects, to iterate later
var charts = chartContainers.map(function(chartContainer){
    //get sensor info, this depends on the html response of ~/StationView
    let sensorId = chartContainer.querySelector("div.sensorId").textContent;    
    let sensorType = chartContainer.querySelector("div.sensorType").textContent;
   
    //build url to retrieve initial Data
    let query =  timeStampQuery({
        momentJsObject : moment(),
        hoursAgo : CONFIG.hoursAgo
    })  
    let initialDataUrl = `api/Station/${CONFIG.stationId}/Sensor/${sensorId}/${query}`; 
    //get div id to build a CanvasJs chart
    let canvasJsChart = chartContainer.querySelector("div.canvasJsChart");
    let canvasJsChartDivId = canvasJsChart.getAttribute("id");

    return {
        sensorId : sensorId,
        sensorType : sensorType, 
        canvasJsChart : realTimeChart(canvasJsChartDivId),        
        initialDataUrl : initialDataUrl
    }
})

/*** fill charts with initial data ***/
charts.forEach(function(chart){
    //make request
    //MUST VALIDATE WHEN THERE IS NO VALUE IN THE LAST K HOURS
    $.getJSON(chart.initialDataUrl, function(data){        
        //process response
        let dataPoints = data.map(toDataPoint).reverse();        
        let units = getUnits(data);
        let toolTipContent = formatUnitsToTheToolTip(units);
        let axisYTitle = chart.sensorType; 

        //update the chart
        chart.canvasJsChart.options.data[0].dataPoints=dataPoints;
        chart.canvasJsChart.toolTip.set("content", toolTipContent);
        chart.canvasJsChart.axisY[0].set("title", axisYTitle);

        //render changes
        chart.canvasJsChart.render();
    });
});

/*** set a job to update charts for certain time period, this makes the chart realtime ***/

charts.forEach(function(chart){
    //build url to retrieve last Data    
    let sensorId = chart.sensorId;
    let lastDataUrl = `api/Station/${CONFIG.stationId}/Sensor/${sensorId}/Data/LastData`;

    //set the job
    setInterval(updateChart, CONFIG.timeInterval, chart.canvasJsChart, lastDataUrl, CONFIG.maxDataPointsAllowed);
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
        axisY:{            
            titleFontSize: 18
        },
        data:[{
            xValueType: "dateTime",
            type : "line",
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
           
        //update chart
        canvasJsChart.options.data[0].dataPoints.push(newDataPoint);

        //avoids accumulate points in the chart
        if(canvasJsChart.options.data[0].dataPoints.length > maxDataPointsAllowed ){
            canvasJsChart.options.data[0].dataPoints.shift();
        }

        //render changes
        canvasJsChart.render();
    });
}

function formatUnitsToTheToolTip(units){
    if ( units == "Celcius") {
        return "{y} °C" ; 
    } else if ( units == "Percent") {
        return "{y} %" ;
    } else {
        return "";
    }
}

function getUnits(data){
    if ( data[0] != null ){
        //take units from first element of response and assume all elements have same units    
        return data[0].Units;
    } else {
        return "";
    }
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