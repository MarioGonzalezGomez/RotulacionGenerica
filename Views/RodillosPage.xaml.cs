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

namespace Generico_Front.Views;

public sealed partial class RodillosPage : Page
{
    public List<Cargo> cargos
    {
        get; set;
    }
    public RodillosViewModel ViewModel
    {
        get;
    }

    public RodillosPage()
    {
        ViewModel = App.GetService<RodillosViewModel>();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarRodillos();
        cargos = ViewModel.allCargos;

    }

    private void FiltradoPorTexto_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltradoPorPosicion_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
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
