namespace BancoApi.Models.models_cuota
{
    public class model_CuotaGenerar
    {
        public int idPrestamo { get; set; }

        // Opcional: validación básica
        public void Validar()
        {
            if (idPrestamo <= 0)
            {
                throw new ArgumentException("El id del préstamo debe ser mayor que cero.");
            }
        }
    }
}
