using Generico_Front.Contracts.Services;
using Generico_Front.Helpers;
using Generico_Front.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Windows.System;

namespace Generico_Front.Views;

public sealed partial class ShellPage : Page
{
    public static ShellPage instance
    {
        get; private set;
    }

    public ShellViewModel ViewModel
    {
        get;
    }
    Config.Config config;

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();
        config = Config.Config.GetInstance();
        InitialWindows();
        instance = this;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        App.AppTitlebar = AppTitleBarText as UIElement;
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }

    private void InitialWindows()
    {
        ventanaRotulos.Visibility = config.PestanasActivas.Rotulos ? Visibility.Visible : Visibility.Collapsed;
        ventanaCrawls.Visibility = config.PestanasActivas.Crawls ? Visibility.Visible : Visibility.Collapsed;
        ventanaCreditos.Visibility = config.PestanasActivas.Creditos ? Visibility.Visible : Visibility.Collapsed;
        ventanaFaldones.Visibility = config.PestanasActivas.Faldones ? Visibility.Visible : Visibility.Collapsed;
    }
    public void UpdateWindows(int modificada, bool activo)
    {
        switch (modificada)
        {
            case 0:
                ventanaRotulos.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
            case 1:
                ventanaCrawls.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
            case 2:
                ventanaCreditos.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
            case 3:
                ventanaFaldones.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
            case 4:
                //  ventanaPremios.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
        }
    }
}
