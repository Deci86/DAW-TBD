using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Infrastructure.Repositories;
using SmallChangeDAW.CORE.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddSingleton(sp =>
    new DbConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddScoped<IClientesRepository, ClientesRepository>();
builder.Services.AddScoped<IOfertasRepository, OfertasRepository>();
builder.Services.AddScoped<ITransaccionesRepository, TransaccionesRepository>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IOfertasService, OfertasService>();
builder.Services.AddScoped<ITransaccionesService, TransaccionesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
