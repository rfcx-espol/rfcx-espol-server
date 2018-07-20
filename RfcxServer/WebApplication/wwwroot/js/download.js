//$("#hide").click(false);
//Seleccionar audios por medio de checklist para descaragar
$(document).ready(function(){
    //$("#hide").click(false);
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

/*
$(window).on("load", function(){
    //$("#hide").click();    
    $("#hide").click();
})
*/

