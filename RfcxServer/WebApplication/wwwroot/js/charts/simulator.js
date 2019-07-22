window.onload = function() {
    console.log("Enviando datos...");
    yVal = 15;

    var randomValues = function () {
        var actual = moment();
        var xVal = actual.unix();
        var yValTemp = yVal = Math.random() * 51.00;
        var yValHum = yVal = Math.random() * 21.00;

        var data = 
            {
                "data":
                [
                    {
                        "StationId": "14",
                        "SensorId": "1",
                        "Timestamp": xVal + "",
                        "Type": "Temperature",
                        "Value": yValTemp + "",
                        "Units": "Celcius",
                        "Location": "Environment"
                    },
                    {
                        "StationId": "14",
                        "SensorId": "2",
                        "Timestamp": xVal + "",
                        "Type": "Humidity",
                        "Value": yValHum + "",
                        "Units": "Percent",
                        "Location": "Environment"
                    }
                ]
            };

        $.ajax({
            url: 'api/Data',
            type: 'POST',
            data: JSON.stringify(data),
            dataType: 'json',
            contentType: "application/json",
            success: function(res) {
                console.log("Se enviaron los datos");
            }
        });
    };

    setInterval(randomValues, 90000);
}