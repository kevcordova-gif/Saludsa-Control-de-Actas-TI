using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Application.Services;
using SaludsaActas.Application.Validators;
using SaludsaActas.Domain.Interfaces;
using SaludsaActas.Infrastructure.ActiveDirectory;
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

        // Configuración de Active Directory
        services.Configure<ActiveDirectoryOptions>(
            configuration.GetSection("ActiveDirectory"));

        // Repositories
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<IActaRepository, ActaRepository>();
        services.AddScoped<IActivoRepository, ActivoRepository>();
        services.AddScoped<IActaDraftRepository, ActaDraftRepository>();

        // Services
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IActaService, ActaService>();
        services.AddScoped<IActaDraftService, ActaDraftService>();

        // Documentos
        services.AddScoped<IDocumentService, DocumentService>();

        services.AddScoped<
            IDiscountDocumentService,
            DiscountDocumentService>();

        // Active Directory
        services.AddScoped<
            IActiveDirectoryService,
            ActiveDirectoryService>();

        // Validators
        services.AddScoped<
            IValidator<CreateEmpleadoDto>,
            CreateEmpleadoDtoValidator>();

        services.AddScoped<
            IValidator<CreateActaDto>,
            CreateActaDtoValidator>();

        services.AddScoped<
            IValidator<CreateDiscountDocumentDto>,
            CreateDiscountDocumentDtoValidator>();

        return services;
    }
}