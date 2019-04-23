window.onload = function() {
    console.log("Enviando datos...");
    yVal = 15;

    var randomValues = function () {
        var actual = moment();
        var xVal = actual.unix();
        var yValTemp = yVal = Math.round(5 + Math.random() * (0 - 100));
        var yValHum = yVal = Math.round(5 + Math.random() * (0 - 50));

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

    setInterval(randomValues, 10000);
}