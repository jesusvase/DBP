
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

function eliminarInventario(inventario) {
    if (confirm('¿Estás seguro de que deseas eliminar este inventario?')) {
        fetch('/Carro/DeleteInventario', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                ID: inventario.ID
            })
        })
            .then(response => response.json())
            .then(data => {
                toastr.success('El inventario ha sido eliminado exitosamente');
                loadInventario();
            })
            .catch(error => {
                console.error(error);
                toastr.error('Se produjo un error al eliminar el inventario');
            });
    }
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

                const accionesCell = document.createElement('td');

                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Eliminar';
                deleteButton.classList.add('btn', 'btn-danger');
                deleteButton.addEventListener('click', function () {
                    eliminarInventario(inventario); // Pasar el objeto inventario completo
                });
                accionesCell.appendChild(deleteButton);

                row.appendChild(accionesCell);

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

function eliminarVentas(venta) {
    if (confirm('¿Estás seguro de que deseas eliminar esta venta?')) {
        fetch('/Carro/DeleteVentas', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                ID: venta.ID
            })
        })
            .then(response => response.json())
            .then(data => {
                toastr.success('La venta ha sido eliminada exitosamente');
                loadVentas();
            })
            .catch(error => {
                console.error(error);
                toastr.error('Se produjo un error al eliminar la venta');
            });
    }
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
                precioVentaCell.textContent = venta.Vehiculo.Precio;
                row.appendChild(precioVentaCell);

                const fechaVentaCell = document.createElement('td');
                const fechaVenta = new Date(parseInt(venta.FechaVenta.substr(6))); // Convertir a milisegundos
                const formattedDate = `${fechaVenta.getDate()}/${fechaVenta.getMonth() + 1}/${fechaVenta.getFullYear()}`;
                const formattedTime = `${fechaVenta.getHours().toString().padStart(2, '0')}:${fechaVenta.getMinutes().toString().padStart(2, '0')}`;
                fechaVentaCell.textContent = `${formattedDate} ${formattedTime}`;
                row.appendChild(fechaVentaCell);

                const cantidadCell = document.createElement('td');
                cantidadCell.textContent = venta.Cantidad;
                row.appendChild(cantidadCell);

                const idUsuarioCell = document.createElement('td');
                idUsuarioCell.textContent = venta.Cliente2.Correo; // Utilizar el campo IdUsuario del cliente
                row.appendChild(idUsuarioCell);

                const accionesCell = document.createElement('td');

                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Eliminar';
                deleteButton.classList.add('btn', 'btn-danger');
                deleteButton.addEventListener('click', function () {
                    eliminarVentas(venta); // Pasar el ID directamente
                });
                accionesCell.appendChild(deleteButton);

                row.appendChild(accionesCell);

                newTbody.appendChild(row);
            });

            // Agregar el nuevo tbody con las filas a la tabla
            tabla.appendChild(newTbody);
        });
}



function loadVentasPorCliente() {
    fetch('/Carro/GetVentasCliente')
        .then(response => response.json())
        .then(data => {
            const tabla = document.getElementById('tablaVentasPorCliente');
            const tbody = tabla.querySelector('tbody');

            // Eliminar filas existentes
            while (tbody.firstChild) {
                tbody.removeChild(tbody.firstChild);
            }

            data.forEach(venta => {
                const row = document.createElement('tr');

                const clienteCell = document.createElement('td');
                clienteCell.textContent = venta.Cliente.Correo;
                row.appendChild(clienteCell);

                const vehiculoCell = document.createElement('td');
                vehiculoCell.textContent = venta.Vehiculo.Nombre;
                row.appendChild(vehiculoCell);

                const fechaVentaCell = document.createElement('td');
                const fechaVenta = new Date(parseInt(venta.FechaVenta.substr(6))); // Convertir a milisegundos
                const formattedDate = `${fechaVenta.getDate()}/${fechaVenta.getMonth() + 1}/${fechaVenta.getFullYear()}`;
                const formattedTime = `${fechaVenta.getHours().toString().padStart(2, '0')}:${fechaVenta.getMinutes().toString().padStart(2, '0')}`;
                fechaVentaCell.textContent = `${formattedDate} ${formattedTime}`;
                row.appendChild(fechaVentaCell);

                const precioVentaCell = document.createElement('td');
                precioVentaCell.textContent = venta.Vehiculo.Precio;
                row.appendChild(precioVentaCell);

                const cantidadCell = document.createElement('td');
                cantidadCell.textContent = venta.Cantidad;
                row.appendChild(cantidadCell);

                tbody.appendChild(row);
            });
        });
}

function eliminarMantenimiento(mantenimiento) {
    if (confirm('¿Estás seguro de que deseas eliminar este mantenimiento?')) {
        fetch('/Carro/DeleteServicioMantenimiento', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                Id: mantenimiento.Id
            })
        })
            .then(response => response.json())
            .then(data => {
                toastr.success('El mantenimiento ha sido eliminado exitosamente');
                loadServiciosMantenimiento();
            })
            .catch(error => {
                console.error(error);
                toastr.error('Se produjo un error al eliminar el mantenimiento');
            });
    }
}



function loadServiciosMantenimiento() {
    fetch('/Carro/GetServicioMantenimiento')
        .then(response => response.json())
        .then(data => {
            const tabla = document.getElementById('tablaServiciosMantenimiento');
            const tbody = tabla.querySelector('tbody');

            // Clonar el elemento tbody
            const newTbody = tbody.cloneNode(false);

            // Reemplazar el tbody existente por el clon
            tbody.parentNode.replaceChild(newTbody, tbody);

            data.forEach(servicio => {
                const row = document.createElement('tr');

                const usuarioCell = document.createElement('td');
                usuarioCell.textContent = servicio.Usuario.Correo;
                row.appendChild(usuarioCell);

                const vehiculoCell = document.createElement('td');
                vehiculoCell.textContent = servicio.Vehiculo.Nombre;
                row.appendChild(vehiculoCell);

                const precioCell = document.createElement('td');
                precioCell.textContent = servicio.Precio;
                row.appendChild(precioCell);

                const descripcionCell = document.createElement('td');
                descripcionCell.textContent = servicio.Descripcion;
                row.appendChild(descripcionCell);

                const tipoMantenimientoCell = document.createElement('td');
                tipoMantenimientoCell.textContent = servicio.TipoMantenimiento.Nombre;
                row.appendChild(tipoMantenimientoCell);

                const accionesCell = document.createElement('td');

                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Eliminar';
                deleteButton.classList.add('btn', 'btn-danger');
                deleteButton.addEventListener('click', function () {
                    eliminarMantenimiento(servicio);
                });
                accionesCell.appendChild(deleteButton);

                row.appendChild(accionesCell);

                newTbody.appendChild(row);
            });

            // Agregar el nuevo tbody con las filas a la tabla
            tabla.appendChild(newTbody);
        });
}




function obtenerVehiculos2() {
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
            $('#VehiculoID2').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

function obtenerTipoMantenimientos() {
    $.ajax({
        url: "/Carro/GetTiposMantenimiento",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<option value="' + item.Id + '">' + item.Nombre + '</option>';
            });
            $('#TipoMantenimiento').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

// Repuestos //

function obtenerTiposRepuesto() {
    $.ajax({
        url: "/Carro/GetTiposRepuesto",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<option value="' + item.Id + '">' + item.Nombre + '</option>';
            });
            $('#TipoRepuesto').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}


function obtenerVehiculos3() {
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
            $('#VehiculoID3').append(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

function eliminarRepuesto(repuesto) {
    if (confirm('¿Estás seguro de que deseas eliminar este repuesto?')) {
        fetch('/Carro/DeleteServicioRespuesto', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                Id: repuesto.Id
            })
        })
            .then(response => response.json())
            .then(data => {
                toastr.success('El repuesto ha sido eliminado exitosamente');
                loadServiciosRepuesto();
            })
            .catch(error => {
                console.error(error);
                toastr.error('Se produjo un error al eliminar el repuesto');
            });
    }
}

function loadServiciosRepuesto() {
    fetch('/Carro/GetServicioRepuesto')
        .then(response => response.json())
        .then(data => {
            const tabla = document.getElementById('tablaServiciosRepuestos');
            const tbody = tabla.querySelector('tbody');

            // Clonar el elemento tbody
            const newTbody = tbody.cloneNode(false);

            // Reemplazar el tbody existente por el clon
            tbody.parentNode.replaceChild(newTbody, tbody);

            data.forEach(servicio => {
                const row = document.createElement('tr');

                const usuarioCell = document.createElement('td');
                usuarioCell.textContent = servicio.Usuario.Correo;
                row.appendChild(usuarioCell);

                const vehiculoCell = document.createElement('td');
                vehiculoCell.textContent = servicio.Vehiculo.Nombre;
                row.appendChild(vehiculoCell);

                const precioCell = document.createElement('td');
                precioCell.textContent = servicio.Precio;
                row.appendChild(precioCell);

                const descripcionCell = document.createElement('td');
                descripcionCell.textContent = servicio.Descripcion;
                row.appendChild(descripcionCell);

                const tipoRepuestoCell = document.createElement('td');
                tipoRepuestoCell.textContent = servicio.TipoRepuesto.Nombre;
                row.appendChild(tipoRepuestoCell);

                const actualizarCell = document.createElement('td');
                const actualizarButton = document.createElement('button');
                actualizarButton.textContent = 'Actualizar';
                actualizarButton.classList.add('btn', 'btn-primary');
                actualizarButton.addEventListener('click', () => mostrarFormularioActualizacion(servicio));
                actualizarCell.appendChild(actualizarButton);
                row.appendChild(actualizarCell);

                const eliminarCell = document.createElement('td');
                const eliminarButton = document.createElement('button');
                eliminarButton.textContent = 'Eliminar';
                eliminarButton.classList.add('btn', 'btn-danger');
                eliminarButton.addEventListener('click', () => eliminarRepuesto(servicio));
                eliminarCell.appendChild(eliminarButton);
                row.appendChild(eliminarCell);

                newTbody.appendChild(row);
            });

            // Agregar el nuevo tbody con las filas a la tabla
            tabla.appendChild(newTbody);
        });
}

var servicioActual;

function mostrarFormularioActualizacion(servicio) {
    // Rellenar el formulario con los datos del servicio de repuesto
    $('#VehiculoID3').val(servicio.IdVehiculo);
    $('#TipoRepuesto').val(servicio.IdTipoRepuesto);
    $('#2descripcion').val(servicio.Descripcion);

    // Mostrar el botón de "Actualizar"
    $('#botonGuardar').hide();
    $('#botonActualizar').show();

    // Llamar a la función de actualización al hacer clic en el botón "Actualizar"
    $('#botonActualizar').off('click').on('click', function (e) {
        e.preventDefault(); // Evitar el comportamiento predeterminado del botón

        // Llamar a la función actualizarRepuesto con el objeto servicio
        actualizarRepuesto(servicio);
    });
    
}

function actualizarRepuesto(servicio) {
    // Obtener los datos del formulario
    var idVehiculo = $('#VehiculoID3').val();
    var idTipoRepuesto = $('#TipoRepuesto').val();
    var descripcion = $('#2descripcion').val();

    // Crear el objeto de datos a enviar al servidor
    var data = {
        Id: servicio.Id,
        IdVehiculo: idVehiculo,
        IdTipoRepuesto: idTipoRepuesto,
        Descripcion: descripcion
    };

    // Enviar la solicitud de actualización al servidor
    $.ajax({
        url: '/Carro/UpdateServicioRepuesto',
        type: 'POST',
        data: JSON.stringify(data), // Convertir los datos a JSON
        contentType: 'application/json', // Establecer el encabezado de tipo de contenido como JSON
        success: function (response) {
            // La actualización fue exitosa
            //alert('El servicio de repuesto se actualizó correctamente.');
            // Aquí puedes redirigir a otra página si es necesario
            // Por ejemplo: window.location.href = '/Carro/Index';
            loadServiciosRepuesto();
            $('#botonActualizar').hide();
        },
        error: function (xhr, status, error) {
            // Hubo un error en la actualización
            alert('Error al actualizar el servicio de repuesto: ' + xhr.responseText);
        }
    });
}





document.addEventListener('DOMContentLoaded', function () {
    obtenerTiposVehiculo();
    loadInventario();
    obtenerVehiculos();
    obtenerTiposVehiculo2();
    obtenerCorreosUsuarios();
    loadVentas();
    loadVentasPorCliente();
    loadServiciosMantenimiento();
    obtenerVehiculos2();
    obtenerTipoMantenimientos();
    obtenerTiposRepuesto();
    obtenerVehiculos3();
    loadServiciosRepuesto();
});
