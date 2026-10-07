using System;

namespace CRM_Inmobiliaria_SGE.Models;

public class Proveedor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Cif { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty; // Notaría, Fotografía, Portales, Reformas
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}