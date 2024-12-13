using System.Diagnostics;
using System.Text.Json;
using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI.Xaml;
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
        cmbAccionesConTipos.ItemsSource = ViewModel.Tipos;
        cmbAccionesConTipos.SelectedIndex = 0;
    }

    private void LVRotulos_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        var ordenado = sender.Items.Cast<Rotulo>().ToList();
        ViewModel.ActualizarPosiciones(ordenado);
    }

    private void LVRotulos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LVRotulos.SelectedIndex != -1)
        {
            Rotulo seleccionado = (Rotulo)LVRotulos.SelectedItem;
            var textBoxes = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };

            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i].Text = i < seleccionado.lineas.Count ? seleccionado.lineas[i].texto : string.Empty;
            }

            Tipo tipo = ViewModel.Tipos.FirstOrDefault(t => t.id == seleccionado.tipo.id);
            cmbTiposEditor.SelectedIndex = ViewModel.Tipos.IndexOf(tipo);
            txtorden.Text = seleccionado.posicion.ToString();
        }
    }

    private void LVRotulos_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var clickedItem = ((FrameworkElement)e.OriginalSource).DataContext;

        if (clickedItem != null)
        {
            LVRotulos.SelectedItem = clickedItem;
        }
    }

    private void MenuFlyoutEditar_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!tggEditor.IsOn)
        {
            tggEditor.IsOn = true;
        }
    }

    private void MenuFlyoutBorrar_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (LVRotulos.SelectedItem != null)
        {
            Rotulo actual = LVRotulos.SelectedItem as Rotulo;
            EliminarRotulo(actual);
        }
    }

    //OPCIONES DE FILTRADO
    private void FiltradoPorNombre_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allRotulos
    .Where(r => r.lineas[0].texto.IndexOf(FiltradoPorNombre.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allRotulos
    .Where(r => r.ToString().IndexOf(FiltradoPorCargo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
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
        if (!tggEditor.IsOn) { tggEditor.IsOn = true; }
        if (ViewModel.Rotulos.Count > 0)
        {
            var maxPosicion = ViewModel.Rotulos.Max(r => r.posicion);
            txtorden.Text = $"{maxPosicion + 1}";
        }
        else
        {
            txtorden.Text = "1";
        }
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
        if (LVRotulos.SelectedItem != null)
        {
            Rotulo actual = LVRotulos.SelectedItem as Rotulo;
            EliminarRotulo(actual);
        }
    }
    private async void EliminarRotulo(Rotulo aEliminar)
    {
        await ViewModel.EliminarRotulo(aEliminar);
    }

    private void btnModificarRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (LVRotulos.SelectedItem != null)
        {
            Rotulo actual = LVRotulos.SelectedItem as Rotulo;
            Rotulo modificado = new Rotulo();
            var textLineas = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };
            modificado.id = actual.id;
            modificado.posicion = int.Parse(txtorden.Text);
            modificado.tipo = new Tipo();
            modificado.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            modificado.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;
            modificado.tipo.numLineas = (cmbTiposEditor.SelectedValue as Tipo).numLineas;
            modificado.lineas = actual.lineas;

            for (int i = 0; i < modificado.lineas.Count; i++)
            {
                modificado.lineas[i].texto = textLineas[i].Text;
            }
            ModificarRotulo(modificado);
        }
    }
    private async void ModificarRotulo(Rotulo modificado)
    {
        await ViewModel.GuardarRotulo(modificado);
    }

    private void btnGuardarRotulo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLinea1.Text))
        {
            Rotulo nuevoRotulo = new Rotulo();
            var textLineas = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };
            var maxPosicion = ViewModel.Rotulos.Max(r => r.posicion);
            List<Linea> lineas = new List<Linea>();
            nuevoRotulo.id = 0;
            nuevoRotulo.posicion = maxPosicion + 1;
            nuevoRotulo.tipo = new Tipo();
            nuevoRotulo.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            nuevoRotulo.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;

            for (int i = 0; i < textLineas.Length; i++)
            {
                if (!string.IsNullOrEmpty(textLineas[i].Text))
                {
                    Linea linea = new Linea();
                    linea.id = 0;
                    linea.texto = textLineas[i].Text;
                    lineas.Add(linea);
                }
            }
            nuevoRotulo.lineas = lineas;
            GuardarRotuloNuevo(nuevoRotulo);
            TipGuardarAjustes.IsOpen = true;
        }
    }
    private async void GuardarRotuloNuevo(Rotulo nuevo)
    {
        await ViewModel.GuardarRotulo(nuevo);
    }

    private void TipAddNuevoRotulo_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        //TODO: Solucionar que salga donde debe este cartel
        TipGuardarAjustes.IsOpen = false;
    }


    //ACCIONES EN AJUSTES ADICIONALES
    //TODO: Ajustes adicionales. Hacer si se va a utilizar.
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

    // private void tggPorLineas_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    // {
    //     if (tggPorLineas.IsOn)
    //     {
    //         stckLineas.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
    //         stckEscala.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
    //         stckPosX.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
    //         stckPosY.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
    //     }
    //     else
    //     {
    //         stckLineas.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    //         stckEscala.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    //         stckPosX.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    //         stckPosY.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    //     }
    // }

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
