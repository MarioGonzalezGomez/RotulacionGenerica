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

public sealed partial class RodillosPage : Page
{
    public List<Cargo> cargos
    {
        get; set;
    }
    Config.Config config;
    public RodillosViewModel ViewModel
    {
        get;
    }

    public RodillosPage()
    {
        ViewModel = App.GetService<RodillosViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarRodillos();
        cargos = ViewModel.allCargos;

    }

    //FILTRADO
    private void FiltradoPorPersona_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    //EDICIÓN
    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Visibility.Visible;
            stckEditior1.Visibility = Visibility.Visible;
            stckEditior2.Visibility = Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Visibility.Collapsed;
            stckEditior1.Visibility = Visibility.Collapsed;
            stckEditior2.Visibility = Visibility.Collapsed;
        }
    }

    private void filePicker_Click(object sender, RoutedEventArgs e)
    {
        //disable the button to avoid double-clicking
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;

        // Clear previous returned file name, if it exists, between iterations of this scenario
        txtRuta.Text = "";

        // Create a file picker
        var openPicker = new FileOpenPicker();

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".txt");
        openPicker.FileTypeFilter.Add(".csv");

        // Open the picker for the user to pick a file
        var file = abrirFilePicker(openPicker);
        if (file != null)
        {
            txtRuta.Text = file.Result.Path;
        }

        //re-enable the button
        senderButton.IsEnabled = true;
    }
    private async Task<Windows.Storage.StorageFile> abrirFilePicker(FileOpenPicker openPicker)
    {
        var file = await openPicker.PickSingleFileAsync();
        return file;
    }

    private void txtRuta_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            ViewModel.CargarRodillos();
            config.RotulacionSettings.RutaRodillo = txtRuta.Text;
            Config.Config.SaveConfig(config);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void btnEliminarRodillo_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnModificarRodillo_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnGuardarRodillo_Click(object sender, RoutedEventArgs e)
    {

    }

    private void TipAddNuevoRodillo_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        TipAddNuevoRodillo.IsOpen = false;
    }

    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {

    }


}
