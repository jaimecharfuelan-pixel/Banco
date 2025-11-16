namespace BancoAPI.Models.Tarjetas
{
    public class CrearTarjetaDebitoRequest
    {
        public int IdCuenta { get; set; }
        public decimal LimiteRetiro { get; set; }
    }

    public class CrearTarjetaDebitoResponse
    {
        public int IdTarjeta { get; set; }
    }
}
