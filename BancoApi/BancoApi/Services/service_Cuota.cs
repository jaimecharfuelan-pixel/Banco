using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BancoApi.Models.models_cuota;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;


namespace BancoApi.Services
{
    public class service_Cuota
    {
        private readonly string att_connectionString;

        public service_Cuota(IConfiguration configuration)
        {
            att_connectionString = configuration.GetConnectionString("OracleDb");
        }

        // ============================================================
        // GENERAR CUOTAS DE UN PRÉSTAMO
        // Llama: pkgc_cuotas.pr_generar_cuotas(p_id_prestamo)
        // ============================================================
        public async Task function_generarCuotas(model_CuotaGenerar req)
        {
            req.Validar();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cuotas.pr_generar_cuotas", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_prestamo", OracleDbType.Int32).Value = req.idPrestamo;

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // PAGAR UNA CUOTA
        // Llama: pkgc_cuotas.pr_pagar_cuota(
        //          p_id_cuota      IN NUMBER,
        //          p_id_cuenta     IN NUMBER,
        //          o_id_transaccion OUT NUMBER )
        // ============================================================
        public async Task<int> function_pagarCuota(model_CuotaPagar req)
        {
            req.Validar();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cuotas.pr_pagar_cuota", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuota", OracleDbType.Int32).Value = req.idCuota;
            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;

            var outputIdTransaccion = new OracleParameter("o_id_transaccion", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputIdTransaccion);

            await cmd.ExecuteNonQueryAsync();

            if (outputIdTransaccion.Value == null || outputIdTransaccion.Value == DBNull.Value)
                return 0;

            return Convert.ToInt32(outputIdTransaccion.Value.ToString());
        }

        // ============================================================
        // LISTAR CUOTAS DE UN PRÉSTAMO
        // Llama: pkgc_cuotas.pr_listar_cuotas(
        //          p_id_prestamo IN NUMBER,
        //          o_cursor      OUT SYS_REFCURSOR )
        // ============================================================
        public async Task<List<model_CuotaItem>> function_listarCuotas(model_CuotaListar req)
        {
            req.Validar();

            var lista = new List<model_CuotaItem>();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cuotas.pr_listar_cuotas", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_prestamo", OracleDbType.Int32).Value = req.idPrestamo;
            cmd.Parameters.Add("o_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync();

            var refCursor = (OracleRefCursor)cmd.Parameters["o_cursor"].Value;
            using var reader = refCursor.GetDataReader();

            while (reader.Read())
            {
                var item = new model_CuotaItem
                {
                    idCuota = Convert.ToInt32(reader["ID_CUOTA"]),
                    numeroCuota = Convert.ToInt32(reader["NUMERO_CUOTA"]),
                    montoCuota = Convert.ToDecimal(reader["MONTO_DE_CUOTA"]),
                    capitalCuota = Convert.ToDecimal(reader["CAPITAL_CUOTA"]),
                    fechaVencimiento = reader["FECHA_DE_VENCIMIENTO_CUOTA"] == DBNull.Value
                        ? null
                        : reader["FECHA_DE_VENCIMIENTO_CUOTA"].ToString(),
                    fechaPago = reader["FECHA_DE_PAGO_CUOTA"] == DBNull.Value
                        ? null
                        : reader["FECHA_DE_PAGO_CUOTA"].ToString(),
                    estado = reader["ESTADO_CUOTA"].ToString()
                };

                lista.Add(item);
            }

            return lista;
        }
    }
}
