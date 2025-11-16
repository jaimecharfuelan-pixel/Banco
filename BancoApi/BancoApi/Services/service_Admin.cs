using BancoApi.Models.models_admin;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace BancoApi.Services
{
    public class service_Admin
    {
        private readonly string att_connectionString;

        public service_Admin(IConfiguration config)
        {
            att_connectionString = config.GetConnectionString("OracleDb");
        }

        // =============================================================
        // OBTENER CLIENTES SIN CUENTAS (SOLICITUDES)
        // =============================================================
        public async Task<List<object>> function_obtenerSolicitudes()
        {
            var solicitudes = new List<object>();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.obtener_solicitudes", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync();

            // Leer cursor devuelto
            var refCursor = (OracleRefCursor)cmd.Parameters["result"].Value;
            using var reader = refCursor.GetDataReader();

            while (await reader.ReadAsync())
            {
                solicitudes.Add(new
                {
                    idCliente = reader["ID_CLIENTE"].ToString(),
                    nombre = reader["NOMBRE_CLIENTE"].ToString(),
                    email = reader["EMAIL_CLIENTE"].ToString(),
                    telefono = reader["TELEFONO_CLIENTE"].ToString(),
                    cedula = reader["CEDULA_CLIENTE"].ToString()
                });
            }

            return solicitudes;
        }

        // =============================================================
        // CREAR CUENTA DE UN CLIENTE
        // =============================================================
        public async Task<string> function_crearCuenta(model_CrearCuentaRequest req)
        {
            // Validar inputs
            if (string.IsNullOrWhiteSpace(req.IdCliente))
            {
                throw new ArgumentException("El ID del cliente es requerido");
            }

            if (req.IdAdministrador <= 0)
            {
                throw new ArgumentException("El ID del administrador debe ser mayor a 0");
            }

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_cuenta.crear_cuenta", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.IdCliente;
            cmd.Parameters.Add("p_id_administrador", OracleDbType.Int32).Value = req.IdAdministrador;

            cmd.Parameters.Add("o_id_cuenta", OracleDbType.Decimal).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync();

            var idCuenta = cmd.Parameters["o_id_cuenta"].Value?.ToString();

            if (string.IsNullOrEmpty(idCuenta))
            {
                throw new Exception("No se pudo crear la cuenta. El procedimiento no devolvió un ID.");
            }

            return idCuenta;
        }
    }
}

