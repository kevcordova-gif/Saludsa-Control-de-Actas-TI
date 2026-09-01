using SaludsaActas.Application.Interfaces;
using SaludsaActas.Application.Services;
using SaludsaActas.Domain.Interfaces;
using SaludsaActas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaludsaActas.Infrastructure.Data;

namespace SaludsaActas.CrossCutting.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProjectDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<IActaRepository, ActaRepository>();

        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IActaService, ActaService>();

        return services;
    }
}