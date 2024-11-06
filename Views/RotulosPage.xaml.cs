using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Contacts;

namespace Generico_Front.Views;

public sealed partial class RotulosPage : Page
{
    public RotulosViewModel ViewModel
    {
        get;
    }

    public RotulosPage()
    {
        ViewModel = App.GetService<RotulosViewModel>();
        InitializeComponent();
        LVRotulos.ItemsSource = ViewModel.Rotulos;
    }

    private void FiltradoPorNombre_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.Rotulos.Where(r => r.lineas[0].texto.Contains(FiltradoPorNombre.Text));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.Rotulos.Where(r => r.lineas[1].texto.Contains(FiltradoPorNombre.Text));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPosicion_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        var filtrada = ViewModel.Rotulos.Where(r => r.posicion > FiltradoPorPosicion.Value);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }


    private void BtnAddRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void AbrirPanelAjustes(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = true;
    }

    private void CerrarPanelAjustes(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = false;
    }
}
