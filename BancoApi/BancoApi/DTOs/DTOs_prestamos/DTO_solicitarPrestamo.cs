namespace BancoApi.DTOs.DTOs_prestamos
{
    public class DTO_solicitarPrestamo
    {
        public string idCliente { get; set; }
        public int idSucursal { get; set; }
        public decimal monto { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
    }
}