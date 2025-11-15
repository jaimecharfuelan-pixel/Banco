using BancoApi.Models;                  // Importa el modelo renombrado
using BancoApi.Models.models_cliente;

using Oracle.ManagedDataAccess.Client;  // Driver de Oracle
using System.Data;                      // Tipos para parámetros y comandos



namespace BancoApi.Services
{
    // Prefijo service_ indica que esta clase pertenece a la capa de Servicios
    public class service_Cliente
    {
        private readonly string att_connectionString;
        // Atributo (att_) donde se guarda la cadena de conexión

        public service_Cliente(IConfiguration config)
        {
            // Se lee la cadena de conexión desde appsettings.json
            att_connectionString = config.GetConnectionString("OracleDb");
        }

        // Método principal que ejecuta el procedimiento en Oracle
        public async Task<string> function_crearCliente(model_ClienteRequest prm_request)
        {
            // Crear objeto conexión (todavía no se abre)
            using var conn = new OracleConnection(att_connectionString);

            // Abrir conexión
            await conn.OpenAsync();

            // Crear comando que ejecuta el procedimiento almacenado
            using var cmd = new OracleCommand("pkg_clientes.crear_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Parámetros de entrada recibidos desde el HTML
            cmd.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2)
                           .Value = prm_request.prm_nombre_cliente;

            cmd.Parameters.Add("p_email_cliente", OracleDbType.Varchar2)
                           .Value = prm_request.prm_email_cliente;

            cmd.Parameters.Add("p_telefono_cliente", OracleDbType.Varchar2)
                           .Value = prm_request.prm_telefono_cliente;

            cmd.Parameters.Add("p_cedula_cliente", OracleDbType.Int32)
                           .Value = prm_request.prm_cedula_cliente;

            // Estado por defecto ACTIVO
            cmd.Parameters.Add("p_estado_cliente", OracleDbType.Varchar2)
                           .Value = "ACTIVO";

            // Parámetro de salida para el ID generado por Oracle
            var att_outputId = new OracleParameter("o_id_cliente", OracleDbType.Varchar2, 20);
            att_outputId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(att_outputId);

            // Ejecutar procedimiento Oracle
            await cmd.ExecuteNonQueryAsync();

            // Retornar ID generado por el package Oracle
            return att_outputId.Value.ToString();
        }

        // ============================================================
        //  ACTUALIZAR DATOS COMPLETOS
        // ============================================================
        public async Task function_actualizarDatos(model_ClienteActualizarDatos req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_datos_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.idCliente;
            cmd.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2).Value = req.nombre;
            cmd.Parameters.Add("p_email_cliente", OracleDbType.Varchar2).Value = req.email;
            cmd.Parameters.Add("p_telefono_cliente", OracleDbType.Varchar2).Value = req.telefono;
            cmd.Parameters.Add("p_cedula_cliente", OracleDbType.Int32).Value = req.cedula;

            await cmd.ExecuteNonQueryAsync();
        }


        // ============================================================
        //  ACTUALIZAR CORREO
        // ============================================================
        public async Task function_actualizarCorreo(model_ClienteActualizarCorreo req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_correo_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.idCliente;
            cmd.Parameters.Add("p_email_cliente", OracleDbType.Varchar2).Value = req.nuevoCorreo;

            await cmd.ExecuteNonQueryAsync();
        }


        // ============================================================
        //  ACTUALIZAR NOMBRE
        // ============================================================
        public async Task function_actualizarNombre(model_ClienteActualizarNombre req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_nombre_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.idCliente;
            cmd.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2).Value = req.nuevoNombre;

            await cmd.ExecuteNonQueryAsync();
        }


        // ============================================================
        //  ACTUALIZAR TELEFONO
        // ============================================================
        public async Task function_actualizarTelefono(model_ClienteActualizarTelefono req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_telefono_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.idCliente;
            cmd.Parameters.Add("p_telefono_cliente", OracleDbType.Varchar2).Value = req.telefono;

            await cmd.ExecuteNonQueryAsync();
        }


        // ============================================================
        //  ACTUALIZAR CEDULA
        // ============================================================
        public async Task function_actualizarCedula(model_ClienteActualizarCedula req)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_clientes.actualizar_cedula_cliente", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.idCliente;
            cmd.Parameters.Add("p_cedula_cliente", OracleDbType.Int32).Value = req.cedula;

            await cmd.ExecuteNonQueryAsync();
        }


       

    }
}
