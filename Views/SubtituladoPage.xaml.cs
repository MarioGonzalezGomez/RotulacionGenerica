using System.Diagnostics;
using System.Text.Json;
using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;

namespace Generico_Front.Views;

public sealed partial class SubtituladoPage : Page
{

    private Config.Config config;
    public SubtituladoViewModel ViewModel
    {
        get;
    }

    private string selected;
    private bool 
    public SubtituladoPage()
    {
        ViewModel = App.GetService<SubtituladoViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        IniciarListas();
    }

    private void IniciarListas()
    {
        ViewModel.CargarSubtitulos();
        LVSubtitulos.ItemsSource = ViewModel.Subtitulos;
        txtRuta.Text = config.RotulacionSettings.RutaSubtitulos;
    }

    //FILTRADO
    private void FiltradoPorTexto_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allSubtitulos
       .Where(c => c.IndexOf(FiltradoPorTexto.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPosicion_TextChanged(object sender, TextChangedEventArgs e)
    {
        string value = string.IsNullOrEmpty(FiltradoPorPosicion.Text) ? "0" : FiltradoPorPosicion.Text;
        int inicio = int.Parse(value);
        List<string> filtrada = ViewModel.allSubtitulos.GetRange(inicio, ViewModel.allSubtitulos.Count - inicio);

        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);

    }

    //EDICION
    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Visibility.Visible;
            linkAjustes.Visibility = Visibility.Visible;
            stckBotonera.Visibility = Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Visibility.Collapsed;
            linkAjustes.Visibility = Visibility.Collapsed;
            stckBotonera.Visibility = Visibility.Collapsed;

        }
    }

    private async void filePicker_Click(object sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;

        txtRuta.Text = "";

        var openPicker = new FileOpenPicker();

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".txt");
        openPicker.FileTypeFilter.Add(".csv");

        var file = await abrirFilePicker(openPicker);
        if (file != null)
        {
            txtRuta.Text = file.Path;
            config.RotulacionSettings.RutaRodillo = file.Path;
            PonerSubtitulosEnLista();
        }
        else
        {
            txtRuta.Text = config.RotulacionSettings.RutaRodillo;
        }

        senderButton.IsEnabled = true;
    }
    private void PonerSubtitulosEnLista()
    {
        try
        {
            config.RotulacionSettings.RutaSubtitulos = txtRuta.Text;
            Config.Config.SaveConfig(config);
            ViewModel.CargarSubtitulos();
        }
        catch (Exception)
        {
            throw;
        }
    }
    private async Task<Windows.Storage.StorageFile> abrirFilePicker(FileOpenPicker openPicker)
    {
        var file = await openPicker.PickSingleFileAsync();
        return file;
    }

    private async void btnModificar_Click(object sender, RoutedEventArgs e)
    {
        await AbrirFicheroYRefrescarUI(config.RotulacionSettings.RutaSubtitulos);

    }
    public async Task AbrirFicheroYRefrescarUI(string rutaFichero)
    {
        if (!File.Exists(rutaFichero))
        {
            ShowDialog("El fichero no existe", $"No puede encontrarse el fichero de subtítulos en la ruta {rutaFichero}");
            return;
        }

        try
        {
            var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{rutaFichero}\"",
                    UseShellExecute = true
                }
            };

            proceso.Start();
            await proceso.WaitForExitAsync();
            ViewModel.CargarSubtitulos();
        }
        catch (Exception ex)
        {
            ShowDialog("Error al abrir el fichero", ex.Message);
        }
    }

    private void btnGuardar_Click(object sender, RoutedEventArgs e)
    {

    }

    //EDICION TIPOS
    private void AbrirPanelAjustes(object sender, RoutedEventArgs e)
    {

    }

    //METODOS DE LANZADO DE MENSAJES
    private async void AbrirTip()
    {
        Tip.IsOpen = true;
        await Task.Delay(1500);
        Tip.IsOpen = false;
    }
    private void Tip_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        Tip.IsOpen = false;
    }

    public async void ShowDialog(string titulo, string content)
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = titulo,
            Content = content,
            CloseButtonText = "Cerrar",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }


    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(selected)) {

            ViewModel.Entra(selected);
        }
        
    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Sale();
    }

}
