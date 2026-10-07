// Models/Cliente.cs
using System;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Models;

public class Cliente
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nif { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Notas { get; set; } = string.Empty;

    // Preferencias de búsqueda para el motor de matching inmobiliario
    public TipoInmueble? TipoInteres { get; set; }
    public decimal PresupuestoMaximo { get; set; }
    public int MinHabitaciones { get; set; }
    public string? ZonaInteres { get; set; }
}