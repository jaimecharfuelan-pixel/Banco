namespace BancoApi.DTOs.DTOs_Transaccion
{
    public class DTO_TransaccionTransferencia
    {
        public int CuentaOrigen { get; set; }
        public int CuentaDestino { get; set; }
        public decimal Monto { get; set; }
    }
}
