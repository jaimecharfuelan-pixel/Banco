using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BancoApi.Models.models_abono;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace BancoApi.Services
{
    public class service_Abono
    {
        private readonly string att_connectionString;

        public service_Abono(IConfiguration configuration)
        {
            att_connectionString = configuration.GetConnectionString("OracleDb");
        }

        // ============================================================
        // REGISTRAR ABONO EXTRAORDINARIO
        // Llama: pkgc_abonos.pr_registrar_abono(
        //   p_id_prestamo, p_monto_abono, p_tipo_abono, p_id_cuenta,
        //   o_id_abono, o_id_transaccion)
        // ============================================================
        public async Task<(int idAbono, int idTransaccion)> function_registrarAbono(
            model_AbonoRegistrar req)
        {
            req.Validar();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_abonos.pr_registrar_abono", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_prestamo", OracleDbType.Int32).Value = req.idPrestamo;
            cmd.Parameters.Add("p_monto_abono", OracleDbType.Decimal).Value = req.montoAbono;
            cmd.Parameters.Add("p_tipo_abono", OracleDbType.Varchar2).Value = req.tipoAbono;
            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;

            var pIdAbono = new OracleParameter("o_id_abono", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            var pIdTransaccion = new OracleParameter("o_id_transaccion", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(pIdAbono);
            cmd.Parameters.Add(pIdTransaccion);

            await cmd.ExecuteNonQueryAsync();

            int idAbono = (pIdAbono.Value == null || pIdAbono.Value == DBNull.Value)
                ? 0
                : Convert.ToInt32(pIdAbono.Value.ToString());

            int idTransaccion = (pIdTransaccion.Value == null || pIdTransaccion.Value == DBNull.Value)
                ? 0
                : Convert.ToInt32(pIdTransaccion.Value.ToString());

            return (idAbono, idTransaccion);
        }

        // ============================================================
        // LISTAR ABONOS DE UN PRÉSTAMO
        // Llama: pkgc_abonos.pr_listar_abonos(
        //   p_id_prestamo, o_cursor OUT SYS_REFCURSOR)
        // ============================================================
        public async Task<List<model_AbonoItem>> function_listarAbonos(
            model_AbonoListar req)
        {
            req.Validar();

            var lista = new List<model_AbonoItem>();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_abonos.pr_listar_abonos", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_prestamo", OracleDbType.Int32).Value = req.idPrestamo;
            cmd.Parameters.Add("o_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync();

            var refCursor = (OracleRefCursor)cmd.Parameters["o_cursor"].Value;
            using var reader = refCursor.GetDataReader();

            while (reader.Read())
            {
                var item = new model_AbonoItem
                {
                    idAbono = Convert.ToInt32(reader["ID_ABONO"]),
                    idPrestamo = Convert.ToInt32(reader["ID_PRESTAMO"]),
                    idTransaccion = reader["ID_TRANSACCION"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(reader["ID_TRANSACCION"]),
                    montoAbono = Convert.ToDecimal(reader["MONTO_ABONO"]),
                    fechaAbono = reader["FECHA_ABONO"] == DBNull.Value
                        ? null
                        : reader["FECHA_ABONO"].ToString(),
                    tipoAbono = reader["TIPO_ABONO"].ToString()
                };

                lista.Add(item);
            }

            return lista;
        }
    }
}
