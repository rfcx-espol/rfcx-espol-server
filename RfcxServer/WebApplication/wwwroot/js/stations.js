$(window).on('load',function(){
    var body_maximum_height = 0;
    var bodies = $(".cuerpo").get();
    for(b of bodies) {
        if($(b).height() > body_maximum_height) {
            body_maximum_height = $(b).height();
        }
    }
    for(b of bodies) {
        $(b).height(body_maximum_height);
    }
    getDevicesList();
});

function getDevicesList(){
    $.get('api/Device/', function(data){
        var dataDic = JSON.parse(data);
    });
}