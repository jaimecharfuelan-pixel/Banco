using BancoApi.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace BancoApi.Services
{
    public class service_Sucursal
    {
        private readonly string att_connectionString;

        public service_Sucursal(IConfiguration config)
        {
            att_connectionString = config.GetConnectionString("OracleDb");
        }


        // ============================================================
        // 1. LISTAR SUCURSALES  (pr_listar_sucursales)
        // ============================================================
        public async Task<List<model_SucursalLista>> function_listarSucursales()
        {
            var lista = new List<model_SucursalLista>();

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_sucursal.pr_listar_sucursales", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("o_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync();

            var refCursor = (OracleRefCursor)cmd.Parameters["o_cursor"].Value;
            using var reader = refCursor.GetDataReader();

            while (reader.Read())
            {
                lista.Add(new model_SucursalLista
                {
                    idSucursal = Convert.ToInt32(reader["ID_SUCURSAL"]),
                    nombreSucursal = reader["NOMBRE_SUCURSAL"].ToString(),
                    direccionSucursal = reader["DIRECCION_SUCURSAL"].ToString(),
                    telefonoSucursal = reader["TELEFONO_SUCURSAL"].ToString(),
                    estadoSucursal = reader["ESTADO_SUCURSAL"].ToString(),
                    idAdministrador = reader["ID_ADMINISTRADOR"] == DBNull.Value
                                        ? (int?)null
                                        : Convert.ToInt32(reader["ID_ADMINISTRADOR"])
                });
            }


            return lista;
        }


        // ============================================================
        // 2. CREAR SUCURSAL (fn_crear_sucursal)
        // ============================================================

        public async Task<int> function_crearSucursal(model_SucursalCrear request)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_sucursal.fn_crear_sucursal", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // 🔥 ESTA LÍNEA ES OBLIGATORIA 🔥
            cmd.BindByName = true;

            cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = request.nombreSucursal;
            cmd.Parameters.Add("p_direccion", OracleDbType.Varchar2).Value = request.direccionSucursal;
            cmd.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = request.telefonoSucursal;

            cmd.Parameters.Add("p_id_admin", OracleDbType.Int32).Value =
                request.idAdministrador.HasValue
                    ? request.idAdministrador.Value
                    : (object)DBNull.Value;

            var output = new OracleParameter("RETURN_VALUE", OracleDbType.Int32)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(output);

            await cmd.ExecuteNonQueryAsync();

            return int.Parse(output.Value.ToString());

        }


        // ============================================================
        // 3. EDITAR ESTADO (pr_editar_estado)
        // ============================================================
        public async Task function_editarEstado(int idSucursal, string estado)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_sucursal.pr_editar_estado", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_sucursal", OracleDbType.Int32).Value = idSucursal;
            cmd.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = estado;

            await cmd.ExecuteNonQueryAsync();
        }


        // ============================================================
        // 4. ELIMINAR SUCURSAL (pr_eliminar_sucursal)
        // ============================================================
        public async Task function_eliminarSucursal(int idSucursal)
        {
            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkg_sucursal.pr_eliminar_sucursal", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_sucursal", OracleDbType.Int32).Value = idSucursal;

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
