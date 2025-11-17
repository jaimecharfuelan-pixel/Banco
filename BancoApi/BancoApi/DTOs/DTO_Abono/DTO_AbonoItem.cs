namespace BancoApi.DTOs
{
    // DTO que representa un abono en las respuestas
    public class DTO_AbonoItem
    {
        public int idAbono { get; set; }
        public int idPrestamo { get; set; }
        public int? idTransaccion { get; set; }
        public decimal montoAbono { get; set; }
        public string? fechaAbono { get; set; }
        public string tipoAbono { get; set; } = string.Empty;
    }
}
