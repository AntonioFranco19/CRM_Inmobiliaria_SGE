using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;


namespace CRM_Inmobiliaria_SGE.Views;

public partial class MatchingView : UserControl
{
    public MatchingView()
    {
        InitializeComponent();
        CargarClientes();
    }

    private void CargarClientes()
    {
        CmbClientesMatching.ItemsSource = DataManager.Datos.Clientes.Select(c => $"{c.Nif} - {c.Nombre} {c.Apellidos}").ToList();
        if (DataManager.Datos.Clientes.Any())
        {
            CmbClientesMatching.SelectedIndex = 0;
        }
    }

    private void OnClienteSeleccionadoParaMatchingChanged(object? sender, SelectionChangedEventArgs e)
    {
        EjecutarAlgoritmoMatching();
    }

    private void OnRecalcularClick(object? sender, RoutedEventArgs e)
    {
        EjecutarAlgoritmoMatching();
    }

    private void EjecutarAlgoritmoMatching()
    {
        if (CmbClientesMatching.SelectedIndex < 0 || CmbClientesMatching.SelectedIndex >= DataManager.Datos.Clientes.Count)
        {
            TxtCriteriosCliente.Text = "Ningún cliente seleccionado.";
            ListaResultadosMatching.ItemsSource = new List<Inmueble>();
            TxtTotalCoincidencias.Text = "Inmuebles Coincidentes: 0";
            return;
        }

        var cliente = DataManager.Datos.Clientes[CmbClientesMatching.SelectedIndex];

        // Mostrar criterios en pantalla
        var tipoStr = cliente.TipoInteres.HasValue ? cliente.TipoInteres.Value.ToString() : "Cualquier tipo";
        var ptoStr = cliente.PresupuestoMaximo > 0 ? $"{cliente.PresupuestoMaximo:N0} €" : "Sin límite";
        var zonaStr = !string.IsNullOrEmpty(cliente.ZonaInteres) ? cliente.ZonaInteres : "Cualquier zona";

        TxtCriteriosCliente.Text = $"Criterios de {cliente.Nombre}: [Tipo: {tipoStr}] | [Presupuesto Máx: {ptoStr}] | [Zona: {zonaStr}]";

        // Algoritmo de filtrado sobre inmuebles disponibles
        var query = DataManager.Datos.Inmuebles.Where(i => i.Estado == EstadoInmueble.Disponible);

        if (cliente.PresupuestoMaximo > 0)
        {
            query = query.Where(i => i.Precio <= cliente.PresupuestoMaximo);
        }

        if (cliente.TipoInteres.HasValue)
        {
            query = query.Where(i => i.Tipo == cliente.TipoInteres.Value);
        }

        if (!string.IsNullOrWhiteSpace(cliente.ZonaInteres))
        {
            var zona = cliente.ZonaInteres.Trim().ToLowerInvariant();
            query = query.Where(i => i.Ciudad.ToLowerInvariant().Contains(zona) || i.Direccion.ToLowerInvariant().Contains(zona));
        }

        var coincidencias = query.OrderBy(i => i.Precio).ToList();
        ListaResultadosMatching.ItemsSource = coincidencias;
        TxtTotalCoincidencias.Text = $"Inmuebles Coincidentes: {coincidencias.Count}";
    }

    private void OnAgendarVisitaDirectaClick(object? sender, RoutedEventArgs e)
    {
        // En una app desktop se puede redirigir o precargar la visita
        if (sender is Button btn && btn.DataContext is Inmueble inmueble && CmbClientesMatching.SelectedIndex >= 0)
        {
            var cliente = DataManager.Datos.Clientes[CmbClientesMatching.SelectedIndex];

            var nuevaVisita = new Visita
            {
                Id = System.Guid.NewGuid(),
                InmuebleId = inmueble.Id,
                ClienteId = cliente.Id,
                AgenteId = DataManager.UsuarioActual?.Id ?? System.Guid.Empty,
                FechaHora = System.DateTime.Now.AddDays(1),
                ComentariosCliente = "Visita generada desde Matching automático",
                InteresEstimado = 4,
                Realizada = false
            };

            DataManager.Datos.Visitas.Add(nuevaVisita);
            DataManager.GuardarDatos();

            TxtTotalCoincidencias.Text = $"¡Visita registrada para {cliente.Nombre} en {inmueble.Referencia}!";
        }
    }
}