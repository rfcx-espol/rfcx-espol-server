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
                let dataPoints = responseElement.aggregates.map(function(aggregate){
                    let year = aggregate._id.year; 
                    let month =  aggregate._id.month ;
                    let day =  aggregate._id.dayOfMonth;
                    let avg = aggregate.avg;
                    //format x value
                    let x = new Date(year, (month - 1), day);                   
                    //format y value
                    let y = Number(parseFloat(avg).toFixed(2));  
                    //console.log(`x:${x} , y:${y}`);
                    return {
                        "x" : x,
                        "y" : y 
                    }
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

});