using Microsoft.AspNetCore.Mvc;
using BancoApi.Services;
using BancoApi.Models;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/sucursal")]
    public class controller_Sucursal : ControllerBase
    {
        private readonly service_Sucursal service;

        public controller_Sucursal(service_Sucursal srv)
        {
            service = srv;
        }

        // ================================================
        // LISTAR
        // ================================================
        [HttpGet("listar")]
        public async Task<IActionResult> listar()
        {
            return Ok(await service.function_listarSucursales());
        }


        // ================================================
        // CREAR
        // ================================================
        [HttpPost("crear")]
        public async Task<IActionResult> crear([FromBody] model_SucursalCrear request)
        {
            try
            {
                int id = await service.function_crearSucursal(request);
                return Ok(new { message = "Sucursal creada", idSucursal = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        // ================================================
        // EDITAR ESTADO
        // ================================================
        [HttpPost("editar-estado")]
        public async Task<IActionResult> editarEstado([FromBody] model_SucursalCambiarEstado req)
        {
            try
            {
                await service.function_editarEstado(req.idSucursal, req.estado);
                return Ok(new { message = "Estado actualizado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        // ================================================
        // ELIMINAR
        // ================================================
        [HttpDelete("eliminar/{idSucursal}")]
        public async Task<IActionResult> eliminar(int idSucursal)
        {
            try
            {
                await service.function_eliminarSucursal(idSucursal);
                return Ok(new { message = "Sucursal eliminada" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
