//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();
    date_picker.value = now.subtract(7,'days').format('YYYY-MM-DD');    
});

//get Id of Station
var stationId = $("#stationId").text();


//In this view, we use only one chart
//get div id to build a CanvasJs chart
var canvasJsChart = document.querySelector("div.canvasJsChart");
var canvasJsChartDivId = canvasJsChart.getAttribute("id");

//create chart
var chart = avgPerDateChart(canvasJsChartDivId);

let filterButton = document.querySelector("button.button-filter");
filterButton.addEventListener("click", function(){ 
    let selectedSensorText = document.querySelector("select.form-control").value;
    let sensorType = selectedSensorText.trim().split(" ")[0];
    let sensorId = parseInt(selectedSensorText.trim().split(" ")[2]);
    let startTimestamp = moment(document.querySelector("input.date-picker").value).unix();
    let dataUrl = `api/Station/${stationId}/Sensor/${sensorId}/AvgPerDate?StartTimestamp=${startTimestamp}`;
    $.getJSON(dataUrl, function(data){
        //process response
        let dataPoints = data.map(function(responseElement){            
            let datePart = responseElement[0];
            let avgPart = responseElement[1];

            let year = datePart.Value[0].Value;            
            let month = datePart.Value[1].Value;            
            let day = datePart.Value[2].Value;
            //format x value
            //let x = new Date(parseInt(timestamp)*1000);                
            let x_= `${year}-${month}-${day}`;
            let x = new Date(x_);

            //format y value
            var y = Number(parseFloat(avgPart.Value).toFixed(2));            
            console.log(`x:${x} , y:${y}`);
            return {
                "x" : x,
                "y" : y 
            }
        })
        .reverse();
        //update the chart
        chart.options.data.shift();
        chart.options.data.push({            
            legendMarkerType: "circle",
            toolTipContent: "{y} $",
            name : sensorType,
            showInLegend: true,
            xValueType: "dateTime",
            type : "stackedArea",
            dataPoints: dataPoints
        });

        //render changes
        chart.render();   
    });
    console.log(dataUrl);
});
filterButton.click();

//set behaviour to obtain agreggated data...

/*** Aditional webpage behaviour ***/
setGoToLinkBehaviourInTabs();


/*** Functions for charts logic ***/
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