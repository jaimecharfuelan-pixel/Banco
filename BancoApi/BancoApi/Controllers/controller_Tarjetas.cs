using BancoAPI.Models.Tarjetas;
using BancoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BancoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarjetasController : ControllerBase
    {
        private readonly TarjetasService _tarjetasService;

        public TarjetasController(TarjetasService tarjetasService)
        {
            _tarjetasService = tarjetasService;
        }

        [HttpPost("debito")]
        public ActionResult<CrearTarjetaDebitoResponse> CrearTarjetaDebito(
            [FromBody] CrearTarjetaDebitoRequest request)
        {
            var resp = _tarjetasService.CrearTarjetaDebito(request);
            return Ok(resp);
        }

        [HttpPost("credito")]
        public ActionResult<CrearTarjetaCreditoResponse> CrearTarjetaCredito(
            [FromBody] CrearTarjetaCreditoRequest request)
        {
            var resp = _tarjetasService.CrearTarjetaCredito(request);
            return Ok(resp);
        }

        [HttpPut("estado")]
        public IActionResult CambiarEstadoTarjeta(
            [FromBody] CambiarEstadoTarjetaRequest request)
        {
            _tarjetasService.CambiarEstadoTarjeta(request);
            return NoContent();
        }

        [HttpGet("{idCuenta:int}")]
        public ActionResult<List<TarjetaDTO>> ListarTarjetas(int idCuenta)
        {
            var tarjetas = _tarjetasService.ListarTarjetas(idCuenta);
            return Ok(tarjetas);
        }
    }
}
