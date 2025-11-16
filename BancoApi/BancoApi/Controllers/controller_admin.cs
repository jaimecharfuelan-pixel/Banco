using Microsoft.AspNetCore.Mvc;
using BancoApi.Services;
using BancoApi.Models.models_admin;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/controller_Admin")]
    public class controller_admin : ControllerBase
    {
        private readonly service_Admin att_serviceAdmin;

        public controller_admin(service_Admin service)
        {
            att_serviceAdmin = service;
        }

        // =============================================================
        // OBTENER CLIENTES SIN CUENTAS (SOLICITUDES)
        // =============================================================
        [HttpGet("service_solicitudes")]
        public async Task<IActionResult> service_solicitudes()
        {
            try
            {
                var solicitudes = await att_serviceAdmin.function_obtenerSolicitudes();
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener solicitudes" });
            }
        }

        // =============================================================
        // CREAR CUENTA DE UN CLIENTE
        // =============================================================
        [HttpPost("service_crearCuenta")]
        public async Task<IActionResult> service_crearCuenta([FromBody] model_CrearCuentaRequest req)
        {
            try
            {
                var idCuenta = await att_serviceAdmin.function_crearCuenta(req);
                return Ok(new { idCuenta, message = "Cuenta creada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al crear la cuenta" });
            }
        }
    }
}
