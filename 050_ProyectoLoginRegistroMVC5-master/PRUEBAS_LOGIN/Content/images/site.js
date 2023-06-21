
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

function loadInventario() {
    fetch('/Carro/GetInventario')
        .then(response => response.json())
        .then(data => {
            const tabla = document.getElementById('tablaInventario');
            const tbody = tabla.querySelector('tbody');

            // Clonar el elemento tbody
            const newTbody = tbody.cloneNode(false);

            // Reemplazar el tbody existente por el clon
            tbody.parentNode.replaceChild(newTbody, tbody);

            data.forEach(inventario => {
                const row = document.createElement('tr');

                const nombreVehiculoCell = document.createElement('td');
                nombreVehiculoCell.textContent = inventario.Vehiculo.Nombre;
                row.appendChild(nombreVehiculoCell);

                const tipoVehiculoCell = document.createElement('td');
                tipoVehiculoCell.textContent = inventario.Vehiculo.TipoVehiculo.Nombre;
                row.appendChild(tipoVehiculoCell);

                const precioCell = document.createElement('td');
                precioCell.textContent = inventario.Vehiculo.Precio; // Agregar el campo Precio
                row.appendChild(precioCell);

                const cantidadDisponibleCell = document.createElement('td');
                cantidadDisponibleCell.textContent = inventario.CantidadDisponible;
                row.appendChild(cantidadDisponibleCell);

                newTbody.appendChild(row);
            });

            // Agregar el nuevo tbody con las filas a la tabla
            tabla.appendChild(newTbody);
        });
}


function obtenerVehiculos() {
    $.ajax({
        url: "/Carro/GetVehiculos",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<option value="' + item.ID + '">' + item.Nombre + '</option>';
            });
            $('#VehiculoID').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

function obtenerTiposVehiculo2() {
    $.ajax({
        url: "/Carro/ObtenerInventarioConCantidadMayorCero",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<option value="' + item.VehiculoID + '">' + item.Vehiculo.Nombre + '</option>';
            });
            $('#TipoVehiculoID').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

function obtenerCorreosUsuarios() {
    $.ajax({
        url: "/Carro/ObtenerCorreoIdUsuario",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<option value="' + item.IdUsuario + '">' + item.Correo + '</option>';
            });
            $('#TipoClienteID').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

function loadVentas() {
    fetch('/Carro/GetVentas')
        .then(response => response.json())
        .then(data => {
            const tabla = document.getElementById('tablaVentas');
            const tbody = tabla.querySelector('tbody');

            // Clonar el elemento tbody
            const newTbody = tbody.cloneNode(false);

            // Reemplazar el tbody existente por el clon
            tbody.parentNode.replaceChild(newTbody, tbody);

            data.forEach(venta => {
                const row = document.createElement('tr');

                const clienteCell = document.createElement('td');
                clienteCell.textContent = venta.Cliente.Correo;
                row.appendChild(clienteCell);

                const vehiculoCell = document.createElement('td');
                vehiculoCell.textContent = venta.Vehiculo.Nombre;
                row.appendChild(vehiculoCell);

                const precioVentaCell = document.createElement('td');
                precioVentaCell.textContent = venta.PrecioVenta;
                row.appendChild(precioVentaCell);

                const fechaVentaCell = document.createElement('td');
                fechaVentaCell.textContent = venta.FechaVenta;
                row.appendChild(fechaVentaCell);

                const cantidadCell = document.createElement('td');
                cantidadCell.textContent = venta.Cantidad;
                row.appendChild(cantidadCell);

                newTbody.appendChild(row);
            });

            // Agregar el nuevo tbody con las filas a la tabla
            tabla.appendChild(newTbody);
        });
}






document.addEventListener('DOMContentLoaded', function () {
    obtenerTiposVehiculo();
    loadInventario();
    obtenerVehiculos();
    obtenerTiposVehiculo2();
    obtenerCorreosUsuarios();
    loadVentas();
});
