# 📚 Explicaciones de Conceptos Técnicos del Proyecto

## 📑 Índice de Conceptos

### ✅ Conceptos Ya Implementados en el Proyecto

1. ✅ **Controller (Controlador)** - [Ver explicación](#1-controller-controlador)
   > *Recibe peticiones HTTP del frontend, valida datos y llama al Service. Es el "recepcionista" de tu API.*

2. ✅ **Service (Servicio)** - [Ver explicación](#2-service-servicio)
   > *Contiene la lógica de negocio (reglas de tu aplicación). Valida, normaliza datos y orquesta llamadas al Repository.*

3. ✅ **Model (Modelo)** - [Ver explicación](#3-model-modelo)
   > *Representa datos y contiene lógica de validación/normalización. Separa datos (DTO) de reglas de negocio.*

4. ✅ **DTO (Data Transfer Object)** - [Ver explicación](#4-dto-data-transfer-object)
   > *Objeto que SOLO contiene datos para transferir entre capas. Sin lógica, solo propiedades.*

5. ✅ **Repository (Repositorio)** - [Ver explicación](#5-repository-repositorio)
   > *Se encarga SOLO de acceder a la base de datos. Ejecuta procedimientos almacenados y retorna datos.*

6. ✅ **appsettings.json** - [Ver explicación](#6-appsettingsjson-configuración)
   > *Archivo JSON con configuraciones (cadenas de conexión, claves JWT, etc.). Se lee con `IConfiguration`.*

7. ✅ **Connection String (Cadena de Conexión)** - [Ver explicación](#7-connection-string-cadena-de-conexión)
   > *Texto con usuario, contraseña y servidor para conectarse a la BD. Se guarda en `appsettings.json`.*

8. ✅ **Dependency Injection (Inyección de Dependencias)** - [Ver explicación](#8-dependency-injection-inyección-de-dependencias)
   > *Las clases reciben sus dependencias (servicios, repositorios) por el constructor. Se registra en `Program.cs`.*

9. ✅ **Program.cs** - [Ver explicación](#9-programcs-punto-de-entrada)
   > *Archivo principal que configura la aplicación: registra servicios, configura JWT/CORS y arranca el servidor.*

10. ✅ **Namespace (Espacio de Nombres)** - [Ver explicación](#10-namespace-espacio-de-nombres)
    > *Organiza código en "carpetas lógicas" (ej: `BancoApi.Controllers`). Evita conflictos de nombres.*

11. ✅ **Async/Await (Asíncrono)** - [Ver explicación](#11-asyncawait-asíncrono)
    > *Permite esperar operaciones largas (BD, red) sin bloquear el servidor. Métodos con `async` y `await`.*

12. ✅ **Try-Catch (Manejo de Errores)** - [Ver explicación](#12-try-catch-manejo-de-errores)
    > *Captura errores durante la ejecución y los maneja de forma controlada. Evita que la app se caiga.*

13. ✅ **HTTP Methods (Métodos HTTP)** - [Ver explicación](#13-http-methods-métodos-http)
    > *Indican la acción: GET (leer), POST (crear), PUT (actualizar), DELETE (eliminar). Se usan como `[HttpPost]`.*

14. ✅ **JSON (JavaScript Object Notation)** - [Ver explicación](#14-json-javascript-object-notation)
    > *Formato de texto para intercambiar datos entre frontend y backend. Se convierte automáticamente en C#.*

15. ✅ **JWT (JSON Web Token)** - [Ver explicación](#15-jwt-json-web-token)
    > *Token generado al hacer login que autentica peticiones. Se genera en `service_JWT` y se valida en `Program.cs`.*

16. ✅ **CORS (Cross-Origin Resource Sharing)** - [Ver explicación](#16-cors-cross-origin-resource-sharing)
    > *Permite que el frontend (puerto diferente) llame a la API. Configurado en `Program.cs` con `AllowAnyOrigin()`.*

17. ✅ **Swagger** - [Ver explicación](#17-swagger-documentación-de-api)
    > *Interfaz web interactiva que documenta y permite probar tu API. Acceso en `https://localhost:7251/swagger`.*

18. ✅ **Validaciones Frontend** - [Ver explicación](#18-validaciones-frontend)
    > *Validar datos en el navegador ANTES de enviarlos al servidor. Mejora UX y reduce carga en el servidor.*

19. ✅ **Config.js (Configuración Centralizada)** - [Ver explicación](#19-configjs-configuración-centralizada)
    > *Archivo JavaScript que centraliza todas las URLs de la API. Función `apiCall()` maneja errores automáticamente.*

20. ✅ **Manejo de Errores de Red** - [Ver explicación](#20-manejo-de-errores-de-red)
    > *Detecta cuando la API no responde y muestra mensajes apropiados. Implementado en `apiCall()` de `config.js`.*

### ⏳ Conceptos Pendientes (No Implementados Aún)

21. ⏳ **Logging y Auditoría** - [Ver explicación](#21-logging-y-auditoría-pendiente)
    > *Registrar eventos importantes (errores, acciones) en archivos o BD. Útil para debugging y auditoría.*

22. ⏳ **Authorization (Autorización)** - [Ver explicación](#22-authorization-autorización-pendiente)
    > *Verificar que un usuario tiene PERMISOS para hacer una acción. Diferente de autenticación (login).*

23. ⏳ **Unit Testing (Pruebas Unitarias)** - [Ver explicación](#23-unit-testing-pruebas-unitarias-pendiente)
    > *Código que prueba automáticamente que tu código funciona. Se ejecuta antes de desplegar.*

24. ⏳ **Middleware** - [Ver explicación](#24-middleware-pendiente)
    > *Código que se ejecuta en cada petición HTTP antes del controlador. Útil para logging, validación global.*

25. ⏳ **Entity Framework** - [Ver explicación](#25-entity-framework-pendiente)
    > *Framework que mapea objetos C# a tablas automáticamente. Alternativa a procedimientos almacenados.*

---

## 📖 Explicaciones Detalladas

### 1. Controller (Controlador) ✅

**¿Qué es?**
Un Controller es una clase que recibe las peticiones HTTP del frontend y decide qué hacer con ellas. Es como el "recepcionista" de tu API.

**¿Dónde está en tu proyecto?**
- 📁 Carpeta: `BancoApi/BancoApi/Controllers/`
- 📄 Ejemplo: `controller_Cliente.cs` (líneas 1-189)
- 📄 Otros: `controller_Admin.cs`, `controller_Sucursal.cs`, `controller_Cajero.cs`, etc.

**Cómo se usa:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 10-16)
[ApiController]  // ← Marca esta clase como controlador de API
[Route("api/controller_Cliente")]  // ← Define la ruta base
public class controller_Cliente : ControllerBase
{
    private readonly service_Cliente att_serviceCliente;
    
    // Constructor recibe el servicio (Dependency Injection)
    public controller_Cliente(service_Cliente prm_service)
    {
        att_serviceCliente = prm_service;
    }
}
```

**Ejemplo de endpoint:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 32-47)
[HttpPost("service_solicitarCuenta")]  // ← Método HTTP y ruta
public async Task<IActionResult> service_solicitarCuenta([FromBody] DTO_ClienteRequest dto)
{
    try
    {
        var model = new model_ClienteRequest(dto);
        var att_clienteId = await att_serviceCliente.function_crearCliente(model);
        return Ok(new { message = "Cuenta creada exitosamente", id_cliente = att_clienteId });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
```

**¿Para qué sirve?**
- Recibe peticiones del frontend
- Valida que los datos vengan correctos
- Llama al Service para hacer el trabajo
- Devuelve una respuesta (éxito o error)

**Flujo:**
```
Frontend → Controller → Service → Repository → Base de Datos
         ← Controller ← Service ← Repository ←
```

---

### 2. Service (Servicio) ✅

**¿Qué es?**
Un Service contiene la lógica de negocio de tu aplicación. Es donde decides QUÉ hacer con los datos, no CÓMO guardarlos.

**¿Dónde está en tu proyecto?**
- 📁 Carpeta: `BancoApi/BancoApi/Services/`
- 📄 Ejemplo: `service_Cliente.cs` (líneas 1-94)
- 📄 Otros: `service_Autenticacion.cs`, `service_Admin.cs`, `service_JWT.cs`, etc.

**Cómo se usa:**
```csharp
// BancoApi/Services/service_Cliente.cs (línea 10-17)
public class service_Cliente
{
    private readonly Repository_Cliente _repository;  // ← Usa el repositorio
    
    public service_Cliente(Repository_Cliente repository)  // ← Recibe por inyección
    {
        _repository = repository;
    }
    
    // Lógica de negocio
    public async Task<string> function_crearCliente(model_ClienteRequest prm_request)
    {
        prm_request.Normalizar();  // ← Normaliza datos
        prm_request.Validar();      // ← Valida datos
        return await _repository.CrearCliente(...);  // ← Llama al repositorio
    }
}
```

**¿Para qué sirve?**
- Contiene la lógica de negocio (reglas de tu aplicación)
- Valida y normaliza datos antes de guardarlos
- Orquesta las llamadas al repositorio
- Puede combinar datos de múltiples fuentes

**Diferencia con Controller:**
- **Controller**: "¿Qué quiere el usuario?" → Llama al Service
- **Service**: "¿Cómo lo hago?" → Ejecuta la lógica
- **Repository**: "¿Dónde lo guardo?" → Accede a la BD

---

### 3. Model (Modelo) ✅

**¿Qué es?**
Un Model representa la estructura de datos y puede contener lógica de validación y transformación.

**¿Dónde está en tu proyecto?**
- 📁 Carpeta: `BancoApi/BancoApi/Models/models_cliente/`
- 📄 Ejemplo: `model_ClienteRequest.cs` (líneas 1-69)
- 📁 También: `models_cuenta/`, `models_admin/`, `models_autenticacion/`, etc.

**Cómo se usa:**
```csharp
// BancoApi/Models/models_cliente/model_ClienteRequest.cs (línea 6-68)
public class model_ClienteRequest
{
    private DTO_ClienteRequest _dto;  // ← Contiene el DTO
    
    // Propiedades que exponen los datos
    public string prm_nombre_cliente => _dto.prm_nombre_cliente;
    public string prm_email_cliente => _dto.prm_email_cliente;
    
    // Lógica de validación
    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(prm_nombre_cliente))
            throw new ArgumentException("El nombre del cliente es requerido");
        // ... más validaciones
    }
    
    // Lógica de normalización
    public void Normalizar()
    {
        if (_dto.prm_nombre_cliente != null)
            _dto.prm_nombre_cliente = _dto.prm_nombre_cliente.Trim();
        // ... más normalizaciones
    }
}
```

**¿Para qué sirve?**
- Define la estructura de datos
- Contiene lógica de validación
- Normaliza datos (trim, lowercase, etc.)
- Separa datos (DTO) de lógica (Model)

**Diferencia con DTO:**
- **DTO**: Solo datos, sin lógica
- **Model**: Datos + lógica de validación/normalización

---

### 4. DTO (Data Transfer Object) ✅

**¿Qué es?**
Un DTO es un objeto que SOLO contiene datos para transferir entre capas. NO tiene lógica.

**¿Dónde está en tu proyecto?**
- 📁 Carpeta: `BancoApi/BancoApi/DTOs/`
- 📄 Ejemplo: `DTO_ClienteRequest.cs`
- 📄 Otros: `DTO_ClienteActualizarDatos.cs`, `DTO_ClienteActualizarCorreo.cs`, etc.

**Cómo se usa:**
```csharp
// BancoApi/DTOs/DTO_ClienteRequest.cs
namespace BancoApi.DTOs
{
    public class DTO_ClienteRequest
    {
        // Solo propiedades, sin métodos
        public string prm_nombre_cliente { get; set; }
        public string prm_email_cliente { get; set; }
        public string prm_telefono_cliente { get; set; }
        public int prm_cedula_cliente { get; set; }
    }
}
```

**Uso en Controller:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 33)
[HttpPost("service_solicitarCuenta")]
public async Task<IActionResult> service_solicitarCuenta([FromBody] DTO_ClienteRequest dto)
{
    // Convierte DTO a Model (que tiene lógica)
    var model = new model_ClienteRequest(dto);
    // ...
}
```

**¿Para qué sirve?**
- Transferir datos entre capas sin exponer lógica
- Separar datos de validación
- Facilitar cambios sin afectar otras capas

---

### 5. Repository (Repositorio) ✅

**¿Qué es?**
Un Repository se encarga SOLO de acceder a la base de datos. Separa la lógica de acceso a datos de la lógica de negocio.

**¿Dónde está en tu proyecto?**
- 📁 Carpeta: `BancoApi/BancoApi/Repositories/`
- 📄 Ejemplo: `Repository_Cliente.cs`

**Cómo se usa:**
```csharp
// BancoApi/Repositories/Repository_Cliente.cs
public class Repository_Cliente
{
    private readonly string _connectionString;
    
    public Repository_Cliente(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("OracleDb");
    }
    
    // Solo acceso a BD, sin lógica de negocio
    public async Task<string> CrearCliente(string nombre, string email, ...)
    {
        using var conn = new OracleConnection(_connectionString);
        await conn.OpenAsync();
        
        using var cmd = new OracleCommand("pkg_clientes.crear_cliente", conn);
        // ... ejecuta procedimiento almacenado
    }
}
```

**Uso en Service:**
```csharp
// BancoApi/Services/service_Cliente.cs (línea 29)
return await _repository.CrearCliente(
    prm_request.prm_nombre_cliente,
    prm_request.prm_email_cliente,
    // ...
);
```

**¿Para qué sirve?**
- Centraliza el acceso a la base de datos
- Facilita cambiar de Oracle a otra BD
- Separa lógica de acceso de lógica de negocio
- Facilita testing (puedes crear un repositorio "falso" para pruebas)

---

### 6. appsettings.json (Configuración) ✅

**¿Qué es?**
Un archivo JSON que contiene configuraciones de tu aplicación (cadenas de conexión, claves, URLs, etc.).

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoApi/BancoApi/appsettings.json` (líneas 1-19)

**Cómo se usa:**
```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=BANCO;Password=Oracle1;Data Source=localhost:1521/XEPDB1;"
  },
  "Jwt": {
    "Key": "TuClaveSecretaSuperSeguraParaJWT_Minimo32Caracteres",
    "Issuer": "BancoApi",
    "Audience": "BancoWeb",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Cómo se lee en el código:**
```csharp
// BancoApi/Repositories/Repository_Cliente.cs (línea 8-11)
public Repository_Cliente(IConfiguration config)
{
    _connectionString = config.GetConnectionString("OracleDb");
    // ↑ Lee "ConnectionStrings:OracleDb" del appsettings.json
}

// BancoApi/Program.cs (línea 28)
IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
// ↑ Lee "Jwt:Key" del appsettings.json
```

**¿Para qué sirve?**
- Centralizar configuraciones
- Cambiar valores sin recompilar
- Separar configuraciones por ambiente (Development, Production)
- Mantener secretos fuera del código

**Archivos relacionados:**
- `appsettings.json` - Configuración general
- `appsettings.Development.json` - Configuración solo para desarrollo

---

### 7. Connection String (Cadena de Conexión) ✅

**¿Qué es?**
Una cadena de texto que contiene toda la información necesaria para conectarse a la base de datos.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoApi/BancoApi/appsettings.json` (línea 4)

**Cómo se usa:**
```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=BANCO;Password=Oracle1;Data Source=localhost:1521/XEPDB1;"
  }
}
```

**Estructura de la cadena:**
```
User Id=BANCO          ← Usuario de la base de datos
Password=Oracle1       ← Contraseña
Data Source=localhost:1521/XEPDB1  ← Servidor y base de datos
```

**Cómo se lee:**
```csharp
// BancoApi/Repositories/Repository_Cliente.cs (línea 10)
_connectionString = config.GetConnectionString("OracleDb");
// ↑ Obtiene la cadena de conexión del appsettings.json
```

**Uso en código:**
```csharp
// BancoApi/Repositories/Repository_Cliente.cs (línea 15)
using var conn = new OracleConnection(_connectionString);
await conn.OpenAsync();
```

**¿Para qué sirve?**
- Conectarse a la base de datos
- Centralizar la configuración de conexión
- Cambiar de base de datos fácilmente

---

### 8. Dependency Injection (Inyección de Dependencias) ✅

**¿Qué es?**
Un patrón donde las clases reciben sus dependencias (servicios, repositorios) a través del constructor, en lugar de crearlas ellas mismas.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoApi/BancoApi/Program.cs` (líneas 41-51)
- 📄 Ejemplo: `BancoApi/Controllers/controller_Cliente.cs` (línea 22-25)

**Cómo se registra:**
```csharp
// BancoApi/Program.cs (líneas 41-51)
// Registra los servicios para que estén disponibles
builder.Services.AddScoped<Repository_Cliente>();
builder.Services.AddScoped<service_Cliente>();
builder.Services.AddScoped<service_Autenticacion>();
// ...
```

**Cómo se usa:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 22-25)
public controller_Cliente(service_Cliente prm_service)  // ← Recibe el servicio
{
    att_serviceCliente = prm_service;  // ← Lo guarda
}

// BancoApi/Services/service_Cliente.cs (línea 14-17)
public service_Cliente(Repository_Cliente repository)  // ← Recibe el repositorio
{
    _repository = repository;  // ← Lo guarda
}
```

**¿Para qué sirve?**
- Facilita testing (puedes inyectar un servicio "falso")
- Reduce acoplamiento entre clases
- El framework crea las instancias automáticamente
- Mejor organización del código

**Tipos de inyección:**
- `AddScoped`: Una instancia por petición HTTP
- `AddSingleton`: Una instancia para toda la aplicación
- `AddTransient`: Una nueva instancia cada vez

---

### 9. Program.cs (Punto de Entrada) ✅

**¿Qué es?**
El archivo principal que configura y arranca la aplicación. Es el "cerebro" de tu API.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoApi/BancoApi/Program.cs` (líneas 1-84)

**Qué hace:**
```csharp
// BancoApi/Program.cs

// 1. Crea el builder de la aplicación
var builder = WebApplication.CreateBuilder(args);

// 2. Registra controladores
builder.Services.AddControllers();

// 3. Registra Swagger (documentación)
builder.Services.AddSwaggerGen();

// 4. Registra servicios (Dependency Injection)
builder.Services.AddScoped<service_Cliente>();
builder.Services.AddScoped<Repository_Cliente>();
// ...

// 5. Configura JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });

// 6. Configura CORS
builder.Services.AddCors(options => { ... });

// 7. Construye la aplicación
var app = builder.Build();

// 8. Configura el pipeline (middleware)
app.UseSwagger();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 9. Arranca la aplicación
app.Run();
```

**¿Para qué sirve?**
- Configurar toda la aplicación
- Registrar servicios
- Configurar middleware
- Arrancar el servidor

---

### 10. Namespace (Espacio de Nombres) ✅

**¿Qué es?**
Un namespace organiza tu código en "carpetas lógicas". Evita conflictos de nombres.

**¿Dónde está en tu proyecto?**
- En todos los archivos `.cs`

**Ejemplos:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 7)
namespace BancoApi.Controllers
{
    public class controller_Cliente : ControllerBase { }
}

// BancoApi/Services/service_Cliente.cs (línea 7)
namespace BancoApi.Services
{
    public class service_Cliente { }
}

// BancoApi/Models/models_cliente/model_ClienteRequest.cs (línea 3)
namespace BancoApi.Models.models_cliente
{
    public class model_ClienteRequest { }
}
```

**Cómo se usa:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (líneas 1-3)
using BancoApi.Models.models_cliente;  // ← Importa el namespace
using BancoApi.Services;
using BancoApi.DTOs;
```

**¿Para qué sirve?**
- Organizar el código
- Evitar conflictos de nombres
- Hacer el código más legible
- Separar por funcionalidad

---

### 11. Async/Await (Asíncrono) ✅

**¿Qué es?**
Permite que tu código espere operaciones largas (como consultas a BD) sin bloquear el hilo principal.

**¿Dónde está en tu proyecto?**
- En todos los métodos que acceden a la BD

**Ejemplos:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 32)
public async Task<IActionResult> service_solicitarCuenta(...)
//     ↑ async indica que el método es asíncrono
{
    var att_clienteId = await att_serviceCliente.function_crearCliente(model);
    //                    ↑ await espera a que termine la operación
    return Ok(...);
}

// BancoApi/Services/service_Cliente.cs (línea 20)
public async Task<string> function_crearCliente(...)
{
    return await _repository.CrearCliente(...);
    //     ↑ await espera la respuesta de la BD
}

// BancoApi/Repositories/Repository_Cliente.cs (línea 15)
public async Task<string> CrearCliente(...)
{
    await conn.OpenAsync();  // ← await espera que se abra la conexión
    await cmd.ExecuteNonQueryAsync();  // ← await espera que termine la consulta
}
```

**¿Para qué sirve?**
- No bloquea el servidor mientras espera la BD
- Permite manejar múltiples peticiones simultáneas
- Mejora el rendimiento
- Es necesario para operaciones de I/O (BD, archivos, red)

**Regla:**
- Si un método usa `await`, debe ser `async`
- Si un método es `async`, debe retornar `Task` o `Task<T>`

---

### 12. Try-Catch (Manejo de Errores) ✅

**¿Qué es?**
Captura errores que pueden ocurrir durante la ejecución y permite manejarlos de forma controlada.

**¿Dónde está en tu proyecto?**
- En todos los controladores

**Ejemplos:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (líneas 35-56)
[HttpPost("service_solicitarCuenta")]
public async Task<IActionResult> service_solicitarCuenta([FromBody] DTO_ClienteRequest dto)
{
    try  // ← Intenta ejecutar este código
    {
        var model = new model_ClienteRequest(dto);
        var att_clienteId = await att_serviceCliente.function_crearCliente(model);
        return Ok(new { message = "Cuenta creada exitosamente", id_cliente = att_clienteId });
    }
    catch (ArgumentException ex)  // ← Si hay un error de validación
    {
        return BadRequest(new { error = ex.Message });  // ← Devuelve error 400
    }
    catch (Exception ex)  // ← Si hay cualquier otro error
    {
        return BadRequest(new { error = ex.Message });  // ← Devuelve error 400
    }
}
```

**Tipos de excepciones:**
- `ArgumentException`: Error de validación (datos incorrectos)
- `Exception`: Cualquier otro error

**¿Para qué sirve?**
- Evitar que la aplicación se caiga
- Devolver mensajes de error apropiados
- Registrar errores para debugging
- Mejorar la experiencia del usuario

---

### 13. HTTP Methods (Métodos HTTP) ✅

**¿Qué es?**
Los métodos HTTP indican QUÉ acción quieres hacer con un recurso.

**¿Dónde está en tu proyecto?**
- En todos los controladores, como atributos `[HttpPost]`, `[HttpGet]`, etc.

**Métodos comunes:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs

[HttpPost("service_solicitarCuenta")]  // ← POST: Crear algo nuevo
public async Task<IActionResult> service_solicitarCuenta(...)

[HttpPut("service_actualizarDatosCliente")]  // ← PUT: Actualizar algo existente
public async Task<IActionResult> service_actualizarDatosCliente(...)

[HttpGet("service_listar")]  // ← GET: Obtener/leer datos (ejemplo en otros controladores)
public async Task<IActionResult> service_listar(...)

[HttpDelete("service_eliminar/{id}")]  // ← DELETE: Eliminar algo (ejemplo en otros controladores)
public async Task<IActionResult> service_eliminar(int id)
```

**Significado:**
- **GET**: Leer/obtener datos (no modifica nada)
- **POST**: Crear algo nuevo
- **PUT**: Actualizar algo existente
- **DELETE**: Eliminar algo

**¿Para qué sirve?**
- Indica la intención de la petición
- Permite que el servidor sepa qué hacer
- Estándar REST API

---

### 14. JSON (JavaScript Object Notation) ✅

**¿Qué es?**
Un formato de texto para intercambiar datos entre frontend y backend.

**¿Dónde está en tu proyecto?**
- En todas las peticiones HTTP entre frontend y backend

**Ejemplo en Frontend:**
```javascript
// BancoWeb/BancoWeb/wwwroot/registro.html
const obj_requestData = {
    prm_email_cliente: "usuario@email.com",
    prm_nombre_cliente: "Juan Pérez",
    prm_telefono_cliente: "3001234567",
    prm_cedula_cliente: 1234567890
};

// Se convierte a JSON para enviar
fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(obj_requestData)  // ← Convierte objeto a JSON
});
```

**Ejemplo en Backend:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 33)
[HttpPost("service_solicitarCuenta")]
public async Task<IActionResult> service_solicitarCuenta([FromBody] DTO_ClienteRequest dto)
//                                                          ↑ Convierte JSON a objeto C#
{
    // dto ya es un objeto C# con los datos del JSON
}
```

**Respuesta JSON:**
```csharp
// BancoApi/Controllers/controller_Cliente.cs (línea 43-47)
return Ok(new
{
    message = "Cuenta creada exitosamente",
    id_cliente = att_clienteId
});
// ↑ Se convierte automáticamente a JSON
```

**¿Para qué sirve?**
- Intercambiar datos entre frontend y backend
- Formato estándar y fácil de leer
- Compatible con JavaScript y C#

---

### 15. JWT (JSON Web Token) ✅

**¿Qué es?**
Un token que se genera cuando un usuario hace login y se usa para autenticar peticiones posteriores.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoApi/BancoApi/Services/service_JWT.cs`
- 📄 Archivo: `BancoApi/BancoApi/Services/service_Autenticacion.cs` (líneas 52, 80)
- 📄 Archivo: `BancoApi/BancoApi/appsettings.json` (líneas 6-11)
- 📄 Archivo: `BancoApi/BancoApi/Program.cs` (líneas 22-36)

**Cómo se genera:**
```csharp
// BancoApi/Services/service_Autenticacion.cs (línea 52)
var token = _jwtService.GenerarToken(adminId, "ADMIN");

// BancoApi/Services/service_JWT.cs
public string GenerarToken(string userId, string tipoUsuario)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim("tipoUsuario", tipoUsuario)
    };
    
    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(60),
        signingCredentials: creds
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**Configuración:**
```json
// BancoApi/appsettings.json (líneas 6-11)
"Jwt": {
  "Key": "TuClaveSecretaSuperSeguraParaJWT_Minimo32Caracteres",
  "Issuer": "BancoApi",
  "Audience": "BancoWeb",
  "ExpirationMinutes": 60
}
```

**¿Para qué sirve?**
- Autenticar usuarios sin guardar sesión en el servidor
- Verificar que el usuario está logueado
- Incluir información del usuario en el token
- Más seguro que guardar contraseñas

**Nota:** Actualmente se genera el token, pero no se valida en endpoints protegidos. Para usarlo, agregar `[Authorize]` a los controladores.

---

### 16. CORS (Cross-Origin Resource Sharing) ✅

**¿Qué es?**
Permite que tu frontend (que corre en un puerto) llame a tu API (que corre en otro puerto).

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoApi/BancoApi/Program.cs` (líneas 54-64, 78)

**Configuración actual:**
```csharp
// BancoApi/Program.cs (líneas 56-64)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // ← ⚠️ Permite CUALQUIER origen
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// BancoApi/Program.cs (línea 78)
app.UseCors("AllowAll");  // ← Aplica la política CORS
```

**¿Para qué sirve?**
- Permite que `localhost:5000` (frontend) llame a `localhost:7251` (API)
- Necesario cuando frontend y backend están en diferentes puertos/dominios

**⚠️ Seguridad:**
- `AllowAnyOrigin()` es peligroso en producción
- En producción, usar: `WithOrigins("https://tudominio.com")`

---

### 17. Swagger (Documentación de API) ✅

**¿Qué es?**
Una interfaz web que muestra toda tu API de forma interactiva. Te permite probar endpoints sin escribir código.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoApi/BancoApi/Program.cs` (líneas 14-15, 71-74)

**Configuración:**
```csharp
// BancoApi/Program.cs (líneas 14-15)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// BancoApi/Program.cs (líneas 71-74)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

**Cómo acceder:**
1. Ejecuta la API
2. Abre en el navegador: `https://localhost:7251/swagger`
3. Verás todos tus endpoints documentados
4. Puedes probarlos directamente desde ahí

**¿Para qué sirve?**
- Documentar tu API automáticamente
- Probar endpoints sin escribir código
- Ver qué parámetros necesita cada endpoint
- Compartir la API con otros desarrolladores

---

### 18. Validaciones Frontend ✅

**¿Qué es?**
Validar datos en el navegador ANTES de enviarlos al servidor.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoWeb/BancoWeb/wwwroot/registro.html` (líneas 43-72)
- 📄 Archivo: `BancoWeb/BancoWeb/wwwroot/login.html`

**Ejemplo:**
```javascript
// BancoWeb/wwwroot/registro.html (líneas 49-72)
// Validaciones frontend
const email = document.getElementById("elm_email").value.trim();

if (!email) {
    alert("El email es requerido");
    return;  // ← No envía si hay error
}

if (!email.includes("@") || !email.includes(".")) {
    alert("El email debe tener un formato válido");
    return;
}

if (!nombre) {
    alert("El nombre es requerido");
    return;
}

// Solo si pasa todas las validaciones, envía al servidor
const obj_result = await apiCall(API_CONFIG.CLIENTE.SOLICITAR_CUENTA, {
    method: "POST",
    body: obj_requestData
});
```

**¿Para qué sirve?**
- Mejor experiencia de usuario (errores inmediatos)
- Menos carga en el servidor
- Validaciones básicas antes de enviar
- Reduce peticiones innecesarias

**Nota:** Las validaciones frontend NO reemplazan las validaciones backend. Siempre valida en ambos lados.

---

### 19. Config.js (Configuración Centralizada) ✅

**¿Qué es?**
Un archivo JavaScript que centraliza todas las URLs y configuraciones del frontend.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoWeb/BancoWeb/wwwroot/js/config.js`

**Contenido:**
```javascript
// BancoWeb/wwwroot/js/config.js
const API_BASE_URL = "https://localhost:7251/api";

const API_CONFIG = {
    AUTH: {
        LOGIN: `${API_BASE_URL}/auth/login`,
    },
    CLIENTE: {
        SOLICITAR_CUENTA: `${API_BASE_URL}/controller_Cliente/service_solicitarCuenta`,
        ACTUALIZAR_DATOS: `${API_BASE_URL}/controller_Cliente/service_actualizarDatosCliente`,
        // ...
    },
    // ...
};

// Función helper para hacer llamadas a la API
async function apiCall(url, options = {}) {
    // Maneja errores, timeouts, etc.
}
```

**Uso:**
```javascript
// BancoWeb/wwwroot/registro.html (línea 38, 82)
<script src="js/config.js"></script>
<script>
    const obj_result = await apiCall(API_CONFIG.CLIENTE.SOLICITAR_CUENTA, {
        method: "POST",
        body: obj_requestData
    });
</script>
```

**¿Para qué sirve?**
- Centralizar URLs (si cambias el puerto, solo cambias un archivo)
- Función `apiCall()` maneja errores automáticamente
- Más fácil de mantener
- Menos errores de tipeo

---

### 20. Manejo de Errores de Red ✅

**¿Qué es?**
Detectar y manejar errores cuando la API no responde o está caída.

**¿Dónde está en tu proyecto?**
- 📄 Archivo: `BancoWeb/BancoWeb/wwwroot/js/config.js` (función `apiCall`)

**Implementación:**
```javascript
// BancoWeb/wwwroot/js/config.js (función apiCall)
async function apiCall(endpoint, options = {}) {
    try {
        const response = await fetch(API_BASE_URL + endpoint, options);
        
        if (!response.ok) {
            // Maneja errores HTTP (400, 500, etc.)
            throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        
        return await response.json();
    } catch (error) {
        // Detecta si la API está caída
        if (error.message.includes('Failed to fetch')) {
            throw new Error("No se pudo conectar con el servidor. Verifica que la API esté ejecutándose.");
        }
        throw error;
    }
}
```

**Uso:**
```javascript
// BancoWeb/wwwroot/registro.html (líneas 81-92)
try {
    const obj_result = await apiCall(API_CONFIG.CLIENTE.SOLICITAR_CUENTA, {
        method: "POST",
        body: obj_requestData
    });
    alert("Cuenta creada exitosamente. ID: " + obj_result.id_cliente);
} catch (error) {
    alert(error.message || "Error de conexión con el servidor");
    // ↑ Muestra mensaje apropiado si la API está caída
}
```

**¿Para qué sirve?**
- Detectar cuando la API no responde
- Mostrar mensajes apropiados al usuario
- Evitar que la aplicación se "cuelgue"
- Mejor experiencia de usuario

---

## ⏳ Conceptos Pendientes

### 21. Logging y Auditoría ⏳

**¿Qué es?**
Registrar eventos importantes (errores, acciones de usuarios) en archivos o base de datos.

**¿Dónde se usaría?**
- Crear un servicio `service_Logging.cs`
- Crear una tabla `AUDITORIA` en la BD
- Registrar acciones importantes en los servicios

**Ejemplo de uso futuro:**
```csharp
_logger.LogInformation("Usuario {UserId} creó cuenta {CuentaId}", userId, cuentaId);
_logger.LogError("Error al crear cuenta: {Error}", ex.Message);
```

**¿Para qué sirve?**
- Ver qué está pasando en producción
- Debuggear errores
- Auditoría (quién hizo qué y cuándo)

---

### 22. Authorization (Autorización) ⏳

**¿Qué es?**
Verificar que un usuario tiene PERMISOS para hacer una acción específica.

**Diferencia con Autenticación:**
- **Autenticación**: "¿Quién eres?" (login)
- **Autorización**: "¿Puedes hacer esto?" (permisos)

**¿Dónde se usaría?**
- Agregar `[Authorize]` a controladores
- Crear roles (Admin, Cliente)
- Verificar permisos en servicios

**Ejemplo de uso futuro:**
```csharp
[Authorize(Roles = "ADMIN")]  // ← Solo admins pueden acceder
public async Task<IActionResult> EliminarCuenta(int id) { }
```

---

### 23. Unit Testing (Pruebas Unitarias) ⏳

**¿Qué es?**
Escribir código que prueba automáticamente que tu código funciona correctamente.

**¿Dónde se usaría?**
- Crear proyecto `BancoApi.Tests`
- Escribir tests para cada servicio
- Ejecutar tests antes de desplegar

**Ejemplo de uso futuro:**
```csharp
[Fact]
public void CrearCliente_ConDatosValidos_RetornaId()
{
    // Arrange
    var service = new service_Cliente(mockRepository);
    
    // Act
    var result = service.function_crearCliente(datosValidos);
    
    // Assert
    Assert.NotNull(result);
}
```

---

### 24. Middleware ⏳

**¿Qué es?**
Código que se ejecuta en cada petición HTTP antes de llegar al controlador.

**¿Dónde se usaría?**
- Crear middleware personalizado en `Program.cs`
- Logging automático
- Validación de tokens
- Manejo de errores global

**Ejemplo de uso futuro:**
```csharp
app.Use(async (context, next) =>
{
    // Código que se ejecuta antes de cada petición
    _logger.LogInformation("Petición: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});
```

---

### 25. Entity Framework ⏳

**¿Qué es?**
Un framework que mapea objetos C# a tablas de base de datos automáticamente.

**Diferencia con tu enfoque actual:**
- **Tu enfoque**: Llamas procedimientos almacenados directamente
- **Entity Framework**: Crea objetos que representan tablas y hace consultas en C#

**Ejemplo de uso futuro:**
```csharp
// En lugar de llamar procedimientos, harías:
var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);
_context.Clientes.Add(nuevoCliente);
await _context.SaveChangesAsync();
```

**Nota:** Tu enfoque actual (procedimientos almacenados) es válido y funciona bien. Entity Framework es una alternativa.

---

## 📝 Resumen de Mejores Prácticas

1. **Separación de Responsabilidades**: Cada capa tiene su función
   - Controller: Recibe peticiones
   - Service: Lógica de negocio
   - Repository: Acceso a BD
   - Model: Validación y normalización
   - DTO: Solo datos

2. **Consistencia**: Usa el mismo estilo en todo el proyecto
   - Rutas: `api/controller_*/service_*`
   - Nombres: Prefijos consistentes (`att_`, `prm_`, `model_`, etc.)

3. **Validación**: Valida en frontend Y backend
   - Frontend: Mejor experiencia de usuario
   - Backend: Seguridad

4. **Manejo de Errores**: Siempre usa try-catch
   - Captura errores específicos (`ArgumentException`)
   - Devuelve códigos HTTP apropiados

5. **Configuración**: Centraliza valores que pueden cambiar
   - `appsettings.json` para backend
   - `config.js` para frontend

6. **Documentación**: Mantén el código documentado
   - Comentarios donde sea necesario
   - Swagger para la API

---

**Última actualización**: Noviembre 2025
