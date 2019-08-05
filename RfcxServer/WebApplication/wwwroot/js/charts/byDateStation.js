$(function(){

//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();    
    document.querySelector("input.date-picker-end").value = now.format('YYYY-MM-DD');
    date_picker.value = now.subtract(29,'days').format('YYYY-MM-DD');    
});

var canvasJsChart = document.querySelector("div.canvasJsChart");
var canvasJsChartDivId = canvasJsChart.getAttribute("id");
var chart = avgPerDateChart(canvasJsChartDivId);

let filterButton = document.querySelector("button.button-filter");

filterButton.addEventListener("click", function(){ 
    let selectedSensorText = document.querySelector("select.form-control").value;
    let sensorType = selectedSensorText.trim().split(" ")[0];
    let sensorLocation = selectedSensorText.trim().split(" ")[1];

    let startDateMoment =  moment(document.querySelector("input.date-picker").value);    
    let endDateMoment =  moment(document.querySelector("input.date-picker-end").value);
    let startTimestamp = startDateMoment.unix();
    let endTimestamp = endDateMoment.unix();

    let validDateRange = isValidDateRange(
        startTimestamp, 
        endTimestamp,
        moment().unix()     
    );
    //console.log(endTimestamp);
    let dataUrl = `api/AvgPerDateStation?SensorType=${sensorType}&SensorLocation=${sensorLocation}&StartTimestamp=${startTimestamp}&EndTimestamp=${endTimestamp}`;
    //console.log(dataUrl);
    let stationsUrl = ` api/Station/`;
    if ( validDateRange ) {
        $.getJSON(stationsUrl,function(stations){
            $.getJSON(dataUrl,function(data){
                if (data.length < 1 ) {                
                    alert("No existen valores en el rango especificado");
                }else {
                    chart.options.data.length = 0; //free array of data on each request
                    data.forEach(function(responseElement){
                        let stationId = responseElement.StationId;                        
                        //console.log(stationId);
                        let station = stations.find( s => s.Id == stationId );
                        let stationName = (station == null) ? `Removed station ${stationId}` : station.Name ;//In case a station was in data but is deleted now
                        //let sensorLocation = station.find( s => s.Id == sensorId ).Location;

                        let rawDataPoints = responseElement.aggregates.map(function(aggregate){
                            let year = aggregate._id.year; 
                            let month =  aggregate._id.month ;
                            let day =  aggregate._id.dayOfMonth;
                            let avg = aggregate.avg;
                            
                            let dateObject = new Date(year, (month - 1), day);
                            let timestamp = dateObject.getTime()/1000;//gives timestamp in seconds
                            //console.log(timestamp);
                            return {
                                Timestamp : timestamp,                
                                Value : avg
                            }; 
                        }).sort(function byTimestamp(a,b){
                            return a.Timestamp - b.Timestamp;                    
                        });
                        //compute basic statistics

                        let rawDataPointsValues = rawDataPoints.map( element => element.Value );
                        let valuesForBasicStatistics = (rawDataPointsValues.length >= 1 ) ? rawDataPointsValues : [-1]; 
                        let basicStatistics = {
                            min : ss.min(valuesForBasicStatistics),
                            max : ss.max(valuesForBasicStatistics),
                            mean : ss.mean(valuesForBasicStatistics)
                        }
                        addDataToBasicStatisticsContainer(basicStatistics);

                        let dpsNullPointsAdded = addNullPoints(rawDataPoints, 86400);
                        let dataPoints = dpsNullPointsAdded.map(function(responseElement){
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
                        });
                        

                        //HERE ADD data to the THE CHART

                        let nameOfChart = `${stationName}`;
                        
                        chart.options.data.push({            
                            legendMarkerType: "line",
                            toolTipContent: "{y}",
                            showInLegend: true,
                            name : nameOfChart,
                            xValueType: "dateTime",
                            type : "line",
                            dataPoints: dataPoints
                        });
                        //render changes
                        chart.render();
            
                    });
                }
            });
        });
    } else {
        alert("Los valores en el rango a filtrar son inválidos. Es probable que esté tratando de filtrar valores que aún no existen.");   
    }
});
filterButton.click();



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

//this is a different method from that which is in the file monitor.js
function addDataToBasicStatisticsContainer(basicStatistics){
    let min = basicStatistics.min != -1 ? formatFloat(basicStatistics.min) : "" ; 
    let max = basicStatistics.mean != -1 ? formatFloat(basicStatistics.max) : "" ; 
    let mean = basicStatistics.max != -1 ? formatFloat(basicStatistics.mean) : "" ;    
    //console.log(basicStatistics.min);
    //console.log(sensorType);
    //console.log(sensorLocation);
    //place values in corresponding section
    $(`div#chartAvgPerDate + .boxInfoValues p#minVal`).text(min);
    $(`div#chartAvgPerDate + .boxInfoValues p#maxVal`).text(max);
    $(`div#chartAvgPerDate + .boxInfoValues p#avgVal`).text(mean);   
}
function formatFloat(value){
    return Number(parseFloat(value).toFixed(2));
}


function isValidDateRange(
    startTimestamp,
    endTimestamp, 
    nowTimestamp
){
    return  !(startTimestamp > nowTimestamp || endTimestamp > nowTimestamp);
}

});