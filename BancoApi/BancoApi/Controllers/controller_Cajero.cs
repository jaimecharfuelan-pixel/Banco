using BancoApi.Models;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/controller_Cajero")]
    public class controller_Cajero : ControllerBase
    {
        private readonly service_Cajero servicio;

        public controller_Cajero(service_Cajero svc)
        {
            servicio = svc;
        }

        [HttpPost("service_crear")]
        public async Task<IActionResult> service_crear(model_CajeroCrear req)
        {
            try
            {
                var result = await servicio.function_crearCajero(req);
                return Ok(new { message = "Cajero creado exitosamente", idCajero = result });
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

        [HttpPut("service_cambiarEstado")]
        public async Task<IActionResult> service_cambiarEstado(model_CajeroCambiarEstado req)
        {
            try
            {
                await servicio.function_cambiarEstado(req);
                return Ok(new { message = "Estado actualizado correctamente" });
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

        [HttpPut("service_recargar")]
        public async Task<IActionResult> service_recargar(model_CajeroRecargar req)
        {
            try
            {
                await servicio.function_recargar(req);
                return Ok(new { message = "Cajero recargado exitosamente" });
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

        [HttpPut("service_descontar")]
        public async Task<IActionResult> service_descontar(model_CajeroRecargar req)
        {
            try
            {
                await servicio.function_descontar(req);
                return Ok(new { message = "Dinero descontado exitosamente" });
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

        [HttpGet("service_listar/{idSucursal}")]
        public async Task<IActionResult> service_listar(int idSucursal)
        {
            try
            {
                return Ok(await servicio.function_listar(idSucursal));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al listar cajeros" });
            }
        }
    }
}
