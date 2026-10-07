using System;

namespace CRM_Inmobiliaria_SGE.Models;

public class Factura
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NumeroFactura { get; set; } = string.Empty; // Ej: 2026-F001
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public Guid? ClienteId { get; set; }
    public Guid? ProveedorId { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public decimal BaseImponible { get; set; }
    public decimal TipoIva { get; set; } = 21.0m; // 21% habitual en servicios
    public decimal CuotaIva => Math.Round(BaseImponible * (TipoIva / 100m), 2);
    public decimal Total => BaseImponible + CuotaIva;
    public bool EsGastoProveedor { get; set; } // true = Factura recibida de gasto, false = Factura emitida de honorarios
}