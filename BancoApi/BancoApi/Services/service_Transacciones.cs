using BancoApi.Models.models_transaccion;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BancoApi.Services
{
    public interface ITransaccionesService
    {
        Task<int> TransferenciaAsync(model_transferencia modelo);
        Task<int> RetiroDebitoAsync(model_retiro_debito modelo);
        Task<int> RetiroCreditoAsync(model_retiro_credito modelo);
        Task<IEnumerable<model_transaccion_detalle>> HistorialCuentaAsync(int idCuenta);
        Task<IEnumerable<model_transaccion_detalle>> HistorialCajeroAsync(int idCuenta);
    }

    public class service_Transacciones : ITransaccionesService
    {
        private readonly string _connectionString;

        public service_Transacciones(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        // ---------------- TRANSFERENCIA ENTRE CUENTAS ----------------
        public async Task<int> TransferenciaAsync(model_transferencia modelo)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_transacciones.pr_transferencia", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_cuenta_origen", OracleDbType.Int32).Value = modelo.CuentaOrigen;
            cmd.Parameters.Add("p_cuenta_destino", OracleDbType.Int32).Value = modelo.CuentaDestino;
            cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = modelo.Monto;

            var pOut = new OracleParameter("o_id_transaccion", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pOut);

            await cmd.ExecuteNonQueryAsync();

            int idTransaccion = pOut.Value == null || pOut.Value == DBNull.Value
                ? 0
                : int.Parse(pOut.Value.ToString()!);

            modelo.IdTransaccion = idTransaccion;
            return idTransaccion;
        }

        // ---------------- RETIRO DÉBITO ----------------
        public async Task<int> RetiroDebitoAsync(model_retiro_debito modelo)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_transacciones.pr_retiro_debito", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_tarjeta", OracleDbType.Int32).Value = modelo.IdTarjeta;
            cmd.Parameters.Add("p_id_cajero", OracleDbType.Int32).Value = modelo.IdCajero;
            cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = modelo.Monto;

            var pOut = new OracleParameter("o_id_transaccion", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pOut);

            await cmd.ExecuteNonQueryAsync();

            int idTransaccion = pOut.Value == null || pOut.Value == DBNull.Value
                ? 0
                : int.Parse(pOut.Value.ToString()!);

            modelo.IdTransaccion = idTransaccion;
            return idTransaccion;
        }

        // ---------------- RETIRO CRÉDITO ----------------
        public async Task<int> RetiroCreditoAsync(model_retiro_credito modelo)
        {
            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_transacciones.pr_retiro_credito", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_tarjeta", OracleDbType.Int32).Value = modelo.IdTarjeta;
            cmd.Parameters.Add("p_id_cajero", OracleDbType.Int32).Value = modelo.IdCajero;
            cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = modelo.Monto;

            var pOut = new OracleParameter("o_id_transaccion", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pOut);

            await cmd.ExecuteNonQueryAsync();

            int idTransaccion = pOut.Value == null || pOut.Value == DBNull.Value
                ? 0
                : int.Parse(pOut.Value.ToString()!);

            modelo.IdTransaccion = idTransaccion;
            return idTransaccion;
        }

        // ---------------- HISTORIAL POR CUENTA ----------------
        public async Task<IEnumerable<model_transaccion_detalle>> HistorialCuentaAsync(int idCuenta)
        {
            var lista = new List<model_transaccion_detalle>();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_transacciones.pr_historial_cuenta", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = idCuenta;

            var cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cursor);

            await cmd.ExecuteNonQueryAsync();

            using var reader = ((OracleRefCursor)cursor.Value!).GetDataReader();
            while (await reader.ReadAsync())
            {
                var trx = new model_transaccion_detalle
                {
                    IdTransaccion = reader.GetInt32(reader.GetOrdinal("ID_TRANSACCION")),
                    IdCajero = reader.IsDBNull(reader.GetOrdinal("ID_CAJERO")) ? null : reader.GetInt32(reader.GetOrdinal("ID_CAJERO")),
                    IdTarjeta = reader.IsDBNull(reader.GetOrdinal("ID_TARJETA")) ? null : reader.GetInt32(reader.GetOrdinal("ID_TARJETA")),
                    IdCuentaOrigen = reader.IsDBNull(reader.GetOrdinal("ID_CUENTA_ORIGEN")) ? null : reader.GetInt32(reader.GetOrdinal("ID_CUENTA_ORIGEN")),
                    IdCuentaDestino = reader.IsDBNull(reader.GetOrdinal("ID_CUENTA_DESTINO")) ? null : reader.GetInt32(reader.GetOrdinal("ID_CUENTA_DESTINO")),
                    FechaTransaccion = reader.GetDateTime(reader.GetOrdinal("FECHA_TRANSACCION")),
                    TipoTransaccion = reader.GetString(reader.GetOrdinal("TIPO_TRANSACCION")),
                    MontoTransaccion = reader.GetDecimal(reader.GetOrdinal("MONTO_TRANSACCION")),
                    EstadoTransaccion = reader.GetString(reader.GetOrdinal("ESTADO_TRANSACCION")),
                    DescripcionTransaccion = reader.IsDBNull(reader.GetOrdinal("DESCRIPCION_TRANSACCION"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("DESCRIPCION_TRANSACCION"))
                };

                lista.Add(trx);
            }

            return lista;
        }

        // ---------------- HISTORIAL DE CAJERO POR CUENTA ----------------
        public async Task<IEnumerable<model_transaccion_detalle>> HistorialCajeroAsync(int idCuenta)
        {
            var lista = new List<model_transaccion_detalle>();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand("pkgc_transacciones.pr_historial_cajero", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = idCuenta;

            var cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cursor);

            await cmd.ExecuteNonQueryAsync();

            using var reader = ((OracleRefCursor)cursor.Value!).GetDataReader();
            while (await reader.ReadAsync())
            {
                var trx = new model_transaccion_detalle
                {
                    IdTransaccion = reader.GetInt32(reader.GetOrdinal("ID_TRANSACCION")),
                    IdCajero = reader.IsDBNull(reader.GetOrdinal("ID_CAJERO")) ? null : reader.GetInt32(reader.GetOrdinal("ID_CAJERO")),
                    IdTarjeta = reader.IsDBNull(reader.GetOrdinal("ID_TARJETA")) ? null : reader.GetInt32(reader.GetOrdinal("ID_TARJETA")),
                    IdCuentaOrigen = reader.IsDBNull(reader.GetOrdinal("ID_CUENTA_ORIGEN")) ? null : reader.GetInt32(reader.GetOrdinal("ID_CUENTA_ORIGEN")),
                    IdCuentaDestino = reader.IsDBNull(reader.GetOrdinal("ID_CUENTA_DESTINO")) ? null : reader.GetInt32(reader.GetOrdinal("ID_CUENTA_DESTINO")),
                    FechaTransaccion = reader.GetDateTime(reader.GetOrdinal("FECHA_TRANSACCION")),
                    TipoTransaccion = reader.GetString(reader.GetOrdinal("TIPO_TRANSACCION")),
                    MontoTransaccion = reader.GetDecimal(reader.GetOrdinal("MONTO_TRANSACCION")),
                    EstadoTransaccion = reader.GetString(reader.GetOrdinal("ESTADO_TRANSACCION")),
                    DescripcionTransaccion = reader.IsDBNull(reader.GetOrdinal("DESCRIPCION_TRANSACCION"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("DESCRIPCION_TRANSACCION"))
                };

                lista.Add(trx);
            }

            return lista;
        }
    }
}
