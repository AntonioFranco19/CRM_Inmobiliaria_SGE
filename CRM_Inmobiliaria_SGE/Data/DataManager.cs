// Data/DataManager.cs
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Data;

public static class DataManager
{
    private static readonly string FilePath;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static DatabaseJson Datos { get; set; } = new();
    public static Usuario? UsuarioActual { get; set; }

    static DataManager()
    {
        var appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "InmoCRM"
        );
        Directory.CreateDirectory(appFolder);
        FilePath = Path.Combine(appFolder, "inmocrm_data.json");
    }

    public static void CargarDatos()
{
    try
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            Datos = JsonSerializer.Deserialize<DatabaseJson>(json, JsonOptions) ?? GenerarDatosIniciales();
        }
        else
        {
            Datos = GenerarDatosIniciales();
            GuardarDatos();
        }
    }
    catch
    {
        // Si el archivo está corrupto o da fallo de I/O, arrancamos con datos base limpios
        Datos = GenerarDatosIniciales();
    }
}

public static void GuardarDatos()
{
    var json = JsonSerializer.Serialize(Datos, JsonOptions);
    File.WriteAllText(FilePath, json);
}
    private static DatabaseJson GenerarDatosIniciales()
    {
        return new DatabaseJson
        {
            Usuarios =
            [
                new Usuario
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = "admin123",
                    NombreCompleto = "Administrador Principal",
                    Rol = RolUsuario.Administrador,
                    Activo = true
                },
                new Usuario
                {
                    Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
                    Referencia = "INM-001",
                    Titulo = "Ático en primera línea con terraza",
                    Tipo = TipoInmueble.Atico,
                    Estado = EstadoInmueble.Disponible,
                    Precio = 215000m,
                    MetrosCuadrados = 95,
                    Habitaciones = 3,
                    Banos = 2,
                    Direccion = "Paseo Marítimo 14",
                    Ciudad = "Águilas"
                },
                new Inmueble
                {
                    Id = Guid.NewGuid(),
                    Referencia = "INM-002",
                    Titulo = "Piso céntrico luminoso",
                    Tipo = TipoInmueble.Piso,
                    Estado = EstadoInmueble.Disponible,
                    Precio = 135000m,
                    MetrosCuadrados = 80,
                    Habitaciones = 2,
                    Banos = 1,
                    Direccion = "Calle Mayor 45",
                    Ciudad = "Águilas"
                }
            ],
            Clientes =
            [
                new Cliente
                {
                    Id = Guid.NewGuid(),
                    Nif = "12345678Z",
                    Nombre = "Elena",
                    Apellidos = "García Ruiz",
                    Telefono = "654987321",
                    Email = "elena@example.com",
                    TipoInteres = TipoInmueble.Atico,
                    PresupuestoMaximo = 230000m,
                    MinHabitaciones = 2,
                    ZonaInteres = "Águilas"
                }
            ]
        };
    }
}