using System.Text.Json;
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
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarRotulos();
        LVRotulos.ItemsSource = ViewModel.Rotulos;
        ViewModel.CargarTipos();
        cmbTiposEditor.ItemsSource = ViewModel.Tipos;
        cmbAccionesConTipos.SelectedIndex = 0;
    }

    private void LVRotulos_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        var ordenado = sender.Items.Cast<Rotulo>().ToList();
        ViewModel.ActualizarPosiciones(ordenado);
    }

    private void LVRotulos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Rotulo seleccionado = (Rotulo)LVRotulos.SelectedItem;
        if (tggEditor.IsOn)
        {
            var textBoxes = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };

            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i].Text = i < seleccionado.lineas.Count ? seleccionado.lineas[i].texto : string.Empty;
            }

            //TODO:Revisar este cambio
            cmbTiposEditor.SelectedIndex = ViewModel.Tipos.IndexOf(seleccionado.tipo);
            txtorden.Text = seleccionado.posicion.ToString();
        }
    }

    //OPCIONES DE FILTRADO
    private void FiltradoPorNombre_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allRotulos.Where(r => r.lineas[0].texto.Contains(FiltradoPorNombre.Text));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allRotulos.Where(r => r.ToString().Contains(FiltradoPorCargo.Text));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPosicion_TextChanged(object sender, TextChangedEventArgs e)
    {
        IEnumerable<Rotulo> filtrada;
        if (int.TryParse(FiltradoPorPosicion.Text, out int value))
        {
            filtrada = ViewModel.allRotulos.Where(r => r.posicion >= value);
        }
        else
        {
            filtrada = ViewModel.allRotulos;
        }
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    //ACCIONES EN EDICIÓN
    private void BtnAddRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void tggEditor_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            linkAjustes.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior3.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            linkAjustes.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior3.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    private void cmbTiposEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTiposEditor.SelectedIndex != -1)
        {
            Tipo seleccionado = (Tipo)cmbTiposEditor.SelectedValue;
            switch (seleccionado.numLineas)
            {
                case 1:
                    txtLinea1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea2.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    txtLinea3.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    txtLinea4.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    break;
                case 2:
                    txtLinea1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea3.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    txtLinea4.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    break;
                case 3:
                    txtLinea1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea3.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea4.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    break;
                case 4:
                    txtLinea1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea3.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    txtLinea4.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                    break;
                default: break;
            }
        }
    }


    private void btnEliminarRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //TODO: Accion de eliminar el rótulo
    }

    private void btnModificarRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //TODO: Accion de modificar el rótulo
    }

    private void btnGuardarRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //TODO: Accion de añadir nuevo rótulo
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
