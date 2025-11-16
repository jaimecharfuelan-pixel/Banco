using BancoApi.Models.models_autenticacion;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BancoApi.Services
{
    public class service_Autenticacion
    {
        private readonly string att_connectionString;
        private readonly service_JWT _jwtService;

        public service_Autenticacion(IConfiguration config, service_JWT jwtService)
        {
            att_connectionString = config.GetConnectionString("OracleDb");
            _jwtService = jwtService;
        }

        public async Task<object?> function_login(model_LoginRequest req)
        {
            // Validar inputs
            if (string.IsNullOrWhiteSpace(req.Email))
            {
                throw new ArgumentException("El email es requerido");
            }

            if (string.IsNullOrWhiteSpace(req.Contrasena))
            {
                throw new ArgumentException("La contraseña es requerida");
            }

            using var conn = new OracleConnection(att_connectionString);
            await conn.OpenAsync();

            // =====================================================
            // LOGIN ADMINISTRADOR
            // =====================================================
            using var cmdAdmin = new OracleCommand(
                "BEGIN :result := pkg_administrador.fn_login_admin(:email, :pass); END;",
                conn);

            cmdAdmin.Parameters.Add("result", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
            cmdAdmin.Parameters.Add("email", OracleDbType.Varchar2).Value = req.Email;
            cmdAdmin.Parameters.Add("pass", OracleDbType.Varchar2).Value = req.Contrasena;

            await cmdAdmin.ExecuteNonQueryAsync();

            var adminResult = cmdAdmin.Parameters["result"].Value;

            if (adminResult != null && adminResult.ToString() != "" && adminResult.ToString() != "null")
            {
                var adminId = adminResult.ToString();
                var token = _jwtService.GenerarToken(adminId, "ADMIN");

                return new
                {
                    tipoUsuario = "ADMIN",
                    id = adminId,
                    token = token
                };
            }

            // =====================================================
            // LOGIN CUENTA (email del cliente + contraseña de cuenta)
            // =====================================================
            using var cmdCuenta = new OracleCommand(
                "BEGIN :result := pkg_cuenta.fn_login_cuenta(:email, :pass); END;",
                conn);

            cmdCuenta.Parameters.Add("result", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
            cmdCuenta.Parameters.Add("email", OracleDbType.Varchar2).Value = req.Email;
            cmdCuenta.Parameters.Add("pass", OracleDbType.Varchar2).Value = req.Contrasena;

            await cmdCuenta.ExecuteNonQueryAsync();

            var cuentaResult = cmdCuenta.Parameters["result"].Value;

            if (cuentaResult != null && cuentaResult.ToString() != "" && cuentaResult.ToString() != "null")
            {
                var cuentaId = cuentaResult.ToString();
                var token = _jwtService.GenerarToken(cuentaId, "CUENTA");

                return new
                {
                    tipoUsuario = "CUENTA",
                    id = cuentaId,
                    token = token
                };
            }

            // =====================================================
            // SI NINGUNO COINCIDIÓ → ERROR
            // =====================================================
            return null;
        }
    }
}

