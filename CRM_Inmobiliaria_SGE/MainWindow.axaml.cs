using Avalonia.Controls;
using Avalonia.Interactivity;
using InmoCRM.Data;
using InmoCRM.Models.Enums;

namespace InmoCRM.Views;

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
            TxtInfoUsuario.Text = $"{usuario.NombreCompleto}\nRol: {usuario.Rol}";

            bool esAdmin = usuario.Rol == RolUsuario.Administrador;
            BtnFacturacion.IsVisible = esAdmin;
            BtnInformes.IsVisible = esAdmin;
            BtnUsuarios.IsVisible = esAdmin;
        }
    }

    // Manejadores de menú vinculados en el XAML
    private void OnMenuInmueblesClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new InmueblesView();
    }

    private void OnMenuClientesClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new ClientesView();
    }

    private void OnMenuVisitasClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new VisitasView();
    }

    private void OnMenuMatchingClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new MatchingView();
    }

    private void OnMenuVentasClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new VentasView();
    }

    private void OnMenuProveedoresClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new ProveedoresView();
    }

    private void OnMenuFacturacionClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new FacturacionView();
    }

    private void OnMenuInformesClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new InformesView();
    }

    private void OnMenuUsuariosClick(object? sender, RoutedEventArgs e)
    {
        // ContenedorModulo.Content = new UsuariosView();
    }

    private void OnCerrarSesionClick(object? sender, RoutedEventArgs e)
    {
        DataManager.UsuarioActual = null;
        var login = new LoginWindow();
        login.Show();
        this.Close();
    }
}