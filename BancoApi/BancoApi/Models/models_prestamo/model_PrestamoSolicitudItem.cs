using BancoApi.DTOs;
using BancoApi.DTOs.DTOs_prestamos;

namespace BancoApi.Models.models_prestamo
{
    public class model_PrestamoSolicitudItem
    {
        private DTO_PrestamoSolicitudItem _dto;

        public model_PrestamoSolicitudItem(DTO_PrestamoSolicitudItem dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public int idPrestamo => _dto.idPrestamo;
        public string idCliente => _dto.idCliente;
        public decimal monto => _dto.monto;
        public DateTime fechaInicio => _dto.fechaInicio;
        public DateTime fechaVencimiento => _dto.fechaVencimiento;
        public string estado => _dto.estado;

        public void Validar()
        {
            if (idPrestamo <= 0)
                throw new ArgumentException("El ID del préstamo debe ser válido");

            if (string.IsNullOrWhiteSpace(idCliente))
                throw new ArgumentException("El ID del cliente es requerido");

            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor que cero");
        }
    }
}