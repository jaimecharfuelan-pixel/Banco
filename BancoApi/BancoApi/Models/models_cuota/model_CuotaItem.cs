namespace BancoApi.Models.models_cuota
{
    public class model_CuotaItem
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
