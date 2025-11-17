using BancoApi.Models.models_cuota;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/controller_Cuota")]
    public class controller_Cuota : ControllerBase
    {
        private readonly service_Cuota att_serviceCuota;

        public controller_Cuota(service_Cuota service)
        {
            att_serviceCuota = service;
        }

        // ---------------------------------------------
        // Generar cuotas de un préstamo
        // POST api/controller_Cuota/service_generarCuotas
        // ---------------------------------------------
        [HttpPost("service_generarCuotas")]
        public async Task<IActionResult> service_generarCuotas([FromBody] model_CuotaGenerar req)
        {
            try
            {
                await att_serviceCuota.function_generarCuotas(req);
                return Ok(new { message = "Cuotas generadas correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al generar cuotas." });
            }
        }

        // ---------------------------------------------
        // Pagar una cuota
        // POST api/controller_Cuota/service_pagarCuota
        // ---------------------------------------------
        [HttpPost("service_pagarCuota")]
        public async Task<IActionResult> service_pagarCuota([FromBody] model_CuotaPagar req)
        {
            try
            {
                var idTransaccion = await att_serviceCuota.function_pagarCuota(req);
                return Ok(new
                {
                    message = "Cuota pagada correctamente.",
                    idTransaccion
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al pagar la cuota." });
            }
        }

        // ---------------------------------------------
        // Listar cuotas de un préstamo
        // POST api/controller_Cuota/service_listarCuotas
        // ---------------------------------------------
        [HttpGet("service_listarCuotas/{idPrestamo:int}")]
        public async Task<IActionResult> service_listarCuotas(int idPrestamo)
        {
            try
            {
                var req = new model_CuotaListar { idPrestamo = idPrestamo };
                var lista = await att_serviceCuota.function_listarCuotas(req);
                return Ok(lista);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Error al listar las cuotas." });
            }
        }

    }
}
