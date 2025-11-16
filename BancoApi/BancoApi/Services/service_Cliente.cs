using BancoApi.Models.models_cliente;
using BancoApi.Repositories;
using BancoApi.DTOs;



namespace BancoApi.Services
{
    // Prefijo service_ indica que esta clase pertenece a la capa de Servicios
    public class service_Cliente
    {
        private readonly Repository_Cliente _repository;

        public service_Cliente(Repository_Cliente repository)
        {
            _repository = repository;
        }

        // Método principal que ejecuta el procedimiento en Oracle
        public async Task<string> function_crearCliente(model_ClienteRequest prm_request)
        {
            // Normalizar datos (trim, lowercase, etc.)
            prm_request.Normalizar();

            // Validar usando la lógica del modelo
            prm_request.Validar();

            // Llamar al repositorio para acceder a la BD
            return await _repository.CrearCliente(
                prm_request.prm_nombre_cliente,
                prm_request.prm_email_cliente,
                prm_request.prm_telefono_cliente,
                prm_request.prm_cedula_cliente,
                "ACTIVO"
            );
        }

        // ============================================================
        //  ACTUALIZAR DATOS COMPLETOS
        // ============================================================
        public async Task function_actualizarDatos(model_ClienteActualizarDatos req)
        {
            // Normalizar y validar
            req.Normalizar();
            req.Validar();

            // Llamar al repositorio
            await _repository.ActualizarDatosCliente(
                req.idCliente,
                req.nombre,
                req.email,
                req.telefono,
                req.cedula
            );
        }


        // ============================================================
        //  ACTUALIZAR CORREO
        // ============================================================
        public async Task function_actualizarCorreo(model_ClienteActualizarCorreo req)
        {
            req.Normalizar();
            req.Validar();
            await _repository.ActualizarCorreo(req.idCliente, req.nuevoCorreo);
        }


        // ============================================================
        //  ACTUALIZAR NOMBRE
        // ============================================================
        public async Task function_actualizarNombre(model_ClienteActualizarNombre req)
        {
            req.Normalizar();
            req.Validar();
            await _repository.ActualizarNombre(req.idCliente, req.nuevoNombre);
        }


        // ============================================================
        //  ACTUALIZAR TELEFONO
        // ============================================================
        public async Task function_actualizarTelefono(model_ClienteActualizarTelefono req)
        {
            req.Normalizar();
            req.Validar();
            await _repository.ActualizarTelefono(req.idCliente, req.telefono);
        }


        // ============================================================
        //  ACTUALIZAR CEDULA
        // ============================================================
        public async Task function_actualizarCedula(model_ClienteActualizarCedula req)
        {
            req.Validar();
            await _repository.ActualizarCedula(req.idCliente, req.cedula);
        }


       

    }
}
