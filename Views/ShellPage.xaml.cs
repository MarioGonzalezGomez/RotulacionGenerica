﻿using Generico_Front.Contracts.Services;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Graphics.Conexion;
using Generico_Front.Helpers;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI;
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
    private BSController controller;
    private Config.Config config;
    private bool mosca = false;

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
        controller = BSController.GetInstance();
        InitialWindows();
        instance = this;
        bool exito = BSConexion.GetInstance().Reconectar();
        ShowDialog(exito);
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

    //MANEJO PESTANAS ACTIVAS
    private void InitialWindows()
    {
        ventanaRotulos.Visibility = config.PestanasActivas.Rotulos ? Visibility.Visible : Visibility.Collapsed;
        ventanaCrawls.Visibility = config.PestanasActivas.Crawls ? Visibility.Visible : Visibility.Collapsed;
        ventanaCreditos.Visibility = config.PestanasActivas.Creditos ? Visibility.Visible : Visibility.Collapsed;
        ventanaFaldones.Visibility = config.PestanasActivas.Faldones ? Visibility.Visible : Visibility.Collapsed;
        ventanaPremios.Visibility = config.PestanasActivas.Premios ? Visibility.Visible : Visibility.Collapsed;
        ventanaSubtitulado.Visibility = config.PestanasActivas.Subtitulado ? Visibility.Visible : Visibility.Collapsed;
        ventanaGafas.Visibility = config.PestanasActivas.Gafas ? Visibility.Visible : Visibility.Collapsed;
        ventanaVarios.Visibility = config.PestanasActivas.Varios ? Visibility.Visible : Visibility.Collapsed;

        btnReset.Visibility = config.PestanasActivas.Reset ? Visibility.Visible : Visibility.Collapsed;
        btnMosca.Visibility = config.PestanasActivas.Mosca ? Visibility.Visible : Visibility.Collapsed;
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
                ventanaPremios.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
            case 5:
                ventanaSubtitulado.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
            case 6:
                ventanaGafas.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
            case 7:
                ventanaVarios.Visibility = activo ? Visibility.Visible : Visibility.Collapsed;
                break;
        }
    }

    //MANEJO CONEXION
    public async void ShowDialog(bool exito)
    {
        if (exito)
        {
            SuccessInfoBar.Title = "Conectado con éxito";
            SuccessInfoBar.Message = $"Se ha conectado con éxito a Brainstorm en el equipo {config.BrainStormOptions.Ip}";
            SuccessInfoBar.Severity = InfoBarSeverity.Success;
            SuccessInfoBar.IsOpen = true;
            await Task.Delay(3000);
            SuccessInfoBar.IsOpen = false;
        }
        else
        {
            SuccessInfoBar.Title = "Error al conectar";
            SuccessInfoBar.Message = $"No se ha podido conectar a Brainstorm en el equipo {config.BrainStormOptions.Ip}, revise que los datos sean correctos en el apartado de Settings y que Brainstorm esté funcionando correctamente en el equipo destino";
            SuccessInfoBar.Severity = InfoBarSeverity.Error;
            SuccessInfoBar.IsOpen = true;
        }
       
    }

    //MANEJO RESET
    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        controller.Reset();
    }
    public void btnResetActivo(bool activo)
    {
        if (activo)
        {
            btnReset.Visibility = Visibility.Visible;
        }
        else
        {
            btnReset.Visibility = Visibility.Collapsed;
        }
    }

    //MANEJO MOSCA
    private void btnMosca_Click(object sender, RoutedEventArgs e)
    {
        if (mosca)
        {
            controller.MoscaSale();
            mosca = false;
            btnMosca.Foreground = new SolidColorBrush(Colors.Green);
        }
        else
        {
            controller.MoscaEntra();
            mosca = true;
            btnMosca.Foreground = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xB2, 0x22, 0x22));
        }
    }
    public void btnMoscaActivo(bool activo)
    {
        if (activo)
        {
            btnMosca.Visibility = Visibility.Visible;
        }
        else
        {
            btnMosca.Visibility = Visibility.Collapsed;
        }
    }
}
