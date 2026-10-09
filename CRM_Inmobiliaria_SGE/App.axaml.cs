using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Views;

namespace CRM_Inmobiliaria_SGE;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DataManager.CargarDatos();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var login = new LoginWindow();
            desktop.MainWindow = login;
            login.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}