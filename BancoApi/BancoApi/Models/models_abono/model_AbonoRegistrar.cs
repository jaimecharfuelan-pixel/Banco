namespace BancoApi.Models.models_abono
{
    public class model_AbonoRegistrar
    {
        public int idPrestamo { get; set; }
        public decimal montoAbono { get; set; }
        public string tipoAbono { get; set; } = string.Empty;
        public int idCuenta { get; set; }

        public void Validar()
        {
            if (idPrestamo <= 0)
                throw new ArgumentException("El id del préstamo debe ser mayor que cero.");

            if (idCuenta <= 0)
                throw new ArgumentException("El id de la cuenta debe ser mayor que cero.");

            if (montoAbono <= 0)
                throw new ArgumentException("El monto del abono debe ser mayor que cero.");

            if (string.IsNullOrWhiteSpace(tipoAbono))
                throw new ArgumentException("El tipo de abono es requerido.");
        }
    }
}
