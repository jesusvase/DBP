
function obtenerTiposVehiculo() {
    $.ajax({
        url: "/Carro/Get",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<option value="' + item.ID + '">' + item.Nombre + '</option>';
            });
            $('#tipoVehiculo').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}





document.addEventListener('DOMContentLoaded', function () {
    obtenerTiposVehiculo();
    
});
