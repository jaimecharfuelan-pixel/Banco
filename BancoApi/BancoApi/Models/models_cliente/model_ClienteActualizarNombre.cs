using BancoApi.DTOs;

namespace BancoApi.Models.models_cliente
{
    public class model_ClienteActualizarNombre
    {
        private DTO_ClienteActualizarNombre _dto;

        public model_ClienteActualizarNombre(DTO_ClienteActualizarNombre dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public string idCliente => _dto.idCliente;
        public string nuevoNombre => _dto.nuevoNombre;

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(idCliente))
                throw new ArgumentException("El ID del cliente es requerido");

            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nuevo nombre es requerido");
        }

        public void Normalizar()
        {
            if (_dto.nuevoNombre != null)
                _dto.nuevoNombre = _dto.nuevoNombre.Trim();
        }
    }
}
