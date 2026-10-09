using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;

namespace CRM_Inmobiliaria_SGE.Views;

// Simple DTO plano sin MVVM
public class VisitaFilaItem
{
    public Visita VisitaOriginal { get; set; } = null!;
    public DateTime FechaHora => VisitaOriginal.FechaHora;
    public string EstadoTexto => VisitaOriginal.Realizada ? "Completada" : "Pendiente";
    public string ColorEstado => VisitaOriginal.Realizada ? "#16A34A" : "#D97706";
    public string InmuebleTexto { get; set; } = string.Empty;
    public string InmuebleDireccion { get; set; } = string.Empty;
    public string ClienteTexto { get; set; } = string.Empty;
    public string ComentariosCliente => string.IsNullOrEmpty(VisitaOriginal.ComentariosCliente) ? "Sin comentarios" : VisitaOriginal.ComentariosCliente;
    public string InteresTexto => VisitaOriginal.InteresEstimado > 0 ? $"Interés: {VisitaOriginal.InteresEstimado}/5" : "Sin valorar";
}

public partial class VisitasView : UserControl
{
    private Visita? _visitaEnEdicion = null;

    public VisitasView()
    {
        InitializeComponent();
        CargarCombos();
        RefrescarLista();
    }

    private CheckBox? CheckBoxRealizada => this.FindControl<CheckBox>("ChkRealizada");

    private void CargarCombos()
    {
        CmbInmuebles.ItemsSource = DataManager.Datos.Inmuebles.Select(i => $"{i.Referencia} - {i.Titulo}").ToList();
        CmbClientes.ItemsSource = DataManager.Datos.Clientes.Select(c => $"{c.Nif} - {c.Nombre} {c.Apellidos}").ToList();

        CmbInteres.ItemsSource = new List<string> { "1 - Muy bajo", "2 - Bajo", "3 - Medio", "4 - Alto", "5 - Muy alto" };
        CmbInteres.SelectedIndex = 2;

        TxtFechaHora.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy 11:00");
    }

    private void RefrescarLista()
    {
        var lista = new List<VisitaFilaItem>();

        foreach (var v in DataManager.Datos.Visitas.OrderBy(v => v.FechaHora))
        {
            var inm = DataManager.Datos.Inmuebles.FirstOrDefault(i => i.Id == v.InmuebleId);
            var cli = DataManager.Datos.Clientes.FirstOrDefault(c => c.Id == v.ClienteId);

            lista.Add(new VisitaFilaItem
            {
                VisitaOriginal = v,
                InmuebleTexto = inm != null ? $"{inm.Referencia}: {inm.Titulo}" : "Inmueble no encontrado",
                InmuebleDireccion = inm != null ? $"{inm.Direccion}, {inm.Ciudad}" : "",
                ClienteTexto = cli != null ? $"{cli.Nombre} {cli.Apellidos} ({cli.Telefono})" : "Cliente no encontrado"
            });
        }

        ListaVisitas.ItemsSource = lista;
    }

    private void OnVisitaSeleccionadaChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ListaVisitas.SelectedItem is VisitaFilaItem seleccionado)
        {
            _visitaEnEdicion = seleccionado.VisitaOriginal;
            TxtTituloFormulario.Text = "Editar Visita Agendada";

            var inmIdx = DataManager.Datos.Inmuebles.FindIndex(i => i.Id == _visitaEnEdicion.InmuebleId);
            if (inmIdx >= 0) CmbInmuebles.SelectedIndex = inmIdx;

            var cliIdx = DataManager.Datos.Clientes.FindIndex(c => c.Id == _visitaEnEdicion.ClienteId);
            if (cliIdx >= 0) CmbClientes.SelectedIndex = cliIdx;

            TxtFechaHora.Text = _visitaEnEdicion.FechaHora.ToString("dd/MM/yyyy HH:mm");
            TxtComentarios.Text = _visitaEnEdicion.ComentariosCliente;

            if (CheckBoxRealizada != null)
            {
                CheckBoxRealizada.IsChecked = _visitaEnEdicion.Realizada;
            }

            if (_visitaEnEdicion.InteresEstimado >= 1 && _visitaEnEdicion.InteresEstimado <= 5)
            {
                CmbInteres.SelectedIndex = _visitaEnEdicion.InteresEstimado - 1;
            }
        }
    }

    private void OnNuevaVisitaClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private void OnLimpiarFormularioClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        _visitaEnEdicion = null;
        ListaVisitas.SelectedItem = null;
        TxtTituloFormulario.Text = "Agendar Nueva Visita";
        TxtFechaHora.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy 11:00");
        TxtComentarios.Text = string.Empty;

        if (CheckBoxRealizada != null)
        {
            CheckBoxRealizada.IsChecked = false;
        }

        CmbInteres.SelectedIndex = 2;
    }

    private void OnGuardarVisitaClick(object? sender, RoutedEventArgs e)
    {
        if (CmbInmuebles.SelectedIndex < 0 || CmbClientes.SelectedIndex < 0) return;

        var inmueble = DataManager.Datos.Inmuebles[CmbInmuebles.SelectedIndex];
        var cliente = DataManager.Datos.Clientes[CmbClientes.SelectedIndex];

        if (!DateTime.TryParseExact(TxtFechaHora.Text?.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
        {
            if (!DateTime.TryParse(TxtFechaHora.Text, out fecha))
            {
                fecha = DateTime.Now;
            }
        }

        int interes = CmbInteres.SelectedIndex + 1;
        bool realizada = CheckBoxRealizada?.IsChecked == true;
        string comentarios = TxtComentarios.Text?.Trim() ?? string.Empty;

        if (_visitaEnEdicion == null)
        {
            var nueva = new Visita
            {
                Id = Guid.NewGuid(),
                InmuebleId = inmueble.Id,
                ClienteId = cliente.Id,
                AgenteId = DataManager.UsuarioActual?.Id ?? Guid.Empty,
                FechaHora = fecha,
                ComentariosCliente = comentarios,
                InteresEstimado = interes,
                Realizada = realizada
            };
            DataManager.Datos.Visitas.Add(nueva);
        }
        else
        {
            _visitaEnEdicion.InmuebleId = inmueble.Id;
            _visitaEnEdicion.ClienteId = cliente.Id;
            _visitaEnEdicion.FechaHora = fecha;
            _visitaEnEdicion.ComentariosCliente = comentarios;
            _visitaEnEdicion.InteresEstimado = interes;
            _visitaEnEdicion.Realizada = realizada;
        }

        DataManager.GuardarDatos();
        RefrescarLista();
        LimpiarFormulario();
    }

    private void OnEliminarVisitaClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is VisitaFilaItem item)
        {
            DataManager.Datos.Visitas.Remove(item.VisitaOriginal);
            DataManager.GuardarDatos();
            RefrescarLista();
            LimpiarFormulario();
        }
    }
}