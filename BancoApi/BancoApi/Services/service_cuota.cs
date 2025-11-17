using BancoApi.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
usign Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace BancoApi.Services
{
    public class service_Cuota
    {
        private readonly string att_connectionString;

        public service_Cuota(IConfigurationSectionHandler config)
        {
            att_connectionString = config.GetConnectionString("OracleDb");
        }

        //============================================================
        // Generar Cuotas
        //============================================================
        public async Task GenerarCuotas(model_generarCuotas model)
        {
            model.Validar();
            await _repository.GenerarCuotas(model.IdPrestamo);
        }

        // ===========================================================
        // Pagar cuota
        // ===========================================================
        public async Task<long> PagarCuota(model_pagarCuota model)
        {
            model.Validar();
            return await _repository.PagarCuota(model.IdCuota, model.IdCuenta);
        }

        // ===========================================================
        // Listar cuotas
        // ===========================================================
        public async Task<List<Cuota>> ListarCuotas(model_listarCuota model)
        {
            model.Validar();
            return await _repository.ListarCuotas(model.IdPrestamo);
        }
    }
}