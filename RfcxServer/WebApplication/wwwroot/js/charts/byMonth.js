$(function(){

let filterButton = document.querySelector("button#button-filter");

filterButton.addEventListener("click", function(){ 
    let selectedStationId = document.querySelector("select.form-control").value;
    let year = document.querySelector("select.form-control.date-picker").value;
    let startDateMoment =  moment(year,"YYYY");
    let startTimestamp = startDateMoment.unix();
    let endTimestamp = startDateMoment.add(1,"years").unix();
    let validDateRange = true;

    let dataUrl = `api/avgPerMonth?StationId=${selectedStationId}&StartTimestamp=${startTimestamp}&EndTimestamp=${endTimestamp}`;    
    let dataPromise = $.getJSON(dataUrl);

    
    if ( !validDateRange ) {
        alert("Los valores en el rango a filtrar son inválidos. Es probable que esté tratando de filtrar valores que aún no existen.");
    } else {
        $.when(dataPromise)
        .done(function(dataResponse) {

        
            if( dataResponse.length < 1 ) {
                alert("No existen valores en el rango especificado");
            }

            dataResponse.forEach(function(data){
                let sensorId = data.SensorId;                
                let sensorType = data.SensorType;                           
                let sensorLocation = data.SensorLocation;
                let rawDataPointsValues = data.aggregates.filter(element => element.average !=-1 ).map( element => element.average ); 
                let basicStatistics = computeBasicStatistics(rawDataPointsValues);

                let dataPoints = data.aggregates.map(function(element){
                    let month =  element.month;
                    let value = element.average;

                    //format x value
                    let currentDate = new Date();//we dont care about the date just the month
                    let x = currentDate.setMonth(month-1);

                    //format y value
                    let y = (value == -1) ? null : formatFloat(value);
                    
                    return {
                        "x" : x,
                        "y" : y
                    };
                });

                let chartDivId = `chart_${sensorId}`;
                let chartDiv = makeChartDiv(
                    chartDivId,
                    basicStatistics
                );

                if ( $(`div#_${chartDivId}.card`).length ) {
                    $(`div#_${chartDivId}.card`).remove(); 
                }
                $("div.chartsContainer").append(chartDiv); 

                //create chart                        
                let chart = aggregationChart({
                    chartDivId : chartDivId,                    
                    chartTitle : "",
                    axisYTitle : sensorType,
                    axisXFormat: "MMMM"
                });

                let toolTipContent = `{y} ${unitsFromSensorType(sensorType)}`;
                let nameOfData = `${sensorType} ${sensorLocation}`;
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