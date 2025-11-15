namespace BancoApi.Models
{
    // Este modelo representa la estructura del JSON recibido desde el frontend.
    // Prefijo "model_" indica que pertenece a la capa de modelos de la API.
    public class model_ClienteRequest
    {
        // Nombre del cliente enviado desde el formulario HTML → JS → API
        public string prm_nombre_cliente { get; set; }

        // Email del cliente que será validado en Oracle
        public string prm_email_cliente { get; set; }

        // Teléfono ingresado por el cliente
        public string prm_telefono_cliente { get; set; }

        // Cédula enviada desde el HTML; Oracle valida duplicados
        public int prm_cedula_cliente { get; set; }
    }
}
