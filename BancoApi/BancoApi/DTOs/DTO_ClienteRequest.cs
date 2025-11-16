namespace BancoApi.DTOs
{
    // DTO: Solo datos para transferir, sin lógica
    public class DTO_ClienteRequest
    {
        public string prm_nombre_cliente { get; set; }
        public string prm_email_cliente { get; set; }
        public string prm_telefono_cliente { get; set; }
        public int prm_cedula_cliente { get; set; }
    }
}

