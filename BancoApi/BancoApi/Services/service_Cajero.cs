using BancoApi.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace BancoApi.Services
{
    public class service_Cajero
    {
        private readonly string att_connectionString;

        public service_Cajero(IConfiguration config)
        {
            att_connectionString = config.GetConnectionString("OracleDb");
        }

        // ============================================================
        // 1. CREAR CAJERO
        // ============================================================
        public async Task<int> function_crearCajero(model_CajeroCrear req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cajeros.pr_crear_cajero", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.BindByName = true;

            cmd.Parameters.Add("p_id_sucursal", OracleDbType.Int32).Value = req.idSucursal;
            cmd.Parameters.Add("p_id_admin", OracleDbType.Int32).Value = req.idAdministrador;
            cmd.Parameters.Add("p_dinero_inicial", OracleDbType.Decimal).Value = req.dineroInicial;

            var output = new OracleParameter("o_id_cajero", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(output);

            await cmd.ExecuteNonQueryAsync();

            return Convert.ToInt32(output.Value.ToString());
        }

        // ============================================================
        // 2. CAMBIAR ESTADO
        // ============================================================
        public async Task function_cambiarEstado(model_CajeroCambiarEstado req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cajeros.pr_cambiar_estado_cajero", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cajero", OracleDbType.Int32).Value = req.idCajero;
            cmd.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = req.estado;

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // 3. RECARGAR
        // ============================================================
        public async Task function_recargar(model_CajeroRecargar req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cajeros.pr_recargar_cajero", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cajero", OracleDbType.Int32).Value = req.idCajero;
            cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = req.monto;

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // 4. DESCONTAR DINERO
        // ============================================================
        public async Task function_descontar(model_CajeroRecargar req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cajeros.pr_descontar_cajero", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cajero", OracleDbType.Int32).Value = req.idCajero;
            cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = req.monto;

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // 5. LISTAR CAJEROS POR SUCURSAL
        // ============================================================
        public async Task<List<model_CajeroLista>> function_listar(int idSucursal)
        {
            var lista = new List<model_CajeroLista>();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_cajeros.pr_listar_cajeros", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_sucursal", OracleDbType.Int32).Value = idSucursal;
            cmd.Parameters.Add("o_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync();

            var cursor = (OracleRefCursor)cmd.Parameters["o_cursor"].Value;

            using var reader = cursor.GetDataReader();

            while (reader.Read())
            {
                lista.Add(new model_CajeroLista
                {
                    idCajero = Convert.ToInt32(reader["ID_CAJERO"]),
                    dineroDisponible = Convert.ToDecimal(reader["DINERO_DISPONIBLE_CAJERO"]),
                    estado = reader["ESTADO_CAJERO"].ToString(),
                    fechaUltimaRecarga = Convert.ToDateTime(reader["FECHA_ULTIMA_RECARGA_CAJERO"])
                });
            }

            return lista;
        }
    }
}
