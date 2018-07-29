
//Seleccionar audios por medio de checklist para descaragar
$(document).ready(function(){
    
    $("#hide").click(false);

    $("#selectAll").change(function(){
        var cbs = $("input.check");
        if($("#selectAll").is(":checked")){
            cbs.each(function(){
            $(this).prop("checked", true);
        });
        } else {
            cbs.each(function(){
            $(this).prop("checked", false);
        });
        }
    });

    $("#dl").click(function(){
        var cbs = $("input.check");
        var lista_check = "";
        cbs.each(function(){
        if($(this).is(":checked")){
            lista_check = lista_check + $(this).val() + ",";
        }
        });
        lista_check = lista_check.substring(0, lista_check.length - 1);
        $("#lista").attr("value", lista_check);
        if(lista_check.length != 0){
        $("#form2").submit();        
        } else {
        alert("No ha seleccionado ningún archivo");
        }
    });
    
});

//FIN --- Seleccionar audios por medio de checklist para descaragar


$(window).on("load", function(){
    
    /* BEGIN: Get the audios of the day */

    var start = document.getElementById("start").value; // Get date: name = start
    var end = document.getElementById("end").value; // Get date: name = end

    //Get system date.
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth()+1;
    var yyyy = today.getFullYear();

    if(dd<10) {
        dd = '0'+dd
    } 

    if(mm<10) {
        mm = '0'+mm
    } 
    today = yyyy + '-' + mm + '-' + dd;


    //Condition
    if((start == today) && (end == today)){
      $("#hide").click();    
    }

    /* END: Get the audios of the day */

});

