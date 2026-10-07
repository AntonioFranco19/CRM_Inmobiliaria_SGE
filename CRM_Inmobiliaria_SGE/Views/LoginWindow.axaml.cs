using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CRM_Inmobiliaria_SGE.Data;

namespace CRM_Inmobiliaria_SGE.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        var usuario = TxtUsuario.Text?.Trim() ?? "";
        var pass = TxtPassword.Text ?? "";

        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(pass))
        {
            MostrarError("Introduce usuario y contraseña.");
            return;
        }

        var usuarioEncontrado = DataManager.Datos.Usuarios
            .FirstOrDefault(u => u.Username.Equals(usuario, StringComparison.OrdinalIgnoreCase) && u.Activo);

        if (usuarioEncontrado == null || usuarioEncontrado.PasswordHash != pass)
        {
            MostrarError("Usuario o contraseña incorrectos.");
            return;
        }

        DataManager.UsuarioActual = usuarioEncontrado;

        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

    private void MostrarError(string mensaje)
    {
        TxtError.Text = mensaje;
        TxtError.IsVisible = true;
    }
}