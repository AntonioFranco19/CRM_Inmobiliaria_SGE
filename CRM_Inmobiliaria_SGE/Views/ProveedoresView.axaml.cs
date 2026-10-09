using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Views;

public partial class ProveedoresView : UserControl
{
    private Proveedor? _proveedorEnEdicion = null;

    public ProveedoresView()
    {
        InitializeComponent();
        RefrescarLista();
    }

    private void RefrescarLista(string? filtro = null)
    {
        var provs = DataManager.Datos.Proveedores.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.Trim().ToLowerInvariant();
            provs = provs.Where(p =>
                p.Cif.ToLowerInvariant().Contains(texto) ||
                p.NombreComercial.ToLowerInvariant().Contains(texto) ||
                p.Sector.ToLowerInvariant().Contains(texto));
        }

        ListaProveedores.ItemsSource = provs.OrderBy(p => p.NombreComercial).ToList();
    }

    private void OnBuscarKeyUp(object? sender, KeyEventArgs e)
    {
        RefrescarLista(TxtBuscarProveedor.Text);
    }

    private void OnProveedorSeleccionadoChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ListaProveedores.SelectedItem is Proveedor p)
        {
            _proveedorEnEdicion = p;
            TxtTituloFormulario.Text = $"Editar: {p.NombreComercial}";
            TxtCif.Text = p.Cif;
            TxtNombreComercial.Text = p.NombreComercial;
            TxtSector.Text = p.Sector;
            TxtTelefono.Text = p.Telefono;
        }
    }

    private void OnNuevoProveedorClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        TxtCif.Focus();
    }

    private void OnLimpiarClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        _proveedorEnEdicion = null;
        ListaProveedores.SelectedItem = null;
        TxtTituloFormulario.Text = "Añadir Nuevo Proveedor";
        TxtCif.Text = string.Empty;
        TxtNombreComercial.Text = string.Empty;
        TxtSector.Text = string.Empty;
        TxtTelefono.Text = string.Empty;
    }

    private void OnGuardarProveedorClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtCif.Text) || string.IsNullOrWhiteSpace(TxtNombreComercial.Text)) return;

        if (_proveedorEnEdicion == null)
        {
            var nuevo = new Proveedor
            {
                Id = Guid.NewGuid(),
                Cif = TxtCif.Text.Trim(),
                NombreComercial = TxtNombreComercial.Text.Trim(),
                Sector = TxtSector.Text?.Trim() ?? "General",
                Telefono = TxtTelefono.Text?.Trim() ?? string.Empty
            };
            DataManager.Datos.Proveedores.Add(nuevo);
        }
        else
        {
            _proveedorEnEdicion.Cif = TxtCif.Text.Trim();
            _proveedorEnEdicion.NombreComercial = TxtNombreComercial.Text.Trim();
            _proveedorEnEdicion.Sector = TxtSector.Text?.Trim() ?? "General";
            _proveedorEnEdicion.Telefono = TxtTelefono.Text?.Trim() ?? string.Empty;
        }

        DataManager.GuardarDatos();
        RefrescarLista(TxtBuscarProveedor.Text);
        LimpiarFormulario();
    }

    private void OnEliminarProveedorClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Proveedor p)
        {
            DataManager.Datos.Proveedores.Remove(p);
            DataManager.GuardarDatos();
            RefrescarLista(TxtBuscarProveedor.Text);
            LimpiarFormulario();
        }
    }
}