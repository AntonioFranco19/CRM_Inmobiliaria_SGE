// Models/Visita.cs
using System;
namespace CRM_Inmobiliaria_SGE.Models;

public class Visita
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InmuebleId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid AgenteId { get; set; }
    public DateTime FechaHora { get; set; }
    public string ComentariosCliente { get; set; } = string.Empty;
    public int InteresEstimado { get; set; } // 1 a 5
    public bool Realizada { get; set; }
}