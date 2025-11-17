namespace BancoApi.DTOs.DTOs_Transaccion
{
    public class DTO_TransaccionRetiroDebito
    {
        public int IdTarjeta { get; set; }
        public int IdCajero { get; set; }
        public decimal Monto { get; set; }
    }
}
