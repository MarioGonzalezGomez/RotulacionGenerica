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
        cmbAccionesConTipos.SelectedIndex = 0;
    }

    //OPCIONES DE FILTRADO
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

    //ACCIONES EN EDICIÓN
    private void BtnAddRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    //ACCIONES EN AJUSTES ADICIONALES
    private void AbrirPanelAjustes(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = true;
    }

    private void CerrarPanelAjustes(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = false;
    }

    private void cmbAccionesConTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        cmbTipos.ItemsSource = cmbAccionesConTipos.SelectedIndex == 0 ? null : ViewModel.Tipos;
        cmbTipos.IsEditable = cmbAccionesConTipos.SelectedIndex != 2;
    }
    private void cmbTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //TODO: Add logica de cargar datos del tipo seleccionado
        //No olvidar poner el num de lineas en la parte de edicion por linea
    }

    private void tggPorLineas_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (tggPorLineas.IsOn)
        {
            stckLineas.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEscala.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckPosX.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckPosY.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            stckLineas.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEscala.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckPosX.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckPosY.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    private async void btnDeleteList_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Confirmación de borrado",
            Content = "¿Estás seguro de que quieres vaciar la lista?",
            CloseButtonText = "Cancelar",
            PrimaryButtonText = "Confirmar",
            DefaultButton = ContentDialogButton.Close
        };

        var result = await dialog.ShowAsync();

        if (result.Equals(ContentDialogResult.Primary))
        {
            //TODO: VaciarLista();
        }
    }

    private void btnSaveAjustes_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TipGuardarAjustes.IsOpen = true;
        //TODO: Logica de comprobaciones antes de guardar
        //GuardarAjustes()
    }

    private void TipGuardarAjustes_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        TipGuardarAjustes.IsOpen = false;
    }
}
