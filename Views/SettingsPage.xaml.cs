using Generico_Front.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Generico_Front.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        LoadSettings();
    }

    Config.Config config;

    //CARAGA DE DATOS
    private void LoadSettings()
    {
        RotulosCheckBox.IsChecked = config.PestanasActivas.Rotulos;
        CrawlsCheckBox.IsChecked = config.PestanasActivas.Crawls;
        CreditosCheckBox.IsChecked = config.PestanasActivas.Creditos;
        FaldonesCheckBox.IsChecked = config.PestanasActivas.Faldones;
        PremiosCheckBox.IsChecked = config.PestanasActivas.Premios;
        VariosCheckBox.IsChecked = config.PestanasActivas.Varios;

        IpBrainstorm.Text = config.BrainStormOptions.Ip;
        DbBrainstorm.Text = config.BrainStormOptions.Database;
    }

    //CHECK BOX PESTANAS
    private void RotulosCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(0, true);
        config.PestanasActivas.Rotulos = true;
        Config.Config.SaveConfig(config);
    }
    private void RotulosCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(0, false);
        config.PestanasActivas.Rotulos = false;
        Config.Config.SaveConfig(config);
    }

    private void CrawlsCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(1, true);
        config.PestanasActivas.Crawls = true;
        Config.Config.SaveConfig(config);
    }
    private void CrawlsCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(1, false);
        config.PestanasActivas.Crawls = false;
        Config.Config.SaveConfig(config);
    }

    private void CreditosCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(2, true);
        config.PestanasActivas.Creditos = true;
        Config.Config.SaveConfig(config);
    }
    private void CreditosCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(2, false);
        config.PestanasActivas.Creditos = false;
        Config.Config.SaveConfig(config);
    }

    private void FaldonesCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(3, true);
        config.PestanasActivas.Faldones = true;
        Config.Config.SaveConfig(config);
    }
    private void FaldonesCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(3, false);
        config.PestanasActivas.Faldones = false;
        Config.Config.SaveConfig(config);
    }

    private void PremiosCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
    private void PremiosCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void VariosCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
    private void VariosCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }


    //SOFTWARE GRAFICO
    private void IpBrainstorm_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void DbBrainstorm_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}
