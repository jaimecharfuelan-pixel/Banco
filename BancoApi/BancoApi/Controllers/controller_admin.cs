using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

[ApiController]
[Route("api/admin")]
public class controller_admin : ControllerBase
{
    private readonly IConfiguration _config;

    public controller_admin(IConfiguration config)
    {
        _config = config;
    }

    // =============================================================
    // 1) OBTENER CLIENTES SIN CUENTAS (SOLICITUDES)
    // =============================================================
    [HttpGet("solicitudes")]
    public IActionResult GetSolicitudes()
    {
        List<object> solicitudes = new();

        using var conn = new OracleConnection(_config.GetConnectionString("OracleDb"));
        conn.Open();

        using var cmd = new OracleCommand("pkg_clientes.obtener_solicitudes", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();

        // Leer cursor devuelto
        var refCursor = (OracleRefCursor)cmd.Parameters["result"].Value;
        using var reader = refCursor.GetDataReader();

        while (reader.Read())
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

        return Ok(solicitudes);
    }

    // =============================================================
    // 2) CREAR CUENTA DE UN CLIENTE
    // =============================================================
    [HttpPost("crear-cuenta")]
    public IActionResult CrearCuenta([FromBody] CrearCuentaRequest req)
    {
        using var conn = new OracleConnection(_config.GetConnectionString("OracleDb"));
        conn.Open();

        using var cmd = new OracleCommand("pkg_cuenta.crear_cuenta", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = req.IdCliente;
        cmd.Parameters.Add("p_id_administrador", OracleDbType.Int32).Value = req.IdAdministrador;

        cmd.Parameters.Add("o_id_cuenta", OracleDbType.Decimal).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();

        var idCuenta = cmd.Parameters["o_id_cuenta"].Value.ToString();

        return Ok(new { idCuenta });
    }
}

// =============================================================
// REQUEST MODEL CORRECTO
// =============================================================
public class CrearCuentaRequest
{
    public string IdCliente { get; set; }       // <-- ahora 100% correcto
    public int IdAdministrador { get; set; }    // <-- con set para asignación
}
