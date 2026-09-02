using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Application.Services;
using SaludsaActas.Application.Validators;
using SaludsaActas.Domain.Interfaces;
using SaludsaActas.Infrastructure.Data;
using SaludsaActas.Infrastructure.Documents;
using SaludsaActas.Infrastructure.Repositories;

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

        // Base de datos
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Configuración de documentos Word
        services.Configure<DocumentOptions>(
            configuration.GetSection("Documents"));

        // Repositories
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<IActaRepository, ActaRepository>();
        services.AddScoped<IActivoRepository, ActivoRepository>();
        services.AddScoped<IActaDraftRepository, ActaDraftRepository>();

        // Services
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IActaService, ActaService>();
        services.AddScoped<IActaDraftService, ActaDraftService>();
        services.AddScoped<IDocumentService, DocumentService>();

        // Validators
        services.AddScoped<
            IValidator<CreateEmpleadoDto>,
            CreateEmpleadoDtoValidator>();

        services.AddScoped<
            IValidator<CreateActaDto>,
            CreateActaDtoValidator>();

        return services;
    }
}