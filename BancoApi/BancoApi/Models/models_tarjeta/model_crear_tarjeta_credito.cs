namespace BancoAPI.Models.Tarjetas
{
    public class CrearTarjetaCreditoRequest
    {
        public int IdCuenta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal TasaInteres { get; set; }
        public decimal CuotaManejo { get; set; }
        public int FechaCorte { get; set; } // día 1..31
    }

    public class CrearTarjetaCreditoResponse
    {
        public int IdTarjeta { get; set; }
    }
}
