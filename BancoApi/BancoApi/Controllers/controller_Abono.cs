using BancoApi.Models.models_abono;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/controller_Abono")]
    public class controller_Abono : ControllerBase
    {
        private readonly service_Abono att_serviceAbono;

        public controller_Abono(service_Abono service)
        {
            att_serviceAbono = service;
        }

        // ---------------------------------------------
        // Registrar abono extraordinario
        // POST api/controller_Abono/service_registrarAbono
        // ---------------------------------------------
        [HttpPost("service_registrarAbono")]
        public async Task<IActionResult> service_registrarAbono(
            [FromBody] model_AbonoRegistrar req)
        {
            try
            {
                var (idAbono, idTransaccion) = await att_serviceAbono.function_registrarAbono(req);

                return Ok(new
                {
                    message = "Abono registrado correctamente.",
                    idAbono,
                    idTransaccion
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al registrar el abono.",
                    detalle = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ---------------------------------------------
        // Listar abonos de un préstamo
        // GET api/controller_Abono/service_listarAbonos/{idPrestamo}
        // ---------------------------------------------
        [HttpGet("service_listarAbonos/{idPrestamo:int}")]
        public async Task<IActionResult> service_listarAbonos(int idPrestamo)
        {
            try
            {
                var req = new model_AbonoListar { idPrestamo = idPrestamo };
                var lista = await att_serviceAbono.function_listarAbonos(req);

                return Ok(lista);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al listar los abonos.",
                    detalle = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}
