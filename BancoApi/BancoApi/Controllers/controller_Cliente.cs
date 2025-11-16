using BancoApi.Models.models_cliente;
using BancoApi.Services;
using BancoApi.DTOs;
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
        public async Task<IActionResult> service_solicitarCuenta([FromBody] DTO_ClienteRequest dto)
        {
            try
            {
                // Convertir DTO a Modelo (con lógica)
                var model = new model_ClienteRequest(dto);
                
                // Llamamos al servicio
                var att_clienteId = await att_serviceCliente.function_crearCliente(model);

                return Ok(new
                {
                    message = "Cliente creado exitosamente",
                    id_cliente = att_clienteId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ===============================================================
        // == SECCIÓN 1: ACTUALIZAR DATOS COMPLETOS ======================
        // ===============================================================
        [HttpPut("service_actualizarDatosCliente")]
        public async Task<IActionResult> service_actualizarDatosCliente(
            [FromBody] DTO_ClienteActualizarDatos dto)
        {
            try
            {
                var model = new model_ClienteActualizarDatos(dto);
                await att_serviceCliente.function_actualizarDatos(model);
                return Ok(new { message = "Datos del cliente actualizados correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
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
            [FromBody] DTO_ClienteActualizarCorreo dto)
        {
            try
            {
                var model = new model_ClienteActualizarCorreo(dto);
                await att_serviceCliente.function_actualizarCorreo(model);
                return Ok(new { message = "Correo actualizado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
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
            [FromBody] DTO_ClienteActualizarNombre dto)
        {
            try
            {
                var model = new model_ClienteActualizarNombre(dto);
                await att_serviceCliente.function_actualizarNombre(model);
                return Ok(new { message = "Nombre actualizado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
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
            [FromBody] DTO_ClienteActualizarTelefono dto)
        {
            try
            {
                var model = new model_ClienteActualizarTelefono(dto);
                await att_serviceCliente.function_actualizarTelefono(model);
                return Ok(new { message = "Teléfono actualizado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
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
            [FromBody] DTO_ClienteActualizarCedula dto)
        {
            try
            {
                var model = new model_ClienteActualizarCedula(dto);
                await att_serviceCliente.function_actualizarCedula(model);
                return Ok(new { message = "Cédula actualizada correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



       


    }
}
