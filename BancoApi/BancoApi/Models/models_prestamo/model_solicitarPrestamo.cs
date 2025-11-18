using BancoApi.DTOs;
using BancoApi.DTOs.DTOs_prestamos;

namespace BancoApi.Models.models_prestamo
{
    public class model_solicitarPrestamo
    {
        private DTO_solicitarPrestamo _dto;

        public model_solicitarPrestamo(DTO_solicitarPrestamo dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public string idCliente => _dto.idCliente;
        public int idSucursal => _dto.idSucursal;
        public decimal monto => _dto.monto;
        public DateTime fechaInicio => _dto.fechaInicio;
        public DateTime fechaFin => _dto.fechaFin;

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(idCliente))
                throw new ArgumentException("El ID del cliente es requerido");

            if (idSucursal <= 0)
                throw new ArgumentException("La sucursal debe ser un número válido");

            if (monto <= 0)
                throw new ArgumentException("El monto del préstamo debe ser mayor a cero");

            if (fechaFin <= fechaInicio)
                throw new ArgumentException("La fecha de fin debe ser mayor a la de inicio");
        }
    }
}
