using BancoApi.DTOs;

namespace BancoApi.Models.models_cliente
{
    public class model_ClienteActualizarCorreo
    {
        private DTO_ClienteActualizarCorreo _dto;

        public model_ClienteActualizarCorreo(DTO_ClienteActualizarCorreo dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public string idCliente => _dto.idCliente;
        public string nuevoCorreo => _dto.nuevoCorreo;

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(idCliente))
                throw new ArgumentException("El ID del cliente es requerido");

            if (string.IsNullOrWhiteSpace(nuevoCorreo))
                throw new ArgumentException("El nuevo correo es requerido");

            if (!nuevoCorreo.Contains("@") || !nuevoCorreo.Contains("."))
                throw new ArgumentException("El email debe tener un formato válido");
        }

        public void Normalizar()
        {
            if (_dto.nuevoCorreo != null)
                _dto.nuevoCorreo = _dto.nuevoCorreo.Trim().ToLower();
        }
    }
}
