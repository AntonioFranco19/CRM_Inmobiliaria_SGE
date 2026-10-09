using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
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
    }

    private void ConfigurarPermisosYUsuario()
    {
        var usuario = DataManager.UsuarioActual;
        if (usuario != null)
        {
            var txtInfoUsuario = this.FindControl<TextBlock>("TxtInfoUsuario");
            if (txtInfoUsuario != null)
            {
                txtInfoUsuario.Text = $"{usuario.NombreCompleto}\nRol: {usuario.Rol}";
            }

            bool esAdmin = usuario.Rol == RolUsuario.Administrador;

            var btnFacturacion = this.FindControl<Button>("BtnFacturacion");
            if (btnFacturacion != null) btnFacturacion.IsVisible = esAdmin;

            var btnInformes = this.FindControl<Button>("BtnInformes");
            if (btnInformes != null) btnInformes.IsVisible = esAdmin;

            var btnUsuarios = this.FindControl<Button>("BtnUsuarios");
            if (btnUsuarios != null) btnUsuarios.IsVisible = esAdmin;
        }
    }

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

    private void OnMenuFacturacionClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new FacturacionView();
    }

    private void OnMenuInformesClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new InformesView();
    }

    private void OnMenuUsuariosClick(object? sender, RoutedEventArgs e)
    {
        ContenedorModulo.Content = new UsuariosView();
    }

    private void OnCerrarSesionClick(object? sender, RoutedEventArgs e)
    {
        DataManager.UsuarioActual = null;
        var login = new LoginWindow();
        login.Show();
        this.Close();
    }
}