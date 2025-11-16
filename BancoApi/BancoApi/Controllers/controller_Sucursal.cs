using Microsoft.AspNetCore.Mvc;
using BancoApi.Services;
using BancoApi.Models;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/controller_Sucursal")]
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
        [HttpGet("service_listar")]
        public async Task<IActionResult> service_listar()
        {
            try
            {
                return Ok(await service.function_listarSucursales());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al listar sucursales" });
            }
        }


        // ================================================
        // CREAR
        // ================================================
        [HttpPost("service_crear")]
        public async Task<IActionResult> service_crear([FromBody] model_SucursalCrear request)
        {
            try
            {
                int id = await service.function_crearSucursal(request);
                return Ok(new { message = "Sucursal creada exitosamente", idSucursal = id });
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


        // ================================================
        // EDITAR ESTADO
        // ================================================
        [HttpPut("service_editarEstado")]
        public async Task<IActionResult> service_editarEstado([FromBody] model_SucursalCambiarEstado req)
        {
            try
            {
                await service.function_editarEstado(req.idSucursal, req.estado);
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


        // ================================================
        // ELIMINAR
        // ================================================
        [HttpDelete("service_eliminar/{idSucursal}")]
        public async Task<IActionResult> service_eliminar(int idSucursal)
        {
            try
            {
                await service.function_eliminarSucursal(idSucursal);
                return Ok(new { message = "Sucursal eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
