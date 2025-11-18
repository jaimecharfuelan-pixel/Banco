using BancoApi.DTOs;
using BancoApi.DTOs.DTOs_prestamos;

namespace BancoApi.Models.models_prestamo
{
    public class model_PrestamoAceptar
    {
        private DTO_PrestamoAceptar _dto;

        public model_PrestamoAceptar(DTO_PrestamoAceptar dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public int idPrestamo => _dto.idPrestamo;

        public void Validar()
        {
            if (idPrestamo <= 0)
                throw new ArgumentException("El ID del préstamo debe ser válido");
        }
    }
}