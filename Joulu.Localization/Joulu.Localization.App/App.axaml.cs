using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Joulu.Localization.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
            
            Services = new ServiceCollection()
                .AddSingleton<IStorageProvider>(window.StorageProvider)
                .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
            
            // Services must be initialized before creating first viewmodel
            window.DataContext = new MainWindowViewModel();
            
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private IServiceProvider? Services { get; set; }

    internal static T Resolve<T>() where T : notnull
    {
        if (Current is App { Services: not null } app)
        {
            return app.Services.GetRequiredService<T>();
        }

        throw new InvalidOperationException("Cannot resolve required service, because application is not set");
    }
}