using BancoApi.DTOs;

namespace BancoApi.Models.models_cliente
{
    public class model_ClienteActualizarCedula
    {
        private DTO_ClienteActualizarCedula _dto;

        public model_ClienteActualizarCedula(DTO_ClienteActualizarCedula dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public string idCliente => _dto.idCliente;
        public int cedula => _dto.cedula;

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(idCliente))
                throw new ArgumentException("El ID del cliente es requerido");

            if (cedula <= 0)
                throw new ArgumentException("La cédula debe ser un número válido");
        }
    }
}
