using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Views;

public partial class ClientesView : UserControl
{
    private Cliente? _clienteEnEdicion = null;

    public ClientesView()
    {
        InitializeComponent();
        CargarCombos();
        RefrescarLista();
    }

    private void CargarCombos()
    {
        CmbTipoInteres.ItemsSource = Enum.GetValues<TipoInmueble>();
        CmbTipoInteres.SelectedIndex = 0;
    }

    private void RefrescarLista(string? filtro = null)
    {
        var clientes = DataManager.Datos.Clientes.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.Trim().ToLowerInvariant();
            clientes = clientes.Where(c =>
                c.Nif.ToLowerInvariant().Contains(texto) ||
                c.Nombre.ToLowerInvariant().Contains(texto) ||
                c.Apellidos.ToLowerInvariant().Contains(texto) ||
                c.Telefono.ToLowerInvariant().Contains(texto) ||
                (!string.IsNullOrEmpty(c.ZonaInteres) && c.ZonaInteres.ToLowerInvariant().Contains(texto)));
        }

        ListaClientes.ItemsSource = clientes.OrderBy(c => c.Apellidos).ThenBy(c => c.Nombre).ToList();
    }

    private void OnBuscarClienteClick(object? sender, RoutedEventArgs e)
    {
        RefrescarLista(TxtBuscarCliente.Text);
    }

    private void OnBuscarClienteKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            RefrescarLista(TxtBuscarCliente.Text);
        }
    }

    private void OnClienteSeleccionadoChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ListaClientes.SelectedItem is Cliente seleccionado)
        {
            _clienteEnEdicion = seleccionado;
            TxtTituloFormulario.Text = $"Editar Cliente: {seleccionado.Nombre} {seleccionado.Apellidos}";

            TxtNif.Text = seleccionado.Nif;
            TxtNombre.Text = seleccionado.Nombre;
            TxtApellidos.Text = seleccionado.Apellidos;
            TxtTelefono.Text = seleccionado.Telefono;
            TxtEmail.Text = seleccionado.Email;
            TxtPresupuesto.Text = seleccionado.PresupuestoMaximo > 0 ? seleccionado.PresupuestoMaximo.ToString("0") : "";
            TxtZonaInteres.Text = seleccionado.ZonaInteres ?? "";

            if (seleccionado.TipoInteres.HasValue)
            {
                CmbTipoInteres.SelectedItem = seleccionado.TipoInteres.Value;
            }
        }
    }

    private void OnNuevoClienteClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        TxtNif.Focus();
    }

    private void OnLimpiarFormularioClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        _clienteEnEdicion = null;
        ListaClientes.SelectedItem = null;
        TxtTituloFormulario.Text = "Añadir Nuevo Cliente";

        TxtNif.Text = string.Empty;
        TxtNombre.Text = string.Empty;
        TxtApellidos.Text = string.Empty;
        TxtTelefono.Text = string.Empty;
        TxtEmail.Text = string.Empty;
        TxtPresupuesto.Text = string.Empty;
        TxtZonaInteres.Text = string.Empty;
        CmbTipoInteres.SelectedIndex = 0;
    }

    private void OnGuardarClienteClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtTelefono.Text))
        {
            return;
        }

        decimal.TryParse(TxtPresupuesto.Text, out var presupuesto);
        var tipoInteres = CmbTipoInteres.SelectedItem is TipoInmueble tipo ? (TipoInmueble?)tipo : null;

        if (_clienteEnEdicion == null)
        {
            var nuevo = new Cliente
            {
                Id = Guid.NewGuid(),
                Nif = TxtNif.Text?.Trim() ?? string.Empty,
                Nombre = TxtNombre.Text.Trim(),
                Apellidos = TxtApellidos.Text?.Trim() ?? string.Empty,
                Telefono = TxtTelefono.Text.Trim(),
                Email = TxtEmail.Text?.Trim() ?? string.Empty,
                PresupuestoMaximo = presupuesto,
                TipoInteres = tipoInteres,
                ZonaInteres = TxtZonaInteres.Text?.Trim()
            };

            DataManager.Datos.Clientes.Add(nuevo);
        }
        else
        {
            _clienteEnEdicion.Nif = TxtNif.Text?.Trim() ?? string.Empty;
            _clienteEnEdicion.Nombre = TxtNombre.Text.Trim();
            _clienteEnEdicion.Apellidos = TxtApellidos.Text?.Trim() ?? string.Empty;
            _clienteEnEdicion.Telefono = TxtTelefono.Text.Trim();
            _clienteEnEdicion.Email = TxtEmail.Text?.Trim() ?? string.Empty;
            _clienteEnEdicion.PresupuestoMaximo = presupuesto;
            _clienteEnEdicion.TipoInteres = tipoInteres;
            _clienteEnEdicion.ZonaInteres = TxtZonaInteres.Text?.Trim();
        }

        DataManager.GuardarDatos();
        RefrescarLista(TxtBuscarCliente.Text);
        LimpiarFormulario();
    }

    private void OnEliminarClienteClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Cliente cli)
        {
            DataManager.Datos.Clientes.Remove(cli);
            DataManager.GuardarDatos();
            RefrescarLista(TxtBuscarCliente.Text);
            LimpiarFormulario();
        }
    }
}