using BancoApi.Services;
using BancoApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
// JWT
// =======================
builder.Services.AddScoped<service_JWT>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// =======================
// REPOSITORIES
// =======================
builder.Services.AddScoped<Repository_Cliente>();

// =======================
// SERVICES
// =======================
builder.Services.AddScoped<service_Cliente>();
builder.Services.AddScoped<service_Sucursal>();
builder.Services.AddScoped<service_Cajero>();
builder.Services.AddScoped<service_Cuenta>();
builder.Services.AddScoped<service_Autenticacion>();
builder.Services.AddScoped<service_Admin>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
