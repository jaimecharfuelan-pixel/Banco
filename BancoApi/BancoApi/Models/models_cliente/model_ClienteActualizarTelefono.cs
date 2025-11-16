using BancoApi.DTOs;

namespace BancoApi.Models.models_cliente
{
    public class model_ClienteActualizarTelefono
    {
        private DTO_ClienteActualizarTelefono _dto;

        public model_ClienteActualizarTelefono(DTO_ClienteActualizarTelefono dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public string idCliente => _dto.idCliente;
        public string telefono => _dto.telefono;

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(idCliente))
                throw new ArgumentException("El ID del cliente es requerido");

            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("El teléfono es requerido");
        }

        public void Normalizar()
        {
            if (_dto.telefono != null)
                _dto.telefono = _dto.telefono.Trim();
        }
    }
}

