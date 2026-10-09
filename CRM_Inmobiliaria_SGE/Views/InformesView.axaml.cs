using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Views;

// Clases de soporte para la agregación de datos
public class VentasMesReportItem
{
    public string Periodo { get; set; } = string.Empty;
    public int OperacionesCount { get; set; }
    public decimal TotalHonorarios { get; set; }
}

public class TipoEstrellaReportItem
{
    public string TipoNombre { get; set; } = string.Empty;
    public int UnidadesVendidas { get; set; }
    public decimal VolumenEuros { get; set; }
}

public class TopClienteReportItem
{
    public string NombreCompleto { get; set; } = string.Empty;
    public int VisitasRealizadas { get; set; }
    public decimal ImporteTotalInvertido { get; set; }
}

public partial class InformesView : UserControl
{
    public InformesView()
    {
        InitializeComponent();
        GenerarInformes();
    }

    private void OnActualizarInformesClick(object? sender, RoutedEventArgs e)
    {
        GenerarInformes();
    }

    private void GenerarInformes()
    {
        // 1. INFORME: Ventas agrupadas por Mes
        var informeMes = DataManager.Datos.Operaciones
            .GroupBy(o => new { o.FechaCierre.Year, o.FechaCierre.Month })
            .Select(g => new VentasMesReportItem
            {
                Periodo = $"{g.Key.Month:D2}/{g.Key.Year}",
                OperacionesCount = g.Count(),
                TotalHonorarios = g.Sum(o => o.ComisionHonorarios)
            })
            .OrderByDescending(r => r.Periodo)
            .ToList();

        if (!informeMes.Any())
        {
            informeMes.Add(new VentasMesReportItem { Periodo = "Sin ventas", OperacionesCount = 0, TotalHonorarios = 0 });
        }
        ListaVentasMes.ItemsSource = informeMes;

        // 2. INFORME: Inmueble / Tipo Estrella
        var operaciones = DataManager.Datos.Operaciones;
        var inmuebles = DataManager.Datos.Inmuebles;

        var informeTipos = System.Enum.GetValues<TipoInmueble>()
            .Select(tipo =>
            {
                var opsDelTipo = operaciones.Where(op =>
                {
                    var inm = inmuebles.FirstOrDefault(i => i.Id == op.InmuebleId);
                    return inm != null && inm.Tipo == tipo;
                }).ToList();

                return new TipoEstrellaReportItem
                {
                    TipoNombre = tipo.ToString(),
                    UnidadesVendidas = opsDelTipo.Count,
                    VolumenEuros = opsDelTipo.Sum(o => o.ImporteFinal)
                };
            })
            .OrderByDescending(t => t.UnidadesVendidas)
            .ThenByDescending(t => t.VolumenEuros)
            .ToList();

        ListaTiposEstrella.ItemsSource = informeTipos;

        // 3. INFORME: Mejores Clientes
        var clientes = DataManager.Datos.Clientes;
        var visitas = DataManager.Datos.Visitas;

        var informeClientes = clientes.Select(c =>
        {
            var opsCliente = operaciones.Where(o => o.ClienteId == c.Id);
            var visitasCliente = visitas.Count(v => v.ClienteId == c.Id && v.Realizada);

            return new TopClienteReportItem
            {
                NombreCompleto = $"{c.Nombre} {c.Apellidos}",
                VisitasRealizadas = visitasCliente,
                ImporteTotalInvertido = opsCliente.Sum(o => o.ImporteFinal)
            };
        })
        .OrderByDescending(c => c.ImporteTotalInvertido)
        .ThenByDescending(c => c.VisitasRealizadas)
        .Take(10)
        .ToList();

        ListaTopClientes.ItemsSource = informeClientes;
    }
}