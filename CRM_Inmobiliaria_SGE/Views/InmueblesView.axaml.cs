using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Views;

public partial class InmueblesView : UserControl
{
    private Inmueble? _inmuebleEnEdicion = null;

    public InmueblesView()
    {
        InitializeComponent();
        CargarCombos();
        RefrescarLista();
    }

    private void CargarCombos()
    {
        CmbTipo.ItemsSource = Enum.GetValues<TipoInmueble>();
        CmbTipo.SelectedIndex = 0;

        CmbEstado.ItemsSource = Enum.GetValues<EstadoInmueble>();
        CmbEstado.SelectedIndex = 0;
    }

    private void RefrescarLista(string? filtro = null)
    {
        var inmuebles = DataManager.Datos.Inmuebles.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.Trim().ToLowerInvariant();
            inmuebles = inmuebles.Where(i =>
                i.Referencia.ToLowerInvariant().Contains(texto) ||
                i.Titulo.ToLowerInvariant().Contains(texto) ||
                i.Ciudad.ToLowerInvariant().Contains(texto));
        }

        ListaInmuebles.ItemsSource = inmuebles.OrderByDescending(i => i.FechaAlta).ToList();
    }

    private void OnBuscarClick(object? sender, RoutedEventArgs e)
    {
        RefrescarLista(TxtBuscar.Text);
    }

    private void OnBuscarKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            RefrescarLista(TxtBuscar.Text);
        }
    }

    private void OnInmuebleSeleccionadoChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ListaInmuebles.SelectedItem is Inmueble seleccionado)
        {
            _inmuebleEnEdicion = seleccionado;
            TxtTituloFormulario.Text = $"Editar Inmueble: {seleccionado.Referencia}";

            TxtReferencia.Text = seleccionado.Referencia;
            TxtTitulo.Text = seleccionado.Titulo;
            TxtPrecio.Text = seleccionado.Precio.ToString("0");
            TxtCiudad.Text = seleccionado.Ciudad;
            TxtHabitaciones.Text = seleccionado.Habitaciones.ToString();

            CmbTipo.SelectedItem = seleccionado.Tipo;
            CmbEstado.SelectedItem = seleccionado.Estado;
        }
    }

    private void OnNuevoClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        TxtReferencia.Focus();
    }

    private void OnLimpiarFormularioClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        _inmuebleEnEdicion = null;
        ListaInmuebles.SelectedItem = null;
        TxtTituloFormulario.Text = "Añadir Nuevo Inmueble";

        TxtReferencia.Text = string.Empty;
        TxtTitulo.Text = string.Empty;
        TxtPrecio.Text = string.Empty;
        TxtCiudad.Text = string.Empty;
        TxtHabitaciones.Text = string.Empty;
        CmbTipo.SelectedIndex = 0;
        CmbEstado.SelectedIndex = 0;
    }

    private void OnGuardarInmuebleClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtReferencia.Text) || string.IsNullOrWhiteSpace(TxtTitulo.Text))
        {
            return;
        }

        decimal.TryParse(TxtPrecio.Text, out var precio);
        int.TryParse(TxtHabitaciones.Text, out var habitaciones);

        if (_inmuebleEnEdicion == null)
        {
            // Alta de nuevo inmueble
            var nuevo = new Inmueble
            {
                Id = Guid.NewGuid(),
                Referencia = TxtReferencia.Text.Trim(),
                Titulo = TxtTitulo.Text.Trim(),
                Ciudad = TxtCiudad.Text?.Trim() ?? string.Empty,
                Precio = precio,
                Habitaciones = habitaciones,
                Tipo = (TipoInmueble)(CmbTipo.SelectedItem ?? TipoInmueble.Piso),
                Estado = (EstadoInmueble)(CmbEstado.SelectedItem ?? EstadoInmueble.Disponible),
                FechaAlta = DateTime.UtcNow
            };

            DataManager.Datos.Inmuebles.Add(nuevo);
        }
        else
        {
            // Modificación del existente
            _inmuebleEnEdicion.Referencia = TxtReferencia.Text.Trim();
            _inmuebleEnEdicion.Titulo = TxtTitulo.Text.Trim();
            _inmuebleEnEdicion.Ciudad = TxtCiudad.Text?.Trim() ?? string.Empty;
            _inmuebleEnEdicion.Precio = precio;
            _inmuebleEnEdicion.Habitaciones = habitaciones;
            _inmuebleEnEdicion.Tipo = (TipoInmueble)(CmbTipo.SelectedItem ?? TipoInmueble.Piso);
            _inmuebleEnEdicion.Estado = (EstadoInmueble)(CmbEstado.SelectedItem ?? EstadoInmueble.Disponible);
        }

        // Guardar persistencia en JSON
        DataManager.GuardarDatos();

        RefrescarLista(TxtBuscar.Text);
        LimpiarFormulario();
    }

    private void OnEliminarInmuebleClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Inmueble inm)
        {
            DataManager.Datos.Inmuebles.Remove(inm);
            DataManager.GuardarDatos();
            RefrescarLista(TxtBuscar.Text);
            LimpiarFormulario();
        }
    }
}