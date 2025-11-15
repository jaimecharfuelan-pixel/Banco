namespace BancoApi.Models
{
    public class model_CajeroLista
    {
        public int idCajero { get; set; }
        public decimal dineroDisponible { get; set; }
        public string estado { get; set; }
        public DateTime fechaUltimaRecarga { get; set; }
    }
}
