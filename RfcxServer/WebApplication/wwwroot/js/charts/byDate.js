$(function(){

//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();
    date_picker.value = now.subtract(6,'days').format('YYYY-MM-DD');    
});

let filterButton = document.querySelector("button.button-filter");

filterButton.addEventListener("click", function(){ 
    let selectedStationId = document.querySelector("select.form-control").value;
    let startDateMoment =  moment(document.querySelector("input.date-picker").value);
    let startTimestamp = startDateMoment.unix();
    let endTimestamp = makeEndTimestamp(
        document.querySelector("select.range-picker").value,
        startDateMoment
    );
    let validDateRange = isValidDateRange(
        startTimestamp, 
        endTimestamp,
        moment().unix()     
    );
    let dataUrl = `api/Station/${selectedStationId}/AvgPerDate?StartTimestamp=${startTimestamp}&EndTimestamp=${endTimestamp}`;
    let sensorsUrl = ` api/Station/${selectedStationId}/Sensor`;
    let dataPromise = $.getJSON(dataUrl);
    let sensorsPromise = $.getJSON(sensorsUrl);

    if ( !validDateRange ) {
        alert("Los valores en el rango a filtrar son inválidos. Es probable que esté tratando de filtrar valores que aún no existen.");
    } else {
        $.when( sensorsPromise , dataPromise)
        .done(function(sensorsResponse, dataResponse) {
            let sensors = sensorsResponse[0];

            if( dataResponse[0].length < 1 ) {
                alert("No existen valores en el rango especificado");
            }

            sensors.forEach(function(sensor){
                //filter data of sensor
                let sensorData = dataResponse[0].find( data => data.SensorId == sensor.Id );

                //if no value, .find() returns undefined. So, check for safety
                let aggregates =  sensorData != null ? sensorData.aggregates : [];
                
                let rawDataPoints = aggregates.map(function(aggregate){
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
                }).sort(byTimestamp);

                let dpsNullPointsAdded = addNullPoints_(
                    rawDataPoints, 
                    startTimestamp,
                    endTimestamp,
                    86400
                );
                let dataPoints = dpsNullPointsAdded.map(function(responseElement){
                    let timestamp = responseElement.Timestamp;
                    let value = responseElement.Value;

                    //format x value
                    let x = new Date(parseInt(timestamp)*1000);

                    //format y value
                    let y = (value == null) ? null : formatFloat(value);
                    
                    return {
                        "x" : x,
                        "y" : y
                    };
                });

                let rawDataPointsValues = rawDataPoints.map( element => element.Value ); 
                let basicStatistics = computeBasicStatistics(rawDataPointsValues);

                let chartDivId = `chart_${sensor.Id}`;                    
                let chartDiv = makeChartDiv(
                    chartDivId,
                    basicStatistics
                );

                if ( $(`div#_${chartDivId}.panel`).length ) {
                    $(`div#_${chartDivId}.panel`).remove(); 
                }
                $("div#individual").append(chartDiv);            
            
                //create chart                        
                let chart = aggregationChart({
                    chartDivId : chartDivId,                    
                    chartTitle : "",
                    axisYTitle : sensor.Type,
                    axisXFormat: "DD MMM YY"
                });
                
                let toolTipContent = `{y} ${unitsFromSensorType(sensor.Type)}`;
                let nameOfData = `${sensor.Type} ${sensor.Location}`;
                chart.options.data.push({         
                    legendMarkerType: "circle",
                    toolTipContent: toolTipContent,
                    showInLegend: true,
                    name : nameOfData,
                    xValueType: "dateTime",
                    type : "line",                                        
                    markerSize: 5,
                    dataPoints: dataPoints
                });
                //render changes
                chart.render();                                   
            });    
        });
    }  
    /*
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
                    let basicStatisticsContainer = makeBasicStatisticsContainer(basicStatistics);
                    //console.log(basicStatisticsContainer);
                    //console.log(basicStatistics);             
                    //let dpsNullPointsAdded = addNullPoints(rawDataPoints, 86400);
                    let dpsNullPointsAdded = addNullPoints_(
                        rawDataPoints, 
                        startTimestamp,
                        endTimestamp,
                        86400
                    );
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
                        $(`div.historical #${chartDivId}`).parent().remove();
                      //  $(`div.historical #${chartDivId}`).remove();
                        //$(`div.historical #${chartDivId} + .boxInfoValues`).remove();
                        //$(`div.historical #${chartDivId}`).remove();  
                    }
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
                        markersize : 5,          
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

                    //adding basic statistics to charts                
                });
            });
        });
    } else {
        alert("Los valores en el rango a filtrar son inválidos. Es probable que esté tratando de filtrar valores que aún no existen.");   
    }
    */
});
filterButton.click();


/*
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
            labelAngle: -90,
            interval: 86400
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
    
    let timestampRange = endTimestamp - startTimestamp;
    let timestampsLength = Math.floor(timestampRange/timeInterval);
    let timestamps  = Array.from({length: timestampsLength}, (_, i) => (startTimestamp + i*timeInterval) );
    let dataPointsNullPointsAdded = timestamps.map(function(timestamp){
        dpFound = dataPoints.find(dp => dp.Timestamp == timestamp);
        dataPoint = (dpFound == null) ? { Timestamp: timestamp, Value: null } : dpFound ;
        return dataPoint;
    });
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


function isValidDateRange(
    startTimestamp,
    endTimestamp, 
    nowTimestamp
){
    return  !(startTimestamp > nowTimestamp || endTimestamp > nowTimestamp);    
}
*/



function aggregationChart({
    chartDivId,    
    chartTitle,
    axisYTitle,
    axisXFormat
}){
    return new CanvasJS.Chart(chartDivId, {
        animationEnabled:true,
        height: 320,
        theme: "light2",
        title:{
            text : chartTitle
        },
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
            valueFormatString: axisXFormat,
            labelAngle: -90
        },
        axisY:{
            title : axisYTitle,
            titleFontSize: 18
        },
        data:[]
    });
}

//place datapoints if between two datapoints there was supposed to be a value.
//different from the used in realtime chart
function addNullPoints_(
    dataPoints,
    startTimestamp,
    endTimestamp,
    timeInterval
){
    
    if (dataPoints.length > 0 ){
        let timestampRange = endTimestamp - startTimestamp;
        let timestampsLength = Math.floor(timestampRange/timeInterval);
        let timestamps  = Array.from({length: timestampsLength}, (_, i) => (startTimestamp + i*timeInterval) );
        let dataPointsNullPointsAdded = timestamps.map(function(timestamp){
            dpFound = dataPoints.find(dp => dp.Timestamp == timestamp);
            dataPoint = (dpFound == null) ? { Timestamp: timestamp, Value: null } : dpFound ;
            return dataPoint;
        });
        return dataPointsNullPointsAdded;
    }else {
        return [];
    }
};

function computeBasicStatistics(values){

    if (values.length > 0){
        return {
            min : ss.min(values),
            max : ss.max(values),
            mean : ss.mean(values)
        }
    } else {
        return null;
    }
}

function isValidDateRange(
    startTimestamp,
    endTimestamp, 
    nowTimestamp
){
    return  !(startTimestamp > nowTimestamp || endTimestamp > nowTimestamp);
}

function makeChartDiv(
    chartDivId,
    basicStatistics
){    
    let min, max, mean;
    if( basicStatistics != null ) {
        min  = formatFloat(basicStatistics.min);
        max  = formatFloat(basicStatistics.max);
        mean = formatFloat(basicStatistics.mean);
    }else {
        min = max = mean = "";
    }
    
    let chartDiv = `
    <div class="panel panel-default" id="_${chartDivId}">
        <div class="panel-body" >
            <div id="${chartDivId}" style="height: 320px" class="canvasJsChart"></div>
        </div>        
        <div class="panel-footer">
            <i class="material-icons iconBasicStatitic">&#xe15d;</i>
            min                
            <p class="boxLetters" id="minVal">${min}</p>
            
            <i class="material-icons iconBasicStatitic">&#xe148;</i>
            max
            <p class="boxLetters" id="maxVal">${max}</p>

            <i class="fa iconBasicStatitic">&#xf10c;</i>
            avg
            <p class="boxLetters" id="avgVal">${mean}</p>                
        </div>
        
    </div>                      
    `;
    return chartDiv;
};

function formatFloat(value){
    return Number(parseFloat(value).toFixed(2));
};

//compare function to be used in sort method
function byTimestamp(a,b){
    return a.Timestamp - b.Timestamp;
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


function unitsFromSensorType(sensorType){
    let units;
    switch(sensorType){
        case "Temperature":
            units = "C°";
            break;
        case "Humidity":
            units = "%";
            break;
        default :
            units = "";
            break;            
    }
    return units;
}
});