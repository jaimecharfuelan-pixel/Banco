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
        // Reemplaza el saldo de la cuenta con el valor ingresado
        // ============================================================
        public async Task function_cambiarSaldo(model_CuentaCambiarSaldo req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            // 1. Validar que la cuenta existe
            using (var cmdConsulta = new OracleCommand("pkg_cuenta.consultar_cuentas_por_id", conn))
            {
                cmdConsulta.CommandType = CommandType.StoredProcedure;
                cmdConsulta.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
                cmdConsulta.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using var reader = await cmdConsulta.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    throw new ArgumentException("La cuenta no existe");
                }
            } // El reader y cmdConsulta se cierran aquí

            // 2. Validar que el nuevo saldo no sea negativo
            if (req.monto < 0)
            {
                throw new ArgumentException("El saldo no puede ser negativo");
            }

            // 3. Reemplazar el saldo con el valor ingresado
            using var cmdActualizar = new OracleCommand("pkg_cuenta.actualizar_cuenta", conn);
            cmdActualizar.CommandType = CommandType.StoredProcedure;
            cmdActualizar.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
            cmdActualizar.Parameters.Add("p_saldo", OracleDbType.Decimal).Value = req.monto;
            cmdActualizar.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = DBNull.Value;

            await cmdActualizar.ExecuteNonQueryAsync();
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
