$(function(){

//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();    
    //document.querySelector("input.date-picker-end").value = now.format('YYYY-MM-DD');
    //date_picker.value = now.subtract(29,'days').format('YYYY-MM-DD');        
});

let filterButton = document.querySelector("button.button-filter");

filterButton.addEventListener("click", function(){ 
    let selectedStationId = document.querySelector("select.form-control").value;
    let year = document.querySelector("select.form-control.date-picker").value;
    let startDateMoment =  moment(year,"YYYY");
    let startTimestamp = startDateMoment.unix();
    console.log(startTimestamp);
    let endTimestamp = startDateMoment.add(1,"years").unix();
    let validDateRange = true;
    /*isValidDateRange(
        startTimestamp,
        endTimestamp,
        moment().unix()     
    );¨*/
    //console.log(endTimestamp);
    //let endTimestamp = moment(document.querySelector("select.range-picker").value).unix();    
    //let dataUrl = `api/Station/${selectedStationId}/AvgPerDate?StartTimestamp=${startTimestamp}&EndTimestamp=${endTimestamp}`;
    let dataUrl = `api/Station/${selectedStationId}/AvgPerMonth?StartTimestamp=${startTimestamp}&EndTimestamp=${endTimestamp}`;
    console.log(dataUrl);
    let sensorsUrl = ` api/Station/${selectedStationId}/Sensor`;
    if ( validDateRange ) {
        $.getJSON(sensorsUrl,function(sensors){
            $.getJSON(dataUrl,function(data){
                if (data.length < 1 ) {                
                    alert("No existen valores en el rango especificado");
                }
                data.forEach(function(responseElement){
                    let sensorId = responseElement.SensorId;                                      
                    let sensorType = sensors.find( s => s.Id == sensorId ).Type;                                  
                    let sensorLocation = sensors.find( s => s.Id == sensorId ).Location;

                    let rawDataPoints = responseElement.aggregates.map(function(aggregate){
                        let month = aggregate._id.month;
                        let avg = aggregate.avg;
                        
                        let dateObject = new Date(year, (month - 1), 1);//we just care about month not the date by itself.

                        let timestamp = dateObject.getTime()/1000;//gives timestamp in seconds
                        //console.log(timestamp);

                        return {
                            Timestamp : timestamp,
                            Value : avg
                        }; 
                    }).sort(function (a,b){
                        return a.Timestamp - b.Timestamp;                    
                    });
                    console.log(rawDataPoints);
                    //compute basic statistics

                    let rawDataPointsValues = rawDataPoints.map( element => element.Value );
                    let valuesForBasicStatistics = (rawDataPointsValues.length >= 1 ) ? rawDataPointsValues : [-1]; 
                    let basicStatistics = {
                        min : ss.min(valuesForBasicStatistics),
                        max : ss.max(valuesForBasicStatistics),
                        mean : ss.mean(valuesForBasicStatistics)
                    }
                    let basicStatisticsContainer = makeBasicStatisticsContainer(basicStatistics);
                    //console.log(basicStatisticsContainer);
                    //console.log(basicStatistics);             
                    //let dpsNullPointsAdded = addNullPoints(rawDataPoints, 3600);
                    let dpsNullPointsAdded = addNullPoints_(
                        rawDataPoints,
                        startTimestamp,
                        endTimestamp,
                        ( 86400 * 28 )
                    );
                    console.log(dpsNullPointsAdded);
                    let dataPoints = dpsNullPointsAdded.map(function(responseElement){
                        let timestamp = responseElement.Timestamp;
                        let value = responseElement.Value;

                        
                        //format x value
                        let x = new Date(timestamp*1000);//we just care about month not the date by itself.                                          
                        
                        //console.log(x.toUTC());
                        //format y value
                        let y = (value == null) ? null : Number(parseFloat(value).toFixed(2));
                        
                        return {
                            "x" : x,
                            "y" : y
                        };
                    });
                    console.log(dataPoints);
                    let chartDivId = `chart_${sensorId}`;

                    //if there was a previous chart remove it
                    /*
                    if ( $(`div.historical #${chartDivId}`).length ) {                                                      
                        $(`div.historical #${chartDivId} + .boxInfoValues`).remove();
                        $(`div.historical #${chartDivId}`).remove();  
                    }*/
                    let chartDiv = `
                    <div class="col-sm-12 col-md-12 col-lg-12 historical">
                    <div id="${chartDivId}" style="height: 320px" class="canvasJsChart"></div>
                    ${basicStatisticsContainer}
                    </div>                      
                    `;
                    $("div#individual").append(chartDiv);        
                
                    //create chart
                    let chart = avgPerDateChart(chartDivId);            
                    let nameOfChart = `${sensorType} ${sensorLocation}`
                    chart.options.data.push({            
                        legendMarkerType: "circle",
                        toolTipContent: "{y}",
                        showInLegend: true,
                        name : nameOfChart,
                        xValueType: "dateTime",
                        type : "line",
                        dataPoints: dataPoints
                    });
                    //render changes
                    chart.render();      

                    //adding basic statistics to charts                
                });
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
            valueFormatString: "MMMM" ,
            labelAngle: -90
        },
        axisY:{            
            titleFontSize: 18
        },
        data:[]
    });
}

//place datapoints if between two datapoints there was supposed to be a value.
//differente from the used in realtime chart
function addNullPoints_(
    dataPoints,
    startTimestamp,
    endTimestamp,
    timeInterval
){  
   
    let startDateMoment = moment(moment.unix(startTimestamp));
    let timestamps = [
        startDateMoment.unix(),
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),        
        startDateMoment.add(1,"months").unix(),
        startDateMoment.add(1,"months").unix(),
        startDateMoment.add(1,"months").unix(),
    ];

    //let timestampRange = endTimestamp - startTimestamp;
    //let timestampsLength = Math.floor(timestampRange/timeInterval);
    //let timestamps  = Array.from({length: 12}, (_, i) => startDateMoment.add(i*1,"months").unix() );
    console.log(timestamps);
    let dataPointsNullPointsAdded = timestamps.map(function(timestamp){
        dpFound = dataPoints.find(dp => ( dp.Timestamp == timestamp ));
        dataPoint = (dpFound == null) ? { Timestamp: timestamp, Value: null } : dpFound ;
        return {
            Timestamp: timestamp,
            Value : dataPoint.Value
        };
    });    
    /*
    let dataPointsNullPointsAdded = [];
    for(let i = 0 ; i < (dataPoints.length -1) ; i++){
      dataPointsNullPointsAdded.push(dataPoints[i]);
  
      let nextTimestamp = dataPoints[i+1].Timestamp;
      let currentTimestamp = dataPoints[i].Timestamp;
  
      if ( nextTimestamp > (currentTimestamp + timeInterval)) {
        let nullDataPoint = makeNullDataPoint_(currentTimestamp, timeInterval);  
        dataPointsNullPointsAdded.push(nullDataPoint);
      }        
    }
    dataPointsNullPointsAdded.push(dataPoints[dataPoints.length - 1]);
    */
    return dataPointsNullPointsAdded;
}

//differente from the used in realtime chart
function makeNullDataPoint_(currentTimestamp, timeInterval){
    let newTimestamp = currentTimestamp + timeInterval;
    let nullDataPoint = {
        Timestamp: newTimestamp,
        Value: null
    };
    return nullDataPoint;
}


function makeBasicStatisticsContainer({min, max, mean}){

    let container = `
    <div class="boxInfoValues">
        <p class="boxLetters  initialMon">
            <i class="material-icons iconsMinMax">&#xe15d;</i>
            Min
        </p>
        <p class="boxLetters initialValue" id="minVal">${formatFloats(min)}</p>
        <p class="boxLetters middle">
            <i class="material-icons iconsMinMax">&#xe148;</i>
            Max 
        </p>
        <p class="boxLetters middleValue" id="maxVal">${formatFloats(max)}</p>
        <p class="boxLetters last">
            <i class="fa iconsAvg">&#xf10c;</i> 
            Avg
        </p>
        <p class="boxLetters lastValue" id="avgVal" >${formatFloats(mean)}</p>
    </div>
    `;
    return container;
}
function formatFloats(value){
    return Number(parseFloat(value).toFixed(2));
}

function makeEndTimestamp(rangePickerValue, startDateMoment){
    let start = startDateMoment.clone();
    let end;
    switch (rangePickerValue) {
        case "Semana":
            end = start.add(6,"days");
            break;
        case "Mes":
            end = start.add(29,"days");         
            break; 
        case "Año":
            end = start.add(364,"days");
            break;
    }
    let endTimestamp = end.unix();    
    return endTimestamp;
}


function isValidDateRange(
    startTimestamp,
    endTimestamp, 
    nowTimestamp
){
    return  !(startTimestamp > nowTimestamp || endTimestamp > nowTimestamp);
}

});