using System;

namespace BancoAPI.Models.Tarjetas
{
    public class ListarTarjetasRequest
    {
        public int IdCuenta { get; set; }
    }

    public class TarjetaDTO
    {
        public int IdTarjeta { get; set; }
        public string NumeroTarjeta { get; set; } = string.Empty;
        public string EstadoTarjeta { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string TipoTarjeta { get; set; } = string.Empty; // 'credito' / 'debito'
    }
}
