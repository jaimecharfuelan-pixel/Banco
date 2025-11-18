using BancoApi.DTOs;
using BancoApi.DTOs.DTOs_prestamos;
using BancoApi.Services;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoApi.Controllers.Prestamos
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamosService _service;

        public PrestamosController(IPrestamosService service)
        {
            _service = service;
        }

        // POST: api/Prestamos/solicitar
        [HttpPost("solicitar")]
        public async Task<ActionResult<int>> Solicitar([FromBody] DTO_solicitarPrestamo dto)
        {
            var id = await _service.SolicitarPrestamoAsync(dto);
            return Ok(id);
        }

        // POST: api/Prestamos/solicitudes
        // Si tu profe quiere GET, se puede cambiar, pero con body te permite usar el DTO como en el ejemplo de cliente
        [HttpPost("solicitudes")]
        public async Task<ActionResult<IEnumerable<DTO_PrestamoSolicitudItem>>> Listar(
            [FromBody] DTO_PrestamoListarSolicitudes dto)
        {
            var lista = await _service.ListarSolicitudesAsync(dto);
            return Ok(lista);
        }

        // POST: api/Prestamos/aceptar
        [HttpPost("aceptar")]
        public async Task<IActionResult> Aceptar([FromBody] DTO_PrestamoAceptar dto)
        {
            await _service.AceptarPrestamoAsync(dto);
            return NoContent();
        }

        // POST: api/Prestamos/rechazar
        [HttpPost("rechazar")]
        public async Task<IActionResult> Rechazar([FromBody] DTO_PrestamoRechazar dto)
        {
            await _service.RechazarPrestamoAsync(dto);
            return NoContent();
        }

        // POST: api/Prestamos/actualizar-saldo
        [HttpPost("actualizar-saldo")]
        public async Task<IActionResult> ActualizarSaldo([FromBody] DTO_PrestamoActualizarSaldo dto)
        {
            await _service.ActualizarSaldoAsync(dto);
            return NoContent();
        }
    }
}