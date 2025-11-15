using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;

[ApiController]
[Route("api/auth")]
public class controller_autenticacion : ControllerBase
{
    private readonly IConfiguration _config;

    public controller_autenticacion(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        string cs = _config.GetConnectionString("OracleDb");

        using var conn = new OracleConnection(cs);
        conn.Open();

        // =====================================================
        // LOGIN ADMINISTRADOR
        // =====================================================
        using var cmdAdmin = new OracleCommand(
            "BEGIN :result := pkg_administrador.fn_login_admin(:email, :pass); END;",
            conn);

        cmdAdmin.Parameters.Add("result", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
        cmdAdmin.Parameters.Add("email", req.Email);
        cmdAdmin.Parameters.Add("pass", req.Contrasena);

        cmdAdmin.ExecuteNonQuery();

        var adminResult = cmdAdmin.Parameters["result"].Value;

        if (adminResult != null && adminResult.ToString() != "" && adminResult.ToString() != "null")
        {
            return Ok(new
            {
                tipoUsuario = "ADMIN",
                id = adminResult.ToString()
            });
        }


        // =====================================================
        // LOGIN CUENTA (email del cliente + contraseña de cuenta)
        // =====================================================
        using var cmdCuenta = new OracleCommand(
            "BEGIN :result := pkg_cuenta.fn_login_cuenta(:email, :pass); END;",
            conn);

        cmdCuenta.Parameters.Add("result", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
        cmdCuenta.Parameters.Add("email", req.Email);
        cmdCuenta.Parameters.Add("pass", req.Contrasena);

        cmdCuenta.ExecuteNonQuery();

        var cuentaResult = cmdCuenta.Parameters["result"].Value;

        if (cuentaResult != null && cuentaResult.ToString() != "" && cuentaResult.ToString() != "null")
        {
            return Ok(new
            {
                tipoUsuario = "CUENTA",
                id = cuentaResult.ToString()
            });
        }

        // =====================================================
        // SI NINGUNO COINCIDIÓ → ERROR
        // =====================================================
        return Unauthorized("Credenciales incorrectas.");
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Contrasena { get; set; }
}
