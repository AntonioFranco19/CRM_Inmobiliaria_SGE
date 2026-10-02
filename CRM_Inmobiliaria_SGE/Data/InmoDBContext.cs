// Data/InmoDbContext.cs
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using InmoCRM.Models;
using InmoCRM.Models.Enums;

namespace InmoCRM.Data;

public class InmoDbContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Inmueble> Inmuebles => Set<Inmueble>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Visita> Visitas => Set<Visita>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Operacion> Operaciones => Set<Operacion>();
    public DbSet<Factura> Facturas => Set<Factura>();

    public string DbPath { get; }

    public InmoDbContext()
    {
        // Guardamos la BD en la carpeta de datos de la aplicación local
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var path = Path.Combine(folder, "InmoCRM");
        Directory.CreateDirectory(path);
        DbPath = Path.Combine(path, "inmocrm.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Claves e índices
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Inmueble>()
            .HasIndex(i => i.Referencia)
            .IsUnique();

        modelBuilder.Entity<Factura>()
            .HasIndex(f => f.NumeroFactura)
            .IsUnique();

        // Datos semilla iniciales (Seeds)
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var agenteId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Usuarios iniciales (Admin y Agente)
        // Nota: En producción las contraseñas se hashean con BCrypt / PBKDF2. Aquí usamos un hash simple de prueba.
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = adminId,
                Username = "admin",
                PasswordHash = "admin123", // Contraseña para pruebas
                NombreCompleto = "Administrador Principal",
                Rol = RolUsuario.Administrador,
                Activo = true
            },
            new Usuario
            {
                Id = agenteId,
                Username = "agente",
                PasswordHash = "agente123", // Contraseña para pruebas
                NombreCompleto = "Carlos Vendedor",
                Rol = RolUsuario.Agente,
                Activo = true
            }
        );

        // Inmuebles iniciales
        var inm1Id = Guid.Parse("33333333-3333-3333-3333-333333333331");
        var inm2Id = Guid.Parse("33333333-3333-3333-3333-333333333332");

        modelBuilder.Entity<Inmueble>().HasData(
            new Inmueble
            {
                Id = inm1Id,
                Referencia = "INM-001",
                Titulo = "Ático en primera línea de playa con terraza",
                Descripcion = "Magnífico ático reformado, vistas directas al mar y terraza de 40m².",
                Tipo = TipoInmueble.Atico,
                Estado = EstadoInmueble.Disponible,
                Precio = 215000m,
                MetrosCuadrados = 95,
                Habitaciones = 3,
                Banos = 2,
                Direccion = "Paseo Marítimo 14",
                Ciudad = "Águilas",
                Ascensor = true,
                Garaje = true,
                FechaAlta = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new Inmueble
            {
                Id = inm2Id,
                Referencia = "INM-002",
                Titulo = "Piso céntrico luminoso cerca de servicios",
                Descripcion = "Piso espacioso, 2º con ascensor, ideal familias o inversión.",
                Tipo = TipoInmueble.Piso,
                Estado = EstadoInmueble.Disponible,
                Precio = 135000m,
                MetrosCuadrados = 80,
                Habitaciones = 2,
                Banos = 1,
                Direccion = "Calle Mayor 45",
                Ciudad = "Águilas",
                Ascensor = true,
                Garaje = false,
                FechaAlta = new DateTime(2026, 2, 1, 12, 0, 0, DateTimeKind.Utc)
            }
        );

        // Clientes iniciales
        var cli1Id = Guid.Parse("44444444-4444-4444-4444-444444444441");
        modelBuilder.Entity<Cliente>().HasData(
            new Cliente
            {
                Id = cli1Id,
                Nif = "12345678Z",
                Nombre = "Elena",
                Apellidos = "García Ruiz",
                Telefono = "654987321",
                Email = "elena.garcia@example.com",
                Notas = "Busca piso o ático para vivir todo el año.",
                TipoInteres = TipoInmueble.Atico,
                PresupuestoMaximo = 230000m,
                MinHabitaciones = 2,
                ZonaInteres = "Águilas"
            }
        );
    }
}