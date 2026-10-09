using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Views;

public class OperacionItemViewModel
{
    public Operacion Operacion { get; set; } = null!;
    public string InmuebleTexto { get; set; } = string.Empty;
    public string ClienteTexto { get; set; } = string.Empty;
}

public partial class VentasView : UserControl
{
    public VentasView()
    {
        InitializeComponent();
        CargarCombos();
        RefrescarLista();
    }

    private void CargarCombos()
    {
        // Solo inmuebles disponibles para cerrar nuevas operaciones
        CmbInmueblesVenta.ItemsSource = DataManager.Datos.Inmuebles
            .Where(i => i.Estado == EstadoInmueble.Disponible)
            .Select(i => $"{i.Referencia} - {i.Titulo} ({i.Precio:N0} €)")
            .ToList();

        CmbClientesVenta.ItemsSource = DataManager.Datos.Clientes
            .Select(c => $"{c.Nif} - {c.Nombre} {c.Apellidos}")
            .ToList();

        CmbTipoOperacion.ItemsSource = Enum.GetValues<TipoOperacion>();
        CmbTipoOperacion.SelectedIndex = 0;
    }

    private void RefrescarLista()
    {
        var lista = new List<OperacionItemViewModel>();

        foreach (var op in DataManager.Datos.Operaciones.OrderByDescending(o => o.FechaCierre))
        {
            var inm = DataManager.Datos.Inmuebles.FirstOrDefault(i => i.Id == op.InmuebleId);
            var cli = DataManager.Datos.Clientes.FirstOrDefault(c => c.Id == op.ClienteId);

            lista.Add(new OperacionItemViewModel
            {
                Operacion = op,
                InmuebleTexto = inm != null ? $"{inm.Referencia} - {inm.Titulo}" : "Inmueble histórico",
                ClienteTexto = cli != null ? $"Cliente: {cli.Nombre} {cli.Apellidos}" : "Cliente histórico"
            });
        }

        ListaOperaciones.ItemsSource = lista;
    }

    private void OnNuevaOperacionClick(object? sender, RoutedEventArgs e)
    {
        CargarCombos();
        TxtPrecioCierre.Text = string.Empty;
        TxtComision.Text = string.Empty;
    }

    private void OnGuardarOperacionClick(object? sender, RoutedEventArgs e)
    {
        var inmueblesDisponibles = DataManager.Datos.Inmuebles.Where(i => i.Estado == EstadoInmueble.Disponible).ToList();

        if (CmbInmueblesVenta.SelectedIndex < 0 || CmbInmueblesVenta.SelectedIndex >= inmueblesDisponibles.Count) return;
        if (CmbClientesVenta.SelectedIndex < 0 || CmbClientesVenta.SelectedIndex >= DataManager.Datos.Clientes.Count) return;

        var inmueble = inmueblesDisponibles[CmbInmueblesVenta.SelectedIndex];
        var cliente = DataManager.Datos.Clientes[CmbClientesVenta.SelectedIndex];
        var tipo = (TipoOperacion)(CmbTipoOperacion.SelectedItem ?? TipoOperacion.Venta);

        decimal.TryParse(TxtPrecioCierre.Text, out var precioFinal);
        if (precioFinal <= 0) precioFinal = inmueble.Precio;

        decimal.TryParse(TxtComision.Text, out var comision);
        if (comision <= 0) comision = Math.Round(precioFinal * 0.03m, 2); // 3% comisión habitual

        var nuevaOperacion = new Operacion
        {
            Id = Guid.NewGuid(),
            Codigo = $"OP-{DateTime.Now.Year}-{DataManager.Datos.Operaciones.Count + 1:D3}",
            InmuebleId = inmueble.Id,
            ClienteId = cliente.Id,
            AgenteId = DataManager.UsuarioActual?.Id ?? Guid.Empty,
            Tipo = tipo,
            ImporteFinal = precioFinal,
            ComisionHonorarios = comision,
            FechaCierre = DateTime.UtcNow
        };

        // Actualizar el estado del inmueble en el catálogo (stock/disponibilidad)
        inmueble.Estado = tipo == TipoOperacion.Venta ? EstadoInmueble.Vendido : EstadoInmueble.Alquilado;

        DataManager.Datos.Operaciones.Add(nuevaOperacion);
        DataManager.GuardarDatos();

        CargarCombos();
        RefrescarLista();
        TxtPrecioCierre.Text = string.Empty;
        TxtComision.Text = string.Empty;
    }

    private void OnEliminarOperacionClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is OperacionItemViewModel item)
        {
            DataManager.Datos.Operaciones.Remove(item.Operacion);
            DataManager.GuardarDatos();
            CargarCombos();
            RefrescarLista();
        }
    }
}