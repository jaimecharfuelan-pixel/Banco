// ============================================================
// CONFIGURACIÓN DE API
// Centraliza todas las URLs de la API
// ============================================================

const API_CONFIG = {
    BASE_URL: "https://localhost:7251/api",
    
    // Endpoints de autenticación
    AUTH: {
        LOGIN: "/auth/login"
    },
    
    // Endpoints de cliente
    CLIENTE: {
        SOLICITAR_CUENTA: "/controller_Cliente/service_solicitarCuenta",
        ACTUALIZAR_DATOS: "/controller_Cliente/service_actualizarDatosCliente",
        ACTUALIZAR_CORREO: "/controller_Cliente/service_actualizarCorreo",
        ACTUALIZAR_NOMBRE: "/controller_Cliente/service_actualizarNombre",
        ACTUALIZAR_TELEFONO: "/controller_Cliente/service_actualizarTelefono",
        ACTUALIZAR_CEDULA: "/controller_Cliente/service_actualizarCedula"
    },
    
    // Endpoints de cuenta
    CUENTA: {
        CONSULTAR_POR_CLIENTE: "/controller_Cuenta/service_consultarPorCliente",
        CONSULTAR_POR_ID: "/controller_Cuenta/service_consultarPorId",
        ACTUALIZAR: "/controller_Cuenta/service_actualizarCuenta",
        CAMBIAR_SALDO: "/controller_Cuenta/service_cambiarSaldo",
        CAMBIAR_CONTRASENA: "/controller_Cuenta/service_cambiarContrasena",
        ELIMINAR: "/controller_Cuenta/service_eliminarCuenta"
    },
    
    // Endpoints de sucursal
    SUCURSAL: {
        LISTAR: "/controller_Sucursal/service_listar",
        CREAR: "/controller_Sucursal/service_crear",
        EDITAR_ESTADO: "/controller_Sucursal/service_editarEstado",
        ELIMINAR: "/controller_Sucursal/service_eliminar"
    },
    
    // Endpoints de cajero
    CAJERO: {
        CREAR: "/controller_Cajero/service_crear",
        CAMBIAR_ESTADO: "/controller_Cajero/service_cambiarEstado",
        RECARGAR: "/controller_Cajero/service_recargar",
        DESCONTAR: "/controller_Cajero/service_descontar",
        LISTAR: "/controller_Cajero/service_listar"
    },
    
    // Endpoints de admin
    ADMIN: {
        SOLICITUDES: "/controller_Admin/service_solicitudes",
        CREAR_CUENTA: "/controller_Admin/service_crearCuenta"
    }
};

// ============================================================
// FUNCIÓN HELPER PARA HACER LLAMADAS A LA API
// Maneja errores de red, timeout, etc.
// ============================================================
async function apiCall(endpoint, options = {}) {
    const defaultOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
        timeout: 10000 // 10 segundos
    };
    
    const finalOptions = { ...defaultOptions, ...options };
    
    // Si hay body, convertirlo a JSON
    if (finalOptions.body && typeof finalOptions.body === 'object') {
        finalOptions.body = JSON.stringify(finalOptions.body);
    }
    
    try {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), finalOptions.timeout);
        
        const response = await fetch(API_CONFIG.BASE_URL + endpoint, {
            ...finalOptions,
            signal: controller.signal
        });
        
        clearTimeout(timeoutId);
        
        // Verificar si la respuesta es OK
        if (!response.ok) {
            let errorMessage = "Error en la petición";
            
            try {
                const errorData = await response.json();
                errorMessage = errorData.error || errorMessage;
            } catch {
                errorMessage = `Error ${response.status}: ${response.statusText}`;
            }
            
            throw new Error(errorMessage);
        }
        
        // Intentar parsear como JSON
        try {
            return await response.json();
        } catch {
            return { message: "Operación exitosa" };
        }
        
    } catch (error) {
        if (error.name === 'AbortError') {
            throw new Error("La petición tardó demasiado. Verifica tu conexión.");
        }
        
        if (error.message.includes('Failed to fetch') || error.message.includes('NetworkError')) {
            throw new Error("No se pudo conectar con el servidor. Verifica que la API esté ejecutándose.");
        }
        
        throw error;
    }
}

