using BancoApi.Models;
using BancoApi.Models.models_cliente;   // ← ESTE ES EL BUENO

using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace BancoApi.Controllers
{
    // Indica que esta clase es un controlador de API
    [ApiController]

    // NUEVA RUTA RENOMBRADA:
    // Antes: api/cliente
    // Ahora: api/controller_Cliente
    [Route("api/controller_Cliente")]
    public class controller_Cliente : ControllerBase
    {
        private readonly service_Cliente att_serviceCliente;
        // Atributo (att_) del servicio renombrado

        // Constructor que recibe el servicio
        public controller_Cliente(service_Cliente prm_service)
        {
            att_serviceCliente = prm_service;
        }

        // ================================
        // NUEVO ENDPOINT renombrado
        // ================================
        // Antes: POST api/cliente/solicitar-cuenta
        // Ahora: POST api/controller_Cliente/service_solicitarCuenta
        [HttpPost("service_solicitarCuenta")]
        public async Task<IActionResult> service_solicitarCuenta([FromBody] model_ClienteRequest prm_request)
        {
            try
            {
                // Llamamos al servicio renombrado
                var att_clienteId = await att_serviceCliente.function_crearCliente(prm_request);

                // Respuesta exitosa con el ID del cliente generado
                return Ok(new
                {
                    message = "Account successfully created",
                    id_cliente = att_clienteId
                });
            }
            catch (Exception ex)
            {
                // Si Oracle devuelve error (email duplicado, cédula, etc.)
                return BadRequest(new { error = ex.Message });
            }
        }

        // ===============================================================
        // == SECCIÓN 1: ACTUALIZAR DATOS COMPLETOS ======================
        // ===============================================================
        [HttpPut("service_actualizarDatosCliente")]
        public async Task<IActionResult> service_actualizarDatosCliente(
            [FromBody] model_ClienteActualizarDatos req)
        {
            try
            {
                await att_serviceCliente.function_actualizarDatos(req);
                return Ok(new { message = "Datos del cliente actualizados correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        // ===============================================================
        // == SECCIÓN 2: ACTUALIZAR SOLO EMAIL ===========================
        // ===============================================================
        [HttpPut("service_actualizarCorreo")]
        public async Task<IActionResult> service_actualizarCorreo(
            [FromBody] model_ClienteActualizarCorreo req)
        {
            try
            {
                await att_serviceCliente.function_actualizarCorreo(req);
                return Ok(new { message = "Correo actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        // ===============================================================
        // == SECCIÓN 3: ACTUALIZAR SOLO NOMBRE ==========================
        // ===============================================================
        [HttpPut("service_actualizarNombre")]
        public async Task<IActionResult> service_actualizarNombre(
            [FromBody] model_ClienteActualizarNombre req)
        {
            try
            {
                await att_serviceCliente.function_actualizarNombre(req);
                return Ok(new { message = "Nombre actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        // ===============================================================
        // == SECCIÓN 4: ACTUALIZAR SOLO TELÉFONO ========================
        // ===============================================================
        [HttpPut("service_actualizarTelefono")]
        public async Task<IActionResult> service_actualizarTelefono(
            [FromBody] model_ClienteActualizarTelefono req)
        {
            try
            {
                await att_serviceCliente.function_actualizarTelefono(req);
                return Ok(new { message = "Teléfono actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        // ===============================================================
        // == SECCIÓN 5: ACTUALIZAR CÉDULA ===============================
        // ===============================================================
        [HttpPut("service_actualizarCedula")]
        public async Task<IActionResult> service_actualizarCedula(
            [FromBody] model_ClienteActualizarCedula req)
        {
            try
            {
                await att_serviceCliente.function_actualizarCedula(req);
                return Ok(new { message = "Cédula actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



       


    }
}
