//get Id of Station
var stationId = $("#stationId").text();

var CONFIG = {
    stationId : stationId,
    hoursAgo : 1, //From how many hours ago, I want retrieve first batch of data
    timeInterval : 5000, //At which rate I want to request for new single data. Milliseconds
    maxDataPointsAllowed : 15, //How many points I want to keep in the chart when adding new points
}

/*** place empty CanvasJs charts, given the div IDs ***/

var chartContainers = Array.from(document.querySelectorAll("div.sensores_monitor"));

//create a list of objects, to iterate later
var charts = chartContainers.map(function(chartContainer){
    //get sensor info, this depends on the html response of ~/StationView
    let sensorId = chartContainer.querySelector("div.sensorId").textContent;    
    let sensorType = chartContainer.querySelector("div.sensorType").textContent;    
    let sensorLocation = chartContainer.querySelector("div.sensorLocation").textContent;
   
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
        sensorLocation : sensorLocation,
        canvasJsChart : realTimeChart(canvasJsChartDivId),        
        initialDataUrl : initialDataUrl
    }
})

/*** fill charts with initial data ***/
charts.forEach(function(chart){
    //make request    
    $.getJSON(chart.initialDataUrl, function(data){ 
        //default value if empty response        
        let dps = (data.length > 1 ) ? data : [{Timestamp : moment().unix() , Value : null }];
        
        //process response
        let rawDataPoints = dps.map(toRawDataPoint).sort(byTimestamp);
        let dpsNullPointsAdded = addNullPoints(rawDataPoints, (CONFIG.timeInterval/1000));
        let dataPoints = dpsNullPointsAdded.map(toDataPoint);

        //add basic statistics
        let rawDataPointsValues = rawDataPoints.map( element => parseFloat(element.Value) );
        console.log(rawDataPointsValues);
        let valuesForBasicStatistics = (rawDataPointsValues.length >= 1 ) ? rawDataPointsValues : [-1];
        //console.log(valuesForBasicStatistics);
        let basicStatistics = {
            min : ss.min(valuesForBasicStatistics),
            max : ss.max(valuesForBasicStatistics),
            mean : ss.mean(valuesForBasicStatistics)
        }
        //console.log(basicStatistics);
        addDataToBasicStatisticsContainer(chart.sensorType,chart.sensorLocation, basicStatistics);
    
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
    setInterval(
        updateChart, 
        CONFIG.timeInterval, 
        chart.canvasJsChart, 
        lastDataUrl, 
        CONFIG.maxDataPointsAllowed,
        chart.sensorType,
        chart.sensorLocation
    );
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
            markerSize: 5,
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
    let y = (value == null) ? null : Number(parseFloat(value).toFixed(2));
    
    return {
        "x" : x,
        "y" : y
    };
}

function toRawDataPoint(responseElement){
    return {
        Timestamp : responseElement.Timestamp,                
        Value : responseElement.Value
    };
}

//compare function to be used in sort method
function byTimestamp(a,b){
    return a.Timestamp - b.Timestamp;
}

//place datapoints if between two datapoints there was supposed to be a value.
function addNullPoints(dataPoints, timeInterval){
    let dataPointsNullPointsAdded = [];
    for(let i = 0 ; i < (dataPoints.length -1) ; i++){
      dataPointsNullPointsAdded.push(dataPoints[i]);
  
      let nextTimestamp = dataPoints[i+1].Timestamp;
      let currentTimestamp = dataPoints[i].Timestamp;
  
      if ( nextTimestamp > (currentTimestamp + timeInterval)) {
        let nullDataPoint = makeNullDataPoint(nextTimestamp, currentTimestamp);  
        dataPointsNullPointsAdded.push(nullDataPoint);
      }        
    }
    dataPointsNullPointsAdded.push(dataPoints[dataPoints.length - 1]);
    return dataPointsNullPointsAdded;
}

function makeNullDataPoint(nextTimestamp, currentTimestamp){
    let middleTimestamp = Math.floor((nextTimestamp + currentTimestamp)/2);
    let nullDataPoint = {
        Timestamp: middleTimestamp,
        Value: null
    };
    return nullDataPoint;
}

function updateChart( 
    canvasJsChart, 
    lastDataURL, 
    maxDataPointsAllowed,
    sensorType,
    sensorLocation
) {    
    $.getJSON(lastDataURL, function(data) {
        //process response            
        let dataPointsLength = canvasJsChart.options.data[0].dataPoints.length;
        let lastDataPoint = canvasJsChart.options.data[0].dataPoints[dataPointsLength - 1];
        let newDataPoint = toRawDataPoint(data);
                

        let nextTimestamp = newDataPoint.Timestamp;
        let currentTimestamp = lastDataPoint.x.getTime()/1000;
        let timeInterval  = CONFIG.timeInterval/1000;
        if ( nextTimestamp > (currentTimestamp + timeInterval)){         
            let nullDataPoint = makeNullDataPoint(nextTimestamp, currentTimestamp);  
            canvasJsChart.options.data[0].dataPoints.push(toDataPoint(nullDataPoint));
        }
        
        
        //update chart
        canvasJsChart.options.data[0].dataPoints.push(toDataPoint(newDataPoint));

        //avoids accumulate points in the chart
        if(canvasJsChart.options.data[0].dataPoints.length > maxDataPointsAllowed ){
            canvasJsChart.options.data[0].dataPoints.shift();
        }

        //render changes
        canvasJsChart.render();

        //update basic statistics
        let rawDataPointsValues = canvasJsChart.options.data[0].dataPoints.filter( element => element.y != null ).map( element => parseFloat(element.y) );
        console.log(rawDataPointsValues);        
        let valuesForBasicStatistics = (rawDataPointsValues.length >= 1 ) ? rawDataPointsValues : [-1];
        //console.log(valuesForBasicStatistics);
        let basicStatistics = {
            min : ss.min(valuesForBasicStatistics),
            max : ss.max(valuesForBasicStatistics),
            mean : ss.mean(valuesForBasicStatistics)
        }
        //console.log(basicStatistics);
        addDataToBasicStatisticsContainer(
            sensorType,
            sensorLocation, 
            basicStatistics
        );
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

function addDataToBasicStatisticsContainer(sensorType,sensorLocation, basicStatistics){
    let min = ( basicStatistics.min != -1 && !isNaN(basicStatistics.min) ) ? formatFloat(basicStatistics.min) : "" ; 
    let max = ( basicStatistics.max != -1 && !isNaN(basicStatistics.max) ) ? formatFloat(basicStatistics.max) : "" ;
    let mean = ( basicStatistics.mean != -1 && !isNaN(basicStatistics.mean) ) ? formatFloat(basicStatistics.mean) : "" ;    
    //console.log(basicStatistics.min);
    console.log(sensorType);
    console.log(sensorLocation);
    //place values in corresponding section
    $(`div#chartMonitor${sensorType}_${sensorLocation} + .boxInfoValues p#minVal`).text(min);
    $(`div#chartMonitor${sensorType}_${sensorLocation} + .boxInfoValues p#maxVal`).text(max);
    $(`div#chartMonitor${sensorType}_${sensorLocation} + .boxInfoValues p#avgVal`).text(mean);   
}
function formatFloat(value){
    return Number(parseFloat(value).toFixed(2));
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