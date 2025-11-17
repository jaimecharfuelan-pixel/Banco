using BancoApi.Dtos.DTOs_Cuota;
using BancoApi.Models.models_cuota;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuotaController : ControllerBase
    {
        private readonly service_Cuota _service;

        public CuotaController(service_Cuota service)
        {
            _service = service;
        }

        // ============================================================
        // GENERAR CUOTAS
        // ============================================================
        [HttpPost("generar")]
        public async Task<IActionResult> GenerarCuotas([FromBody] DTO_GenerarCuotas dto)
        {
            try
            {
                var model = new model_generarCuotas(dto);
                model.Validar();

                await _service.GenerarCuotas(model);
                return Ok(new { mensaje = "Cuotas generadas correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Loguear el error con un logger real en producción
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ============================================================
        // PAGAR CUOTA
        // ============================================================
        [HttpPost("pagar")]
        public async Task<IActionResult> PagarCuota([FromBody] DTO_PagarCuota dto)
        {
            try
            {
                var model = new model_pagarCuota(dto);
                model.Validar();

                var idTransaccion = await _service.PagarCuota(model);
                return Ok(new { idTransaccion });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ============================================================
        // LISTAR CUOTAS POR PRÉSTAMO
        // ============================================================
        [HttpGet("listar/{idPrestamo}")]
        public async Task<IActionResult> ListarCuotas(long idPrestamo)
        {
            try
            {
                var model = new model_listarCuota(new DTO_ListarCuota { idPrestamo = idPrestamo });
                model.Validar();

                var lista = await _service.ListarCuotas(model);
                return Ok(lista);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
