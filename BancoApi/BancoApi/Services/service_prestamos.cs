using BancoApi.DTOs;
using BancoApi.DTOs.DTOs_prestamos;
using BancoApi.Models.models_prestamo;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BancoApi.Services
{
    public interface IPrestamosService
    {
        Task<int> SolicitarPrestamoAsync(DTO_solicitarPrestamo dto);
        Task<IEnumerable<DTO_PrestamoSolicitudItem>> ListarSolicitudesAsync(DTO_PrestamoListarSolicitudes dto);
        Task AceptarPrestamoAsync(DTO_PrestamoAceptar dto);
        Task RechazarPrestamoAsync(DTO_PrestamoRechazar dto);
        Task ActualizarSaldoAsync(DTO_PrestamoActualizarSaldo dto);
    }

    public class PrestamosService : IPrestamosService
    {
        private readonly string _connectionString;

        public PrestamosService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<int> SolicitarPrestamoAsync(DTO_solicitarPrestamo dto)
        {
            // ✅ Aquí usamos tu model_ para validar
            var model = new model_solicitarPrestamo(dto);
            model.Validar();

            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("PKGC_PRESTAMOS.PR_SOLICITAR_PRESTAMO", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cliente", OracleDbType.Varchar2).Value = model.idCliente;
            cmd.Parameters.Add("p_id_sucursal", OracleDbType.Int32).Value = model.idSucursal;
            cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = model.monto;
            cmd.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = model.fechaInicio;
            cmd.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = model.fechaFin;

            cmd.Parameters.Add("o_id_prestamo", OracleDbType.Int32).Direction = ParameterDirection.Output;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            var idPrestamo = Convert.ToInt32(cmd.Parameters["o_id_prestamo"].Value.ToString());
            return idPrestamo;
        }

        public async Task<IEnumerable<DTO_PrestamoSolicitudItem>> ListarSolicitudesAsync(DTO_PrestamoListarSolicitudes dto)
        {
            var modelEntrada = new model_PrestamoListarSolicitudes(dto);
            modelEntrada.Validar();

            var lista = new List<DTO_PrestamoSolicitudItem>();

            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("PKGC_PRESTAMOS.PR_LISTAR_SOLICITUDES", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_sucursal", OracleDbType.Int32).Value = modelEntrada.idSucursal;
            cmd.Parameters.Add("o_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DTO_PrestamoSolicitudItem
                {
                    idPrestamo = reader.GetInt32(0),
                    idCliente = reader.GetString(1),
                    monto = reader.GetDecimal(2),
                    fechaInicio = reader.GetDateTime(3),
                    fechaVencimiento = reader.GetDateTime(4),
                    estado = reader.GetString(5)
                });
            }

            return lista;
        }

        public async Task AceptarPrestamoAsync(DTO_PrestamoAceptar dto)
        {
            var model = new model_PrestamoAceptar(dto);
            model.Validar();

            await EjecutarSimpleAsync("PKGC_PRESTAMOS.PR_ACEPTAR", model.idPrestamo);
        }

        public async Task RechazarPrestamoAsync(DTO_PrestamoRechazar dto)
        {
            var model = new model_PrestamoRechazar(dto);
            model.Validar();

            await EjecutarSimpleAsync("PKGC_PRESTAMOs.PR_RECHAZAR", model.idPrestamo);
        }

        public async Task ActualizarSaldoAsync(DTO_PrestamoActualizarSaldo dto)
        {
            var model = new model_PrestamoActualizarSaldo(dto);
            model.Validar();

            await EjecutarSimpleAsync("PKGC_PRESTAMOS.PR_ACTUALIZAR_SALDO", model.idPrestamo);
        }

        private async Task EjecutarSimpleAsync(string nombreProcedimiento, int idPrestamo)
        {
            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand(nombreProcedimiento, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_prestamo", OracleDbType.Int32).Value = idPrestamo;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}