using System;
using BancoApi.Dtos.DTOs_Cuota;

namespace BancoApi.Models.models_cuota
{
    }

    public class model_listarCuota
    {
        private readonly DTO_ListarCuota _dto;

        public model_listarCuota(DTO_ListarCuota dto)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public long IdPrestamo => _dto.idPrestamo;
        
        public void Validar()
        {
            if (IdPrestamo <= 0)
            {
                throw new ArgumentException("El IdPrestamo debe ser un número positivo.");
            }
        }
    }

}