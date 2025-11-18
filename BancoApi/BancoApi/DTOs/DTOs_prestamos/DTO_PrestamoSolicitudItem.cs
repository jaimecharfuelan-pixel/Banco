namespace BancoApi.DTOs.DTOs_prestamos
{
    // Coincide con las columnas que devuelve el cursor de PR_LISTAR_SOLICITUDES
    public class DTO_PrestamoSolicitudItem
    {
        public int idPrestamo { get; set; }
        public string idCliente { get; set; }
        public decimal monto { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public string estado { get; set; }
    }
}
