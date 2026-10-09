using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;


namespace CRM_Inmobiliaria_SGE.Views;

public partial class UsuariosView : UserControl
{
    private Usuario? _usuarioEnEdicion = null;

    public UsuariosView()
    {
        InitializeComponent();
        CargarCombos();
        RefrescarLista();
    }

    private void CargarCombos()
    {
        CmbRol.ItemsSource = Enum.GetValues<RolUsuario>();
        CmbRol.SelectedIndex = 1; // Por defecto Agente
    }

    private void RefrescarLista()
    {
        ListaUsuarios.ItemsSource = DataManager.Datos.Usuarios.OrderBy(u => u.Rol).ThenBy(u => u.Username).ToList();
    }

    private void OnUsuarioSeleccionadoChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ListaUsuarios.SelectedItem is Usuario u)
        {
            _usuarioEnEdicion = u;
            TxtTituloFormulario.Text = $"Editar Usuario: {u.Username}";
            TxtUsername.Text = u.Username;
            TxtNombreCompleto.Text = u.NombreCompleto;
            TxtPassword.Text = u.PasswordHash;
            CmbRol.SelectedItem = u.Rol;
            ChkActivo.IsChecked = u.Activo;
        }
    }

    private void OnNuevoUsuarioClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        TxtUsername.Focus();
    }

    private void OnLimpiarClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        _usuarioEnEdicion = null;
        ListaUsuarios.SelectedItem = null;
        TxtTituloFormulario.Text = "Añadir Nuevo Usuario";
        TxtUsername.Text = string.Empty;
        TxtNombreCompleto.Text = string.Empty;
        TxtPassword.Text = string.Empty;
        ChkActivo.IsChecked = true;
        CmbRol.SelectedIndex = 1;
    }

    private void OnGuardarUsuarioClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtUsername.Text) || string.IsNullOrWhiteSpace(TxtPassword.Text)) return;

        var rol = (RolUsuario)(CmbRol.SelectedItem ?? RolUsuario.Agente);
        bool activo = ChkActivo.IsChecked == true;

        if (_usuarioEnEdicion == null)
        {
            var nuevo = new Usuario
            {
                Id = Guid.NewGuid(),
                Username = TxtUsername.Text.Trim(),
                NombreCompleto = TxtNombreCompleto.Text?.Trim() ?? string.Empty,
                PasswordHash = TxtPassword.Text.Trim(),
                Rol = rol,
                Activo = activo
            };
            DataManager.Datos.Usuarios.Add(nuevo);
        }
        else
        {
            _usuarioEnEdicion.Username = TxtUsername.Text.Trim();
            _usuarioEnEdicion.NombreCompleto = TxtNombreCompleto.Text?.Trim() ?? string.Empty;
            _usuarioEnEdicion.PasswordHash = TxtPassword.Text.Trim();
            _usuarioEnEdicion.Rol = rol;
            _usuarioEnEdicion.Activo = activo;
        }

        DataManager.GuardarDatos();
        RefrescarLista();
        LimpiarFormulario();
    }

    private void OnEliminarUsuarioClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Usuario u)
        {
            // Evitar que el usuario borre su propia cuenta en uso
            if (DataManager.UsuarioActual != null && DataManager.UsuarioActual.Id == u.Id)
            {
                return;
            }

            DataManager.Datos.Usuarios.Remove(u);
            DataManager.GuardarDatos();
            RefrescarLista();
            LimpiarFormulario();
        }
    }
}