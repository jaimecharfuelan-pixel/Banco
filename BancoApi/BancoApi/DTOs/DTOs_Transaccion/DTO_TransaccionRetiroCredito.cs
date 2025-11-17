namespace BancoApi.DTOs.DTOs_Transaccion
{
    public class DTO_TransaccionRetiroCredito
    {
        public int IdTarjeta { get; set; }
        public int IdCajero { get; set; }
        public decimal Monto { get; set; }
    }
}
