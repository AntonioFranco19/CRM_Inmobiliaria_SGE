using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;


namespace CRM_Inmobiliaria_SGE.Services;

public class JsonStorageService : IJsonStorageService
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonStorageService()
    {
        var appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "InmoCRM"
        );
        Directory.CreateDirectory(appFolder);
        _filePath = Path.Combine(appFolder, "inmocrm_data.json");
    }

    public async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
            {
                var seedData = GenerarDatosSemilla();
                var json = JsonSerializer.Serialize(seedData, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<DatabaseJson> LoadDataAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
            {
                return GenerarDatosSemilla();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<DatabaseJson>(json, _jsonOptions) ?? GenerarDatosSemilla();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveDataAsync(DatabaseJson data)
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static DatabaseJson GenerarDatosSemilla()
    {
        return new DatabaseJson
        {
            Usuarios =
            [
                new Usuario
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Username = "admin",
                    PasswordHash = "admin123",
                    NombreCompleto = "Administrador Principal",
                    Rol = RolUsuario.Administrador,
                    Activo = true
                },
                new Usuario
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Username = "agente",
                    PasswordHash = "agente123",
                    NombreCompleto = "Carlos Agente",
                    Rol = RolUsuario.Agente,
                    Activo = true
                }
            ],
            Inmuebles =
            [
                new Inmueble
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333331"),
                    Referencia = "INM-001",
                    Titulo = "Ático en primera línea de playa con terraza",
                    Descripcion = "Ático reformado con vistas panorámicas al mar y terraza de 40 m².",
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
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333332"),
                    Referencia = "INM-002",
                    Titulo = "Piso céntrico luminoso cerca de servicios",
                    Descripcion = "Segundo piso con ascensor, balcón exterior y cocina equipada.",
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
            ],
            Clientes =
            [
                new Cliente
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444441"),
                    Nif = "12345678Z",
                    Nombre = "Elena",
                    Apellidos = "García Ruiz",
                    Telefono = "654987321",
                    Email = "elena.garcia@example.com",
                    Notas = "Busca ático o piso con terraza en Águilas.",
                    TipoInteres = TipoInmueble.Atico,
                    PresupuestoMaximo = 230000m,
                    MinHabitaciones = 2,
                    ZonaInteres = "Águilas"
                }
            ]
        };
    }
}