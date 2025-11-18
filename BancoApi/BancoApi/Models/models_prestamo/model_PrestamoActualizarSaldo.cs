using BancoApi.DTOs;
using BancoApi.DTOs.DTOs_prestamos;

namespace BancoApi.Models.models_prestamo
{
    public class model_PrestamoActualizarSaldo
    {
        private DTO_PrestamoActualizarSaldo _dto;

        public model_PrestamoActualizarSaldo(DTO_PrestamoActualizarSaldo dto)
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