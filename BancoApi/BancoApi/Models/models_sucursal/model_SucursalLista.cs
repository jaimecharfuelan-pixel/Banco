namespace BancoApi.Models
{
    public class model_SucursalLista
    {
        public int idSucursal { get; set; }
        public string nombreSucursal { get; set; } = "";
        public string direccionSucursal { get; set; } = "";
        public string telefonoSucursal { get; set; } = "";
        public string estadoSucursal { get; set; } = "";
        public int? idAdministrador { get; set; }

    }
}
