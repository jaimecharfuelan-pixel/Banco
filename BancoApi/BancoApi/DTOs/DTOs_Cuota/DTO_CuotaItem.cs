namespace BancoApi.DTOs
{
    // DTO que representa una cuota en las respuestas de la API
    public class DTO_CuotaItem
    {
        public int idCuota { get; set; }
        public int numeroCuota { get; set; }
        public decimal montoCuota { get; set; }
        public decimal capitalCuota { get; set; }
        public string? fechaVencimiento { get; set; }
        public string? fechaPago { get; set; }
        public string estado { get; set; } = string.Empty;
    }
}
