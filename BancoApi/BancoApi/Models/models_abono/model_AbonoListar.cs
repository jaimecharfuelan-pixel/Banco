namespace BancoApi.Models.models_abono
{
    public class model_AbonoListar
    {
        public int idPrestamo { get; set; }

        public void Validar()
        {
            if (idPrestamo <= 0)
                throw new ArgumentException("El id del préstamo debe ser mayor que cero.");
        }
    }
}
