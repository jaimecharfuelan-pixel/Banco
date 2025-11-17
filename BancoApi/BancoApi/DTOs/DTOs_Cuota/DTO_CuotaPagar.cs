namespace BancoApi.DTOs
{
    // DTO para pagar una cuota desde una cuenta específica
    public class DTO_CuotaPagar
    {
        public int idCuota { get; set; }
        public int idCuenta { get; set; }
    }
}
