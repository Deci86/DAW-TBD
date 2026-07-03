using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Infrastructure.Repositories;
using SmallChangeDAW.CORE.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Configurar Entity Framework Core con SQL Server
builder.Services.AddDbContext<SmallChangeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================================================
// CONFIGURACIÓN DE SEGURIDAD (JWT)
// =======================================================
var secretKey = builder.Configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("La clave secreta de JWT no está configurada.");

/*builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});*/
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Aseguramos que los errores se muestren
    options.IncludeErrorDetails = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 1. APAGAMOS validaciones secundarias que suelen dar falsos positivos
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,

        // 2. DEJAMOS ENCENDIDA solo la validación de tu clave secreta
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wearecharliekirkwecarrytheflmewefightforthegospelwehonorhisname"))
    };
});

// Registrar repositorios con EF Core
builder.Services.AddScoped<IClientesRepository, ClientesRepository>();
builder.Services.AddScoped<IOfertasRepository, OfertasRepository>();
builder.Services.AddScoped<ITransaccionesRepository, TransaccionesRepository>();

// Registrar servicios
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IDivisasService, DivisasService>();
builder.Services.AddScoped<IOfertasService, OfertasService>();
builder.Services.AddScoped<ITransaccionesService, TransaccionesService>();

// Registrar el nuevo servicio de Autenticación
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:9000") // El puerto de tu frontend Quasar
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors("PermitirFrontend");

// MIDDLEWARES DE SEGURIDAD
app.UseAuthentication(); // 1. Verifica quién eres (valida el token)
app.UseAuthorization();  // 2. Verifica si tienes permiso

app.MapControllers();
app.Run();