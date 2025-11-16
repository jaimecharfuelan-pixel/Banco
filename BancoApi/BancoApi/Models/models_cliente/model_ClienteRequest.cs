using BancoApi.DTOs;

namespace BancoApi.Models.models_cliente
{
    // Modelo con lógica de validación y transformación
    public class model_ClienteRequest
    {
        private DTO_ClienteRequest _dto;

        public model_ClienteRequest(DTO_ClienteRequest dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        // Propiedades que exponen los datos del DTO
        public string prm_nombre_cliente => _dto.prm_nombre_cliente;
        public string prm_email_cliente => _dto.prm_email_cliente;
        public string prm_telefono_cliente => _dto.prm_telefono_cliente;
        public int prm_cedula_cliente => _dto.prm_cedula_cliente;

        // Lógica de validación
        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(prm_nombre_cliente))
            {
                throw new ArgumentException("El nombre del cliente es requerido");
            }

            if (string.IsNullOrWhiteSpace(prm_email_cliente))
            {
                throw new ArgumentException("El email del cliente es requerido");
            }

            if (!prm_email_cliente.Contains("@") || !prm_email_cliente.Contains("."))
            {
                throw new ArgumentException("El email debe tener un formato válido");
            }

            if (string.IsNullOrWhiteSpace(prm_telefono_cliente))
            {
                throw new ArgumentException("El teléfono del cliente es requerido");
            }

            if (prm_cedula_cliente <= 0)
            {
                throw new ArgumentException("La cédula debe ser un número válido");
            }
        }

        // Lógica de transformación (normalizar datos)
        public void Normalizar()
        {
            if (_dto.prm_nombre_cliente != null)
            {
                _dto.prm_nombre_cliente = _dto.prm_nombre_cliente.Trim();
            }

            if (_dto.prm_email_cliente != null)
            {
                _dto.prm_email_cliente = _dto.prm_email_cliente.Trim().ToLower();
            }

            if (_dto.prm_telefono_cliente != null)
            {
                _dto.prm_telefono_cliente = _dto.prm_telefono_cliente.Trim();
            }
        }
    }
}
