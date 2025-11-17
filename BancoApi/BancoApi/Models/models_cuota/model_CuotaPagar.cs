namespace BancoApi.Models.models_cuota
{
	public class model_CuotaPagar
	{
		public int idCuota { get; set; }
		public int idCuenta { get; set; }

		public void Validar()
		{
			if (idCuota <= 0)
				throw new ArgumentException("El id de la cuota debe ser mayor que cero.");

			if (idCuenta <= 0)
				throw new ArgumentException("El id de la cuenta debe ser mayor que cero.");
		}
	}
}
