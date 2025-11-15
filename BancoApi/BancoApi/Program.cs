using BancoApi.Services;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

// =======================
// CONTROLLERS
// =======================
builder.Services.AddControllers();

// =======================
// SWAGGER
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =======================
// SERVICES
// =======================
builder.Services.AddScoped<service_Cliente>();
builder.Services.AddScoped<service_Sucursal>();  // <- ÚNICO QUE NECESITAS
builder.Services.AddScoped<service_Cajero>();
builder.Services.AddScoped<service_Cuenta>();





// =======================
// CORS
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// =======================
// SWAGGER
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
