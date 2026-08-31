using Microsoft.EntityFrameworkCore;
using SaludsaActas.Domain.Entities;

namespace SaludsaActas.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Activo> Activos => Set<Activo>();
    public DbSet<Accesorio> Accesorios => Set<Accesorio>();
    public DbSet<Acta> Actas => Set<Acta>();
    public DbSet<ActaDraft> ActaDrafts => Set<ActaDraft>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.ToTable("empleados");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.Property(e => e.FullName)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(e => e.NationalId)
                .HasMaxLength(20);

            entity.HasIndex(e => e.NationalId);

            entity.Property(e => e.City)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Activo>(entity =>
        {
            entity.ToTable("activos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Manufacturer)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Model)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.SerialNumber)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(e => e.SerialNumber)
                .IsUnique();

            entity.Property(e => e.Hostname)
                .HasMaxLength(150);

            entity.HasIndex(e => e.Hostname);

            entity.Property(e => e.PurchaseCost)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.Location)
                .HasMaxLength(150);

            entity.Property(e => e.Observation)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Accesorio>(entity =>
        {
            entity.ToTable("accesorios");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.EquipmentType)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Manufacturer)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Model)
                .HasMaxLength(150);

            entity.Property(e => e.SerialNumber)
                .HasMaxLength(150);

            entity.Property(e => e.PurchaseCost)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.Location)
                .HasMaxLength(150);

            entity.Property(e => e.Observation)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Acta>(entity =>
        {
            entity.ToTable("actas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasMaxLength(50);

            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.EstadoSincronizacion)
                .HasMaxLength(50);

            entity.Property(e => e.ArchivoActa)
                .HasMaxLength(500);

            entity.Property(e => e.ArchivoPagare)
                .HasMaxLength(500);

            entity.HasOne(e => e.Empleado)
                .WithMany(e => e.Actas)
                .HasForeignKey(e => e.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Activos)
                .WithMany(e => e.Actas)
                .UsingEntity(j => j.ToTable("acta_activos"));

            entity.HasMany(e => e.Accesorios)
                .WithMany(e => e.Actas)
                .UsingEntity(j => j.ToTable("acta_accesorios"));
        });

        modelBuilder.Entity<ActaDraft>(entity =>
        {
            entity.ToTable("acta_drafts");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Titulo)
                .HasMaxLength(250);

            entity.Property(e => e.UsuarioJson)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.EquiposJson)
                .HasColumnType("nvarchar(max)");
        });
    }
}