$(function(){

//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();    
    document.querySelector("input.date-picker-end").value = now.format('YYYY-MM-DD');
    date_picker.value = now.subtract(29,'days').format('YYYY-MM-DD');    
});

let filterButton = document.querySelector("button#button-filter");

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

    let dataUrl = `api/avgPerDateStations?SensorType=${sensorType}&SensorLocation=${sensorLocation}&StartTimestamp=${startTimestamp}&EndTimestamp=${endTimestamp}`;
    let dataPromise = $.getJSON(dataUrl);

    if ( !validDateRange ) {
        alert("Los valores en el rango a filtrar son inválidos. Es probable que esté tratando de filtrar valores que aún no existen.");
    } else {
        $.when(dataPromise)
        .done(function(dataResponse) {

            //This chart is unique, and basic statistics involve data from all stations
            let chartDivId = `chartAvgPerDateBetweenStations`;
            let chartDiv = makeChartDiv({
                chartDivId: chartDivId,
                basicStatistics : null
            });
            
            if ( $(`div#_${chartDivId}.card`).length ) {
                $(`div#_${chartDivId}.card`).remove(); 
            }

            $("div.chartsContainer").append(chartDiv);            
                
            //create chart                        
            let axisYTitle = `${sensorType} ${sensorLocation}`;
            let chart = aggregationChart({
                chartDivId : chartDivId,                    
                chartTitle : "",
                axisYTitle : axisYTitle,
                axisXFormat: "DD MMM YY"
            });

            let allValuesForBasicStatistics = [];//gather all values to compute basic statistics
    
        

            if( dataResponse.length < 1 ) {
                alert("No existen valores en el rango especificado");
            }

            dataResponse.forEach(function(data){
                let stationId = data.StationId;
                let stationName = data.StationName;
                let rawDataPointsValues = data.aggregates.filter(element => element.average !=-1 ).map( element => element.average ); 
                allValuesForBasicStatistics = allValuesForBasicStatistics.concat(rawDataPointsValues);

                let dataPoints = data.aggregates.map(function(element){
                    let date =  element.date;
                    let value = element.average;

                    //format x value
                    let m = moment(date, moment.ISO_8601);
                    let x = new Date(m.year(),m.month(),m.date()+1,0,0,0,0);

                    //format y value
                    let y = (value == -1) ? null : formatFloat(value);
                    
                    return {
                        "x" : x,
                        "y" : y
                    };
                });

                let toolTipContent = `{y} ${unitsFromSensorType(sensorType)}`;
                let nameOfData = `${stationName}`;
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
            //after we gathered all data, we run basic statistics
            let basicStatistics = computeBasicStatistics(allValuesForBasicStatistics);
            addDataToBasicStatisticsContainer({
                chartDivId: chartDivId, 
                basicStatistics: basicStatistics
            })

        });
    }  
   
});
filterButton.click();


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

function makeChartDiv({
    chartDivId,
    basicStatistics
}){    
    let min, max, mean;
    if( basicStatistics != null ) {
        min  = formatFloat(basicStatistics.min);
        max  = formatFloat(basicStatistics.max);
        mean = formatFloat(basicStatistics.mean);
    }else {
        min = max = mean = "";
    }
    
    let chartDiv = `
    <div class="card mb-4" id="_${chartDivId}">
        <div class="card-body" >
            <div id="${chartDivId}" style="height: 320px" class="canvasJsChart"></div>
        </div>        
        <div class="card-footer text-center">
            <i class="material-icons iconBasicStatitic">&#xe15d;</i>
            min                
            <p class="basicStatisticValue" id="minVal">${min}</p>
            
            <i class="material-icons iconBasicStatitic">&#xe148;</i>
            max
            <p class="basicStatisticValue" id="maxVal">${max}</p>

            <i class="fa iconBasicStatitic">&#xf10c;</i>
            avg
            <p class="basicStatisticValue" id="avgVal">${mean}</p>                
        </div>
        
    </div>                      
    `;
    return chartDiv;
};


function addDataToBasicStatisticsContainer({
    chartDivId, 
    basicStatistics
}){
    let min, max, mean;
    if( basicStatistics != null ) {
        min  = formatFloat(basicStatistics.min);
        max  = formatFloat(basicStatistics.max);
        mean = formatFloat(basicStatistics.mean);
    } else {
        min = max = mean = "";
    }
        
    
    $(`div.card#_${chartDivId} .card-footer p#minVal`).text(min);
    $(`div.card#_${chartDivId} .card-footer p#maxVal`).text(max);
    $(`div.card#_${chartDivId} .card-footer p#avgVal`).text(mean); 
}

function formatFloat(value){
    return Number(parseFloat(value).toFixed(2));
};

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