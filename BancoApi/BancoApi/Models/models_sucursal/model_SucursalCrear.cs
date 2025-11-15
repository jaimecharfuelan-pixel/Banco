namespace BancoApi.Models
{
    public class model_SucursalCrear
    {
        public string nombreSucursal { get; set; } = "";
        public string direccionSucursal { get; set; } = "";
        public string telefonoSucursal { get; set; } = "";
        public int? idAdministrador { get; set; }
    }
}
