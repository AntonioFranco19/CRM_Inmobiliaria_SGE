using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using InmoCRM.Models;
using InmoCRM.Services;
using InmoCRM.Data;

namespace CRM_Inmobiliaria_SGE;

public partial class App : Application
{
    public static InmoDbContext DbContext { get; private set; } = null!;
    public static AuthService AuthService { get; private set; } = null!;
    public static InmuebleService InmuebleService { get; private set; } = null!;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}