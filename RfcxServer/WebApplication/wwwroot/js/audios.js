function showAlertModal(id) {
    $("input#st_id").val(id);
    $("#alert_modal").modal("show");
}

function closeModal() {
    $("#alert_modal").modal("hide");
}

function deleteStation() {
    var audio_id = $("input#st_id").val();
    var station_id = $("#StationId").find(":selected").val();
    $.ajax({
        url: 'api/Station/' + station_id + '/Audio/' + audio_id,
        type: 'DELETE',
        async: false
    });
    window.location.reload();
}

function downloadAudios() {
    var station_id = $("#StationId").find(":selected").val();
    var audios = $('.check:checkbox:checked').map(function (){
        var currentRow = $(this).closest('tr');
        return currentRow.find("td:eq(1)").text();
    }).get().join(',');
    /*$.ajax({
        url: '/DownloadFile',
        type: 'GET',
        data: {
            'namefile': audios,
            'station': station_id
        },
        error: function (xhr, err) {
            alert("readyState: " + xhr.readyState + "\nstatus: " + xhr.status);
            alert("responseText: " + xhr.responseText)
        },
        success: function (data) {
            console.log(data);
            console.log("Descarga");
        },
        async: true
    });*/
    window.location = 'DownloadFile?namefile=' + audios + '&station=' + station_id;
}