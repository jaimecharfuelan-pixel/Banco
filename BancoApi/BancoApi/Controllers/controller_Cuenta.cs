using Microsoft.AspNetCore.Mvc;
using BancoApi.Services;
using BancoApi.Models.models_cuenta;

namespace BancoApi.Controllers
{
    [ApiController]
    [Route("api/controller_Cuenta")]
    public class controller_Cuenta : ControllerBase
    {
        private readonly service_Cuenta att_serviceCuenta;

        public controller_Cuenta(service_Cuenta service)
        {
            att_serviceCuenta = service;
        }

        [HttpPost("service_consultarPorCliente")]
        public async Task<IActionResult> service_consultarPorCliente([FromBody] model_CuentaConsultarPorCliente req)
        {
            var result = await att_serviceCuenta.function_consultarPorCliente(req);
            return Ok(result);
        }

        [HttpPost("service_consultarPorId")]
        public async Task<IActionResult> service_consultarPorId([FromBody] model_CuentaConsultarPorId req)
        {
            var result = await att_serviceCuenta.function_consultarPorId(req);
            return Ok(result ?? new { message = "Cuenta no encontrada" });
        }

        [HttpPut("service_actualizarCuenta")]
        public async Task<IActionResult> service_actualizarCuenta([FromBody] model_CuentaActualizar req)
        {
            await att_serviceCuenta.function_actualizarCuenta(req);
            return Ok(new { message = "Cuenta actualizada correctamente" });
        }

        [HttpPut("service_cambiarSaldo")]
        public async Task<IActionResult> service_cambiarSaldo([FromBody] model_CuentaCambiarSaldo req)
        {
            await att_serviceCuenta.function_cambiarSaldo(req);
            return Ok(new { message = "Saldo actualizado correctamente" });
        }

        [HttpPut("service_cambiarContrasena")]
        public async Task<IActionResult> service_cambiarContrasena([FromBody] model_CuentaCambiarContrasenaCuenta req)
        {
            await att_serviceCuenta.function_cambiarContrasena(req);
            return Ok(new { message = "Contraseña actualizada correctamente" });
        }

        [HttpDelete("service_eliminarCuenta/{idCuenta}")]
        public async Task<IActionResult> service_eliminarCuenta(int idCuenta)
        {
            await att_serviceCuenta.function_eliminarCuenta(idCuenta);
            return Ok(new { message = "Cuenta eliminada (inactivada) correctamente" });
        }
    }
}
