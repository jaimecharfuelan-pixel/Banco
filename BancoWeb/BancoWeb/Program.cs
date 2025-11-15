var builder = WebApplication.CreateBuilder(args);

// HABILITAR CORS (para permitir llamadas a la API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowAll");   // ? ACTIVAR CORS

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
