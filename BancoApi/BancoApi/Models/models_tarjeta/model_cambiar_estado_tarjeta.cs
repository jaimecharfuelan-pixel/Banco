namespace BancoAPI.Models.Tarjetas
{
    public class CambiarEstadoTarjetaRequest
    {
        public int IdTarjeta { get; set; }
        public string Estado { get; set; } = string.Empty;
        // 'activa','inactiva','bloqueada','suspendida'
    }
}
