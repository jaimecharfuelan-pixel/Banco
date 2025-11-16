using System;
using System.Collections.Generic;
using System.Data;
using BancoAPI.Models.Tarjetas;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace BancoAPI.Services
{
    public class TarjetasService
    {
        private readonly string _connectionString;

        public TarjetasService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public CrearTarjetaDebitoResponse CrearTarjetaDebito(CrearTarjetaDebitoRequest request)
        {
            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("pkgc_tarjetas.pr_crear_tarjeta_debito", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = request.IdCuenta;
            cmd.Parameters.Add("p_limite_retiro", OracleDbType.Decimal).Value = request.LimiteRetiro;

            var pIdTarjeta = new OracleParameter("o_id_tarjeta", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pIdTarjeta);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new CrearTarjetaDebitoResponse
            {
                IdTarjeta = Convert.ToInt32(pIdTarjeta.Value.ToString())
            };
        }

        public CrearTarjetaCreditoResponse CrearTarjetaCredito(CrearTarjetaCreditoRequest request)
        {
            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("pkgc_tarjetas.pr_crear_tarjeta_credito", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = request.IdCuenta;
            cmd.Parameters.Add("p_limite_credito", OracleDbType.Decimal).Value = request.LimiteCredito;
            cmd.Parameters.Add("p_tasa_interes", OracleDbType.Decimal).Value = request.TasaInteres;
            cmd.Parameters.Add("p_cuota_manejo", OracleDbType.Decimal).Value = request.CuotaManejo;
            cmd.Parameters.Add("p_fecha_corte", OracleDbType.Int32).Value = request.FechaCorte;

            var pIdTarjeta = new OracleParameter("o_id_tarjeta", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pIdTarjeta);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new CrearTarjetaCreditoResponse
            {
                IdTarjeta = Convert.ToInt32(pIdTarjeta.Value.ToString())
            };
        }

        public void CambiarEstadoTarjeta(CambiarEstadoTarjetaRequest request)
        {
            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("pkgc_tarjetas.pr_cambiar_estado_tarjeta", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_tarjeta", OracleDbType.Int32).Value = request.IdTarjeta;
            cmd.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = request.Estado;

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<TarjetaDTO> ListarTarjetas(int idCuenta)
        {
            var resultado = new List<TarjetaDTO>();

            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("pkgc_tarjetas.pr_listar_tarjetas", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = idCuenta;

            var pCursor = new OracleParameter("o_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pCursor);

            conn.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var tarjeta = new TarjetaDTO
                {
                    IdTarjeta = reader.GetInt32(reader.GetOrdinal("ID_TARJETA")),
                    NumeroTarjeta = reader.GetString(reader.GetOrdinal("NUMERO_TARJETA")),
                    EstadoTarjeta = reader.GetString(reader.GetOrdinal("ESTADO_TARJETA")),
                    FechaEmision = reader.GetDateTime(reader.GetOrdinal("FECHA_EMISION_TARJETA")),
                    FechaVencimiento = reader.GetDateTime(reader.GetOrdinal("FECHA_VENCIMIENTO_TARJETA")),
                    TipoTarjeta = reader.GetString(reader.GetOrdinal("TIPO_TARJETA"))
                };

                resultado.Add(tarjeta);
            }

            return resultado;
        }
    }
}
