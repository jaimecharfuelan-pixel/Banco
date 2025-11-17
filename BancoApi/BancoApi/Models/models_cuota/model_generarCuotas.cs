using BancoApi.DTOs.DTOs_Cuota;

namespace BancoApi.Models.models_cuota
{
    public class model_generarCuotas
    {
        private readonly DTO_GenerarCuotas _dto;

        public model_generarCuotas(DTO_GenerarCuotas dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public int IdPrestamo => _dto.idPrestamo;

        public void Validar()
        {
            if (IdPrestamo <= 0)
            {
                throw new ArgumentException("El IdPrestamo debe ser un número positivo.");
            }

        }
    }
}