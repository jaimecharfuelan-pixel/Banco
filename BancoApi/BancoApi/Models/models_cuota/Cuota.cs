using System;

namespace BancoApi.Models.models_cuota
{

    public class Cuota
    {
        public long IdCuota { get; set; }
        public long IdPrestamo { get; set; }
        public int NumeroCuota { get; set; }
        public decimal MontoDeCuota { get; set; }
        public decimal CapitalCuota { get; set; }
        public DateTime? FechaDeVencimientoCuota { get; set; }
        public DateTime? FechaDePagoCuota { get; set; }
        public string EstadoCuota { get; set; }
    }
}