$(function(){

//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();
    date_picker.value = now.subtract(7,'days').format('YYYY-MM-DD');    
});

let filterButton = document.querySelector("button.button-filter");

filterButton.addEventListener("click", function(){ 
    let selectedStationId = document.querySelector("select.form-control").value;
    let startTimestamp = moment(document.querySelector("input.date-picker").value).unix();
    let dataUrl = `api/Station/${selectedStationId}/AvgPerDate?StartTimestamp=${startTimestamp}`;
    let sensorsUrl = ` api/Station/${selectedStationId}/Sensor`;    
    $.getJSON(sensorsUrl,function(sensors){
        $.getJSON(dataUrl,function(data){      
            data.forEach(function(responseElement){
                let sensorId = responseElement.SensorId;                                      
                let sensorType = sensors.find( s => s.Id == sensorId ).Type;                                  
                let sensorLocation = sensors.find( s => s.Id == sensorId ).Location;

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

                let chartDivId = `chart_${sensorId}`;

                //if there was a previous chart remove it
                if ( $(`div.historical #${chartDivId}`).length ) {
                    $(`div.historical #${chartDivId}`).remove();
                } 
                let chartDiv = `
                <div id="${chartDivId}" style="height: 320px" class="canvasJsChart"></div>
                `;
                $("div#individual div.historical").append(chartDiv);        
               
                //create chart
                let chart = avgPerDateChart(chartDivId);            
                let nameOfChart = `${sensorType} ${sensorLocation}`
                chart.options.data.push({            
                    legendMarkerType: "circle",
                    toolTipContent: "{y}",
                    showInLegend: true,
                    name : nameOfChart,
                    xValueType: "dateTime",
                    type : "area",
                    dataPoints: dataPoints
                });
                //render changes
                chart.render();      
            });
        });
    });
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

});