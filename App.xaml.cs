using System.Diagnostics;
using Generico_Front.Activation;
using Generico_Front.Contracts.Services;
using Generico_Front.Core.Contracts.Services;
using Generico_Front.Core.Services;
using Generico_Front.Helpers;
using Generico_Front.Models;
using Generico_Front.Services;
using Generico_Front.ViewModels;
using Generico_Front.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Generico_Front;

public partial class App : Application
{
    private Process backendProcess;
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<RotulosViewModel>();
            services.AddTransient<RotulosPage>();
            services.AddTransient<CrawlsViewModel>();
            services.AddTransient<CrawlsPage>();
            services.AddTransient<RodillosViewModel>();
            services.AddTransient<RodillosPage>();
            services.AddTransient<FaldonesViewModel>();
            services.AddTransient<FaldonesPage>();
            services.AddTransient<PremiosPage>();
            services.AddTransient<PremiosViewModel>();
            services.AddTransient<SubtituladoPage>();
            services.AddTransient<SubtituladoViewModel>();
            services.AddTransient<GafasPage>();
            services.AddTransient<GafasViewModel>();
            services.AddTransient<VariosPage>();
            services.AddTransient<VariosViewModel>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        File.WriteAllText($"C:\\TrabajoIPF\\Logs\\error{e.Exception.TargetSite}.log", $"Excepción no controlada: {e.Exception.Message}\n{e.Exception.StackTrace}");
        e.Handled = true;
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }



}
