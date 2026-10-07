// Models/Inmueble.cs
using System;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Models;

public class Inmueble
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Referencia { get; set; } = string.Empty; // Ej: INM-00124
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TipoInmueble Tipo { get; set; }
    public EstadoInmueble Estado { get; set; } = EstadoInmueble.Disponible;
    public decimal Precio { get; set; }
    public int MetrosCuadrados { get; set; }
    public int Habitaciones { get; set; }
    public int Banos { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public bool Ascensor { get; set; }
    public bool Garaje { get; set; }
    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
}