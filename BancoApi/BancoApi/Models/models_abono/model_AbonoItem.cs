namespace BancoApi.Models.models_abono
{
    public class model_AbonoItem
    {
        public int idAbono { get; set; }
        public int idPrestamo { get; set; }
        public int? idTransaccion { get; set; }
        public decimal montoAbono { get; set; }
        public string? fechaAbono { get; set; }
        public string tipoAbono { get; set; } = string.Empty;
    }
}
