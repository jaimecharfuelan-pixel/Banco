using System;

namespace BancoApi.DTOs.DTOs_Transaccion
{
    public class DTO_TransaccionDetalleResponse
    {
        public int IdTransaccion { get; set; }
        public int? IdCajero { get; set; }
        public int? IdTarjeta { get; set; }
        public int? IdCuentaOrigen { get; set; }
        public int? IdCuentaDestino { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public string TipoTransaccion { get; set; } = string.Empty;
        public decimal MontoTransaccion { get; set; }
        public string EstadoTransaccion { get; set; } = string.Empty;
        public string? DescripcionTransaccion { get; set; }
    }
}
