using BancoApi.Models.models_cuenta;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace BancoApi.Services
{
    public class service_Cuenta
    {
        private readonly string att_connectionString;

        public service_Cuenta(IConfiguration config)
        {
            att_connectionString = config.GetConnectionString("OracleDb");
        }

        // ============================================================
        // CONSULTAR CUENTAS POR CLIENTE
        // ============================================================
        public async Task<List<object>> function_consultarPorCliente(model_CuentaConsultarPorCliente req)
        {
            var lista = new List<object>();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_cuenta.consultar_cuentas_por_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.idCliente;
            cmd.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new
                {
                    idCuenta = Convert.ToInt32(reader["ID_CUENTA"]),
                    saldo = Convert.ToDecimal(reader["SALDO_CUENTA"]),
                    estado = reader["ESTADO_CUENTA"].ToString(),
                    fecha = reader["FECHA_CREACION_CUENTA"]?.ToString()
                });
            }

            return lista;
        }

        // ============================================================
        // CONSULTAR CUENTA POR ID
        // ============================================================
        public async Task<object?> function_consultarPorId(model_CuentaConsultarPorId req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_cuenta.consultar_cuentas_por_id", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
            cmd.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new
                {
                    idCuenta = Convert.ToInt32(reader["ID_CUENTA"]),
                    idCliente = reader["ID_CLIENTE"].ToString(),
                    saldo = Convert.ToDecimal(reader["SALDO_CUENTA"]),
                    estado = reader["ESTADO_CUENTA"].ToString(),
                    fechaCreacion = reader["FECHA_CREACION_CUENTA"]?.ToString(),
                    fechaUltTrans = reader["FECHA_ULTIMA_TRANSACCION_CUENTA"]?.ToString()
                };
            }

            return null;
        }

        // ============================================================
        // ACTUALIZAR CUENTA
        // ============================================================
        public async Task function_actualizarCuenta(model_CuentaActualizar req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_cuenta.actualizar_cuenta", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
            cmd.Parameters.Add("p_saldo", OracleDbType.Decimal).Value = (object?)req.saldo ?? DBNull.Value;
            cmd.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = (object?)req.estado ?? DBNull.Value;

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // CAMBIAR SALDO
        // ============================================================
        public async Task function_cambiarSaldo(model_CuentaCambiarSaldo req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_cuenta.cambiar_saldo", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
            cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = req.monto;

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // CAMBIAR CONTRASEÑA
        // ============================================================
        public async Task function_cambiarContrasena(model_CuentaCambiarContrasenaCuenta req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_cuenta.cambiar_contrasena", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
            cmd.Parameters.Add("p_nueva_contrasena", OracleDbType.Varchar2).Value = req.nuevaContrasena;

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // ELIMINAR CUENTA
        // ============================================================
        public async Task function_eliminarCuenta(int idCuenta)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_cuenta.eliminar_cuenta", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = idCuenta;

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
