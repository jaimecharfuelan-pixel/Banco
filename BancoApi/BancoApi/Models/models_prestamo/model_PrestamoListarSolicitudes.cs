using BancoApi.DTOs;
using BancoApi.DTOs.DTOs_prestamos;

namespace BancoApi.Models.models_prestamo
{
    public class model_PrestamoListarSolicitudes
    {
        private DTO_PrestamoListarSolicitudes _dto;

        public model_PrestamoListarSolicitudes(DTO_PrestamoListarSolicitudes dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public int idSucursal => _dto.idSucursal;

        public void Validar()
        {
            if (idSucursal <= 0)
                throw new ArgumentException("La sucursal debe ser un número válido");
        }
    }
}