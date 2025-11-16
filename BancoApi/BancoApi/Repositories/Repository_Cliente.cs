using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BancoApi.Repositories
{
    // Repositorio: Solo acceso a base de datos, sin lógica de negocio
    public class Repository_Cliente
    {
        private readonly string _connectionString;

        public Repository_Cliente(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("OracleDb");
        }

        public async Task<string> CrearCliente(
            string nombre,
            string email,
            string telefono,
            int cedula,
            string estado)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.crear_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2).Value = nombre;
            cmd.Parameters.Add("p_email_cliente", OracleDbType.Varchar2).Value = email;
            cmd.Parameters.Add("p_telefono_cliente", OracleDbType.Varchar2).Value = telefono;
            cmd.Parameters.Add("p_cedula_cliente", OracleDbType.Int32).Value = cedula;
            cmd.Parameters.Add("p_estado_cliente", OracleDbType.Varchar2).Value = estado;

            var outputId = new OracleParameter("o_id_cliente", OracleDbType.Varchar2, 20)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            await cmd.ExecuteNonQueryAsync();

            return outputId.Value?.ToString() ?? string.Empty;
        }

        public async Task ActualizarDatosCliente(
            string idCliente,
            string nombre,
            string email,
            string telefono,
            int cedula)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_datos_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = idCliente;
            cmd.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2).Value = nombre;
            cmd.Parameters.Add("p_email_cliente", OracleDbType.Varchar2).Value = email;
            cmd.Parameters.Add("p_telefono_cliente", OracleDbType.Varchar2).Value = telefono;
            cmd.Parameters.Add("p_cedula_cliente", OracleDbType.Int32).Value = cedula;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ActualizarCorreo(string idCliente, string nuevoCorreo)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_correo_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = idCliente;
            cmd.Parameters.Add("p_email_cliente", OracleDbType.Varchar2).Value = nuevoCorreo;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ActualizarNombre(string idCliente, string nuevoNombre)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_nombre_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = idCliente;
            cmd.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2).Value = nuevoNombre;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ActualizarTelefono(string idCliente, string telefono)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_telefono_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = idCliente;
            cmd.Parameters.Add("p_telefono_cliente", OracleDbType.Varchar2).Value = telefono;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ActualizarCedula(string idCliente, int cedula)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_cedula_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = idCliente;
            cmd.Parameters.Add("p_cedula_cliente", OracleDbType.Int32).Value = cedula;

            await cmd.ExecuteNonQueryAsync();
        }
    }
}

