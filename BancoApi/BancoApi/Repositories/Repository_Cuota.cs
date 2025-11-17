using BancoApi.Models.model_cuota;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BancoApi.Repositories
{
    public class Repository_Cuota
    {
        private readonly string att_connectionString;
        
        public Repository_Cuota(IConfiguration config)
        {
            att_connectionString = config.GetConnectionString("OracleDb");
        }
        //============================================================
        // 1. GENERAR CUOTA
        //============================================================
        public async Task function_generarCuota(long idPrestamo)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cuotas.pr_generar_cuotas", conn) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
            };
            
            cmd.Parameters.Add("p_id_prestamo", OracleDbType.Int64).Value = idPrestamo;
            
            await cmd.ExecuteNonQueryAsync();
        }

        //============================================================
        // Pagar Cuota
        //============================================================
        public async Task<long> function_pagarCuota(long idCuota, int idCuenta)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cuotas.pr_pagar_cuota", conn)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
            };

            cmd.Parameters.Add("p_id_cuota", OracleDbType.Int64).Value = idCuota;
            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = idCuenta;
            
            var output = new OracleParameter("o_id_transaccion", OracleDbType.Int64)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(output);

            await cmd.ExecuteNonQueryAsync();

            return Convert.ToInt64(output.Value);
        }

        //============================================================
        // Listar Cuotas
        //============================================================
        public async Task<long> function_listarCuotas(long idPrestamo)
        {

            var lista = new List<Cuota>();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cuotas.pr_listar_cuotas", conn)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
            };

            cmd.Parameters.Add("p_id_cuota", OracleDbType.Int64).Value = idCuota;
            cmd.Parameters.Add("o_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync();

            var cursor = (OracleRefCursor)cmd.Parameters["o_cursor"].Value;

            using var reader = cursor.GetDataReader();
            while (reader.Read())
            {
                lista.Add(new Cuota
                {
                    IdCuota = Convert.ToInt64(reader["ID_CUOTA"]),
                    IdPrestamo = Convert.ToInt64(reader["ID_PRESTAMO"]),
                    NumeroCuota = Convert.ToInt32(reader["NUMERO_CUOTA"]),
                    MontoDeCuota = Convert.ToDecimal(reader["MONTO_DE_CUOTA"]),
                    CapitalCuota = Convert.ToDecimal(reader["CAPITAL_CUOTA"]),
                    FechaDeVencimientoCuota = reader["FECHA_DE_VENCIMIENTO_CUOTA"] as DateTime?,
                    FechaDePagoCuota = reader["FECHA_DE_PAGO_CUOTA"] as DateTime?,
                    EstadoCuota = reader["ESTADO_CUOTA"].ToString()
                });
            }

            return lista;
        }
    }

}
