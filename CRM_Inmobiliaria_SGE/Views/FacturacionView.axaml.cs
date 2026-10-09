using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Views;

public class FacturaItemViewModel
{
    public Factura Factura { get; set; } = null!;
    public string DestinatarioTexto { get; set; } = string.Empty;
}

public partial class FacturacionView : UserControl
{
    public FacturacionView()
    {
        InitializeComponent();
        CargarClientes();
        RefrescarLista();
        GenerarNumeroFactura();
    }

    private void CargarClientes()
    {
        CmbClientesFactura.ItemsSource = DataManager.Datos.Clientes
            .Select(c => $"{c.Nif} - {c.Nombre} {c.Apellidos}")
            .ToList();
        if (DataManager.Datos.Clientes.Any()) CmbClientesFactura.SelectedIndex = 0;
    }

    private void GenerarNumeroFactura()
    {
        TxtNumeroFactura.Text = $"{DateTime.Now.Year}-F{DataManager.Datos.Facturas.Count + 1:D3}";
    }

    private void RefrescarLista()
    {
        var lista = new List<FacturaItemViewModel>();

        foreach (var f in DataManager.Datos.Facturas.OrderByDescending(f => f.FechaEmision))
        {
            var cli = DataManager.Datos.Clientes.FirstOrDefault(c => c.Id == f.ClienteId);
            lista.Add(new FacturaItemViewModel
            {
                Factura = f,
                DestinatarioTexto = cli != null ? $"Cliente: {cli.Nombre} {cli.Apellidos} ({cli.Nif})" : "Factura General"
            });
        }

        ListaFacturas.ItemsSource = lista;
    }

    private void OnBaseImponibleKeyUp(object? sender, KeyEventArgs e)
    {
        CalcularTotalesVisuales();
    }

    private void CalcularTotalesVisuales()
    {
        if (decimal.TryParse(TxtBaseImponible.Text, out var baseImp) && baseImp >= 0)
        {
            var iva = Math.Round(baseImp * 0.21m, 2);
            var total = baseImp + iva;

            TxtResumenBase.Text = $"Base: {baseImp:N2} €";
            TxtResumenIva.Text = $"IVA (21%): {iva:N2} €";
            TxtResumenTotal.Text = $"Total a Facturar: {total:N2} €";
        }
        else
        {
            TxtResumenBase.Text = "Base: 0,00 €";
            TxtResumenIva.Text = "IVA (21%): 0,00 €";
            TxtResumenTotal.Text = "Total a Facturar: 0,00 €";
        }
    }

    private void OnNuevaFacturaClick(object? sender, RoutedEventArgs e)
    {
        GenerarNumeroFactura();
        TxtConcepto.Text = string.Empty;
        TxtBaseImponible.Text = string.Empty;
        CalcularTotalesVisuales();
    }

    private void OnGuardarFacturaClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNumeroFactura.Text) || string.IsNullOrWhiteSpace(TxtConcepto.Text)) return;
        if (!decimal.TryParse(TxtBaseImponible.Text, out var baseImp) || baseImp <= 0) return;

        Guid? clienteId = null;
        if (CmbClientesFactura.SelectedIndex >= 0 && CmbClientesFactura.SelectedIndex < DataManager.Datos.Clientes.Count)
        {
            clienteId = DataManager.Datos.Clientes[CmbClientesFactura.SelectedIndex].Id;
        }

        var nuevaFactura = new Factura
        {
            Id = Guid.NewGuid(),
            NumeroFactura = TxtNumeroFactura.Text.Trim(),
            FechaEmision = DateTime.UtcNow,
            ClienteId = clienteId,
            Concepto = TxtConcepto.Text.Trim(),
            BaseImponible = baseImp,
            TipoIva = 21.0m,
            EsGastoProveedor = false
        };

        DataManager.Datos.Facturas.Add(nuevaFactura);
        DataManager.GuardarDatos();

        RefrescarLista();
        OnNuevaFacturaClick(this, new RoutedEventArgs());
    }

    private void OnEliminarFacturaClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is FacturaItemViewModel item)
        {
            DataManager.Datos.Facturas.Remove(item.Factura);
            DataManager.GuardarDatos();
            RefrescarLista();
        }
    }
}