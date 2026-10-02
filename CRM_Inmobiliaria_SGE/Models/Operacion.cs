using System;
using InmoCRM.Models.Enums;

namespace InmoCRM.Models;

public class Operacion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Codigo { get; set; } = string.Empty;
    public Guid InmuebleId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid AgenteId { get; set; }
    public TipoOperacion Tipo { get; set; }
    public decimal ImporteFinal { get; set; }
    public decimal ComisionHonorarios { get; set; }
    public DateTime FechaCierre { get; set; } = DateTime.UtcNow;
}