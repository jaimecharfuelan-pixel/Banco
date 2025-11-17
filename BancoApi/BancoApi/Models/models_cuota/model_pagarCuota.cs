using BancoApi.DTOs.DTOs_Cuota;

namespace BancoApi.Models.models_cuota
{
    public class model_pagarCuota
    {
        private readonly DTO_PagarCuota _dto;

        public model_pagarCuota(DTO_PagarCuota dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public int IdCuenta => _dto.IdCuenta;
        public long IdCuota => _dto.IdCuota;

        public void Validar()
        {
            if (IdCuenta <= 0)
            {
                throw new ArgumentException("El IdCuenta debe ser un número positivo.");
            }
            if (IdCuota <= 0)
            {
                throw new ArgumentException("El IdCuota debe ser un número positivo.");
            }
        }
    }
}