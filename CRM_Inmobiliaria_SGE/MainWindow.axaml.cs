using Avalonia.Controls;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;
using CRM_Inmobiliaria_SGE.Views;

namespace CRM_Inmobiliaria_SGE;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ConfigurarPermisosYUsuario();

        // Carga por defecto el primer módulo operativo accesible para todos los roles
        ContenedorModulo.Content = new InmueblesView();
    }

    private void ConfigurarPermisosYUsuario()
    {
        var usuario = DataManager.UsuarioActual;
        if (usuario == null) return;

        // Mostrar datos en el encabezado del menú lateral
        var txtInfoUsuario = this.FindControl<TextBlock>("TxtInfoUsuario");
        if (txtInfoUsuario != null)
        {
            txtInfoUsuario.Text = $"{usuario.NombreCompleto}\nRol: {usuario.Rol}";
        }

        // Validación estricta del rol Administrador
        bool esAdmin = usuario.Rol == RolUsuario.Administrador;

        // Control de visibilidad de botones restringidos
        bool hayBotones = BtnMenuFacturacion != null && BtnMenuInformes != null && BtnMenuUsuarios != null && TxtSeccionAdmin != null;
        if (hayBotones)
        {
            TxtSeccionAdmin.IsVisible = esAdmin;
            BtnMenuFacturacion.IsVisible = esAdmin;
            BtnMenuInformes.IsVisible = esAdmin;
            BtnMenuUsuarios.IsVisible = esAdmin;
        }
    }

    // --- MÓDULOS OPERATIVOS (Accesibles para Agentes y Administradores) ---

    private void OnMenuInmueblesClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new InmueblesView();
    }

    private void OnMenuClientesClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new ClientesView();
    }

    private void OnMenuVisitasClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new VisitasView();
    }

    private void OnMenuMatchingClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new MatchingView();
    }

    private void OnMenuVentasClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new VentasView();
    }

    private void OnMenuProveedoresClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new ProveedoresView();
    }

    // --- MÓDULOS DE ADMINISTRACIÓN (Blindados solo para Administrador) ---

    private void OnMenuFacturacionClick(object? sender, RoutedEventArgs e)
    {
        if (DataManager.UsuarioActual?.Rol != RolUsuario.Administrador) return;
        ContenedorModulo.Content = new FacturacionView();
    }

    private void OnMenuInformesClick(object? sender, RoutedEventArgs e)
    {
        if (DataManager.UsuarioActual?.Rol != RolUsuario.Administrador) return;
        ContenedorModulo.Content = new InformesView();
    }

    private void OnMenuUsuariosClick(object? sender, RoutedEventArgs e)
    {
        if (DataManager.UsuarioActual?.Rol != RolUsuario.Administrador) return;
        ContenedorModulo.Content = new UsuariosView();
    }

    // --- CIERRE DE SESIÓN ---

    private void OnCerrarSesionClick(object? sender, RoutedEventArgs e)
    {
        DataManager.UsuarioActual = null;
        var login = new LoginWindow();
        login.Show();
        this.Close();
    }
}