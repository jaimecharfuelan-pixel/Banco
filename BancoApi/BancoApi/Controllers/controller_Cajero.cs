using BancoApi.Models;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/cajero")]
    public class controller_Cajero : ControllerBase
    {
        private readonly service_Cajero servicio;

        public controller_Cajero(service_Cajero svc)
        {
            servicio = svc;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear(model_CajeroCrear req)
        {
            return Ok(await servicio.function_crearCajero(req));
        }

        [HttpPut("cambiar-estado")]
        public async Task<IActionResult> CambiarEstado(model_CajeroCambiarEstado req)
        {
            await servicio.function_cambiarEstado(req);
            return Ok("Estado actualizado");
        }

        [HttpPut("recargar")]
        public async Task<IActionResult> Recargar(model_CajeroRecargar req)
        {
            await servicio.function_recargar(req);
            return Ok("Cajero recargado");
        }

        [HttpPut("descontar")]
        public async Task<IActionResult> Descontar(model_CajeroRecargar req)
        {
            await servicio.function_descontar(req);
            return Ok("Dinero descontado");
        }

        [HttpGet("listar/{idSucursal}")]
        public async Task<IActionResult> Listar(int idSucursal)
        {
            return Ok(await servicio.function_listar(idSucursal));
        }
    }
}
