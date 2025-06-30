using Generico_Front.Graphics.Conexion;
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

    private Config.Config config;

    //CARAGA DE DATOS
    private void LoadSettings()
    {
        RotulosCheckBox.IsChecked = config.PestanasActivas.Rotulos;
        CrawlsCheckBox.IsChecked = config.PestanasActivas.Crawls;
        CreditosCheckBox.IsChecked = config.PestanasActivas.Creditos;
        FaldonesCheckBox.IsChecked = config.PestanasActivas.Faldones;
        PremiosCheckBox.IsChecked = config.PestanasActivas.Premios;
        SubtituladoCheckBox.IsChecked = config.PestanasActivas.Subtitulado;
        GafasCheckBox.IsChecked = config.PestanasActivas.Gafas;
        TiemposCheckBox.IsChecked = config.PestanasActivas.Tiempos;
        DirectosCheckBox.IsChecked = config.PestanasActivas.Directos;
        VariosCheckBox.IsChecked = config.PestanasActivas.Varios;
        ResetCheckBox.IsChecked = config.PestanasActivas.Reset;
        LogoCheckBox.IsChecked = config.PestanasActivas.Mosca;

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
        ShellPage.instance.UpdateWindows(4, true);
        config.PestanasActivas.Premios = true;
        Config.Config.SaveConfig(config);
    }
    private void PremiosCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(4, false);
        config.PestanasActivas.Premios = false;
        Config.Config.SaveConfig(config);
    }

    private void SubtituladoCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(5, true);
        config.PestanasActivas.Subtitulado = true;
        Config.Config.SaveConfig(config);
    }

    private void SubtituladoCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(5, false);
        config.PestanasActivas.Subtitulado = false;
        Config.Config.SaveConfig(config);
    }

    private void GafasCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(6, true);
        config.PestanasActivas.Gafas = true;
        Config.Config.SaveConfig(config);
    }
    private void GafasCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(6, false);
        config.PestanasActivas.Gafas = false;
        Config.Config.SaveConfig(config);
    }

    private void VariosCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(7, true);
        config.PestanasActivas.Varios = true;
        Config.Config.SaveConfig(config);
        if (!TiemposCheckBox.IsChecked.Value && !DirectosCheckBox.IsChecked.Value)
        {
            TiemposCheckBox.IsChecked = true;
            DirectosCheckBox.IsChecked = true;
        }
        
    }
    private void VariosCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.UpdateWindows(7, false);
        TiemposCheckBox.IsChecked = false;
        DirectosCheckBox.IsChecked = false;
        config.PestanasActivas.Varios = false;
        Config.Config.SaveConfig(config);
    }

    private void TiemposCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        config.PestanasActivas.Tiempos = true;
        Config.Config.SaveConfig(config);
    }
    private void TiemposCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (DirectosCheckBox.IsChecked == false)
        {
            VariosCheckBox.IsChecked = false;
            config.PestanasActivas.Varios = false;
        }
        config.PestanasActivas.Tiempos = false;
        Config.Config.SaveConfig(config);
    }

    private void DirectosCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        config.PestanasActivas.Directos = true;
        Config.Config.SaveConfig(config);
    }
    private void DirectosCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (TiemposCheckBox.IsChecked == false)
        {
            VariosCheckBox.IsChecked = false;
            config.PestanasActivas.Varios = false;
        }
        config.PestanasActivas.Directos = false;
        Config.Config.SaveConfig(config);
    }

    //CHECK BOX BOTONES
    private void ResetCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.btnResetActivo(true);
        config.PestanasActivas.Reset = true;
        Config.Config.SaveConfig(config);
    }
    private void ResetCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.btnResetActivo(false);
        config.PestanasActivas.Reset = false;
        Config.Config.SaveConfig(config);
    }

    private void LogoCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.btnMoscaActivo(true);
        config.PestanasActivas.Mosca = true;
        Config.Config.SaveConfig(config);
    }
    private void LogoCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShellPage.instance.btnMoscaActivo(false);
        config.PestanasActivas.Mosca = false;
        Config.Config.SaveConfig(config);
    }

    //SOFTWARE GRAFICO
    private void IpBrainstorm_TextChanged(object sender, TextChangedEventArgs e)
    {
        config.BrainStormOptions.Ip = IpBrainstorm.Text;
        Config.Config.SaveConfig(config);
    }

    private void DbBrainstorm_TextChanged(object sender, TextChangedEventArgs e)
    {
        config.BrainStormOptions.Database = DbBrainstorm.Text;
        Config.Config.SaveConfig(config);
    }

    private void btnReconectarBS_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        bool exito = BSConexion.GetInstance().Reconectar();
        ShowDialog(exito);
    }
    public async void ShowDialog(bool exito)
    {
        if (exito)
        {
            SuccessInfoBar.Message = $"Se ha conectado con éxito a Brainstorm en el equipo {IpBrainstorm.Text}";
            SuccessInfoBar.IsOpen = true;
        }
        else
        {
            ContentDialog errorDialog = new ContentDialog()
            {
                Title = "Error al conectarse a Brainstorm",
                Content = "Parece que ha ocurrido un error al conectarse con Brainstorm.\nRevise que la IP sea correcta y que el programa esté en funcionamiento en el equipo especificado",
                CloseButtonText = "Cerrar",
                XamlRoot = this.XamlRoot
            };

            await errorDialog.ShowAsync();
        }
    }
}
