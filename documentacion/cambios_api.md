# 📋 CAMBIOS EN LA API - Versión Actual vs Versión Anterior

**Fecha de actualización:** 16 de Noviembre de 2025

Este documento detalla todos los cambios realizados en la API comparando la versión anterior con la versión actual.

---

## 🔐 1. AUTENTICACIÓN JWT (NUEVO)

### **Cambio Principal:**
Se implementó autenticación JWT (JSON Web Tokens) en la API.

### **Archivos Nuevos:**
- `BancoApi/Services/service_Autenticacion.cs` - Servicio de autenticación
- `BancoApi/Services/service_JWT.cs` - Servicio para generar y validar tokens JWT
- `BancoApi/Controllers/controller_autenticacion.cs` - Controlador de autenticación
- `BancoApi/Models/models_autenticacion/model_LoginRequest.cs` - Modelo para login

### **Cambios en Program.cs:**

**Versión Anterior:**
```csharp
// No había autenticación
builder.Services.AddScoped<service_Cliente>();
builder.Services.AddScoped<service_Sucursal>();
builder.Services.AddScoped<service_Cajero>();
builder.Services.AddScoped<service_Cuenta>();
```

**Versión Actual:**
```csharp
// JWT Configuration
builder.Services.AddScoped<service_JWT>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* configuración JWT */ });

// Repositories
builder.Services.AddScoped<Repository_Cliente>();

// Services
builder.Services.AddScoped<service_Cliente>();
builder.Services.AddScoped<service_Sucursal>();
builder.Services.AddScoped<service_Cajero>();
builder.Services.AddScoped<service_Cuenta>();
builder.Services.AddScoped<service_Autenticacion>();  // ← NUEVO
builder.Services.AddScoped<service_Admin>();        // ← NUEVO
```

### **Nuevo Endpoint:**
- `POST /api/auth/login` - Endpoint para autenticación

---

## 📦 2. IMPLEMENTACIÓN DE DTOs (NUEVO)

### **Cambio Principal:**
Se separaron los DTOs (Data Transfer Objects) de los Models para mejor organización.

### **Archivos Nuevos:**
- `BancoApi/DTOs/DTO_ClienteRequest.cs`
- `BancoApi/DTOs/DTO_ClienteActualizarCedula.cs`
- `BancoApi/DTOs/DTO_ClienteActualizarCorreo.cs`
- `BancoApi/DTOs/DTO_ClienteActualizarDatos.cs`
- `BancoApi/DTOs/DTO_ClienteActualizarNombre.cs`
- `BancoApi/DTOs/DTO_ClienteActualizarTelefono.cs`

### **Ejemplo de Cambio:**

**Versión Anterior:**
```csharp
[HttpPost("service_solicitarCuenta")]
public async Task<IActionResult> service_solicitarCuenta(
    [FromBody] model_ClienteRequest prm_request)  // ← Usaba Model directamente
{
    var att_clienteId = await att_serviceCliente.function_crearCliente(prm_request);
    // ...
}
```

**Versión Actual:**
```csharp
[HttpPost("service_solicitarCuenta")]
public async Task<IActionResult> service_solicitarCuenta(
    [FromBody] DTO_ClienteRequest dto)  // ← Ahora usa DTO
{
    var model = new model_ClienteRequest(dto);  // ← Convierte DTO a Model
    var att_clienteId = await att_serviceCliente.function_crearCliente(model);
    // ...
}
```

**Ventajas:**
- Separación de responsabilidades
- Los DTOs solo contienen datos, los Models contienen lógica
- Mejor validación y control de datos

---

## 🏗️ 3. REPOSITORY PATTERN (NUEVO)

### **Cambio Principal:**
Se implementó el patrón Repository para separar la lógica de acceso a datos.

### **Archivo Nuevo:**
- `BancoApi/Repositories/Repository_Cliente.cs`

### **Ejemplo:**

**Versión Anterior:**
```csharp
// El servicio accedía directamente a la base de datos
public class service_Cliente
{
    public async Task<string> function_crearCliente(...)
    {
        using var conn = new OracleConnection(...);
        // Lógica de acceso a BD directamente aquí
    }
}
```

**Versión Actual:**
```csharp
// El servicio usa el Repository
public class service_Cliente
{
    private readonly Repository_Cliente att_repository;
    
    public service_Cliente(Repository_Cliente prm_repository)
    {
        att_repository = prm_repository;
    }
    
    public async Task<string> function_crearCliente(...)
    {
        // Delega al Repository
        return await att_repository.function_crearCliente(...);
    }
}
```

**Ventajas:**
- Mejor separación de responsabilidades
- Facilita testing y mantenimiento
- Código más organizado

---

## 💬 4. MENSAJES DE RESPUESTA

### **Cambio en Mensaje de Cliente Creado:**

**Versión Anterior:**
```csharp
return Ok(new
{
    message = "Account successfully created",  // ← En inglés
    id_cliente = att_clienteId
});
```

**Versión Actual:**
```csharp
return Ok(new
{
    message = "Cliente creado exitosamente",  // ← En español y más descriptivo
    id_cliente = att_clienteId
});
```

---

## 💰 5. CAMBIO EN LÓGICA DE `service_cambiarSaldo`

### **Cambio Principal:**
El endpoint `service_cambiarSaldo` ahora **REEMPLAZA** el saldo en lugar de **SUMAR** el monto.

### **Archivo Modificado:**
- `BancoApi/Services/service_Cuenta.cs` - Método `function_cambiarSaldo`

### **Versión Anterior:**
```csharp
public async Task function_cambiarSaldo(model_CuentaCambiarSaldo req)
{
    using var conn = new OracleConnection(att_connectionString);
    await conn.OpenAsync();

    // Llamaba directamente al procedimiento que SUMA
    using var cmd = new OracleCommand("pkg_cuenta.cambiar_saldo", conn);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
    cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = req.monto;
    
    await cmd.ExecuteNonQueryAsync();
}
```

**Comportamiento:**
- Si la cuenta tenía saldo 100 y se enviaba monto 50, el nuevo saldo era **150** (sumaba)

### **Versión Actual:**
```csharp
public async Task function_cambiarSaldo(model_CuentaCambiarSaldo req)
{
    using var conn = new OracleConnection(att_connectionString);
    await conn.OpenAsync();

    // 1. Validar que la cuenta existe
    using (var cmdConsulta = new OracleCommand("pkg_cuenta.consultar_cuentas_por_id", conn))
    {
        // ... validación ...
    }

    // 2. Validar que el nuevo saldo no sea negativo
    if (req.monto < 0)
    {
        throw new ArgumentException("El saldo no puede ser negativo");
    }

    // 3. REEMPLAZAR el saldo con el valor ingresado
    using var cmdActualizar = new OracleCommand("pkg_cuenta.actualizar_cuenta", conn);
    cmdActualizar.CommandType = CommandType.StoredProcedure;
    cmdActualizar.Parameters.Add("p_id_cuenta", OracleDbType.Int32).Value = req.idCuenta;
    cmdActualizar.Parameters.Add("p_saldo", OracleDbType.Decimal).Value = req.monto;  // ← Reemplaza
    cmdActualizar.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = DBNull.Value;

    await cmdActualizar.ExecuteNonQueryAsync();
}
```

**Comportamiento:**
- Si la cuenta tenía saldo 100 y se envía monto 50, el nuevo saldo es **50** (reemplaza)

**Ejemplo de Uso:**
```json
// Request
{
  "idCuenta": 1003,
  "monto": 123
}

// Resultado: El saldo de la cuenta 1003 ahora es 123 (no se suma, se reemplaza)
```

---

## 📦 6. ACTUALIZACIÓN DE PAQUETES NUGET

### **Cambios en BancoApi.csproj:**

**Versión Anterior:**
```xml
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.0" />
```

**Versión Actual:**
```xml
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.3.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```

**Razón:** Actualización por seguridad y compatibilidad con .NET 8.

---

## 🔧 7. MEJORAS EN MANEJO DE ERRORES

### **Versión Anterior:**
```csharp
catch (Exception ex)
{
    return BadRequest(new { error = ex.Message });
}
```

### **Versión Actual:**
```csharp
catch (ArgumentException ex)  // ← Manejo específico
{
    return BadRequest(new { error = ex.Message });
}
catch (Exception ex)  // ← Manejo general
{
    return BadRequest(new { error = ex.Message });
}
```

**Ventajas:**
- Mejor diferenciación entre tipos de errores
- Mensajes más específicos para el usuario

---

## 📝 RESUMEN DE CAMBIOS

| Aspecto | Versión Anterior | Versión Actual |
|---------|------------------|----------------|
| **Autenticación** | ❌ No tenía | ✅ JWT implementado |
| **DTOs** | ❌ No tenía | ✅ Separados de Models |
| **Repository Pattern** | ❌ No tenía | ✅ Implementado |
| **Mensajes** | Inglés | Español |
| **cambiarSaldo** | Suma el monto | Reemplaza el saldo |
| **Paquetes NuGet** | Versiones antiguas | Actualizados a 8.x |

---

## 🚀 CÓMO MIGRAR DE VERSIÓN ANTERIOR A ACTUAL

1. **Actualizar Program.cs:**
   - Agregar configuración JWT
   - Agregar servicios nuevos (service_Autenticacion, service_Admin)
   - Agregar Repository_Cliente

2. **Actualizar Controllers:**
   - Cambiar `model_ClienteRequest` por `DTO_ClienteRequest`
   - Convertir DTO a Model antes de llamar al servicio

3. **Actualizar Services:**
   - Modificar `function_cambiarSaldo` para reemplazar saldo en lugar de sumar

4. **Instalar Paquetes NuGet:**
   ```bash
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
   dotnet add package System.IdentityModel.Tokens.Jwt --version 8.3.0
   ```

5. **Configurar appsettings.json:**
   ```json
   {
     "Jwt": {
       "Key": "tu_clave_secreta_aqui",
       "Issuer": "BancoApi",
       "Audience": "BancoWeb"
     }
   }
   ```

---

## ⚠️ NOTAS IMPORTANTES

- **Compatibilidad:** La versión actual requiere .NET 8.0
- **Base de Datos:** Los procedimientos almacenados siguen siendo los mismos, solo cambió la lógica en la API
- **Frontend:** Debe actualizarse para usar el nuevo endpoint de autenticación `/api/auth/login`
- **Testing:** Todos los endpoints deben probarse nuevamente debido a los cambios en la lógica

---

**Última actualización:** 16 de Noviembre de 2025

