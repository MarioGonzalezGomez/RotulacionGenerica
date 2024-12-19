using System.Diagnostics;
using System.Text.Json;
using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Contacts;

namespace Generico_Front.Views;

public sealed partial class FaldonesPage : Page
{
    public FaldonesViewModel ViewModel
    {
        get;
    }

    public FaldonesPage()
    {
        ViewModel = App.GetService<FaldonesViewModel>();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarFaldones();
        LVFaldones.ItemsSource = ViewModel.Faldones;
        ViewModel.CargarTipos();
        cmbTiposEditor.ItemsSource = ViewModel.Tipos;
        cmbAccionesConTipos.ItemsSource = ViewModel.Tipos;
        cmbAccionesConTipos.SelectedIndex = 0;
    }

    private void LVFaldones_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        var ordenado = sender.Items.Cast<Faldon>().ToList();
        ViewModel.ActualizarPosiciones(ordenado);
    }

    private void LVFaldones_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LVFaldones.SelectedIndex != -1)
        {
            Faldon seleccionado = (Faldon)LVFaldones.SelectedItem;
            var textBoxes = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };

            for (int i = 0; i < textBoxes.Length; i++)
            {
             //   textBoxes[i].Text = i < seleccionado.lineas.Count ? seleccionado.lineas[i].texto : string.Empty;
            }

            Tipo tipo = ViewModel.Tipos.FirstOrDefault(t => t.id == seleccionado.tipo.id);
            cmbTiposEditor.SelectedIndex = ViewModel.Tipos.IndexOf(tipo);
            txtorden.Text = seleccionado.posicion.ToString();
        }
    }

    private void LVFaldones_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var clickedItem = ((FrameworkElement)e.OriginalSource).DataContext;

        if (clickedItem != null)
        {
            LVFaldones.SelectedItem = clickedItem;
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
        if (LVFaldones.SelectedItem != null)
        {
            Faldon actual = LVFaldones.SelectedItem as Faldon;
            EliminarFaldon(actual);
        }
    }

    //OPCIONES DE FILTRADO
    private void FiltradoPorNombre_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltradoPorPosicion_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    //ACCIONES EN EDICIÓN
    private void BtnAddFaldon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!tggEditor.IsOn) { tggEditor.IsOn = true; }
        if (ViewModel.Faldones.Count > 0)
        {
            var maxPosicion = ViewModel.Faldones.Max(r => r.posicion);
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


    private void btnEliminarFaldon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (LVFaldones.SelectedItem != null)
        {
            Faldon actual = LVFaldones.SelectedItem as Faldon;
            EliminarFaldon(actual);
        }
    }
    private async void EliminarFaldon(Faldon aEliminar)
    {
        await ViewModel.EliminarFaldon(aEliminar);
    }

    private void btnModificarFaldon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (LVFaldones.SelectedItem != null)
        {
            Faldon actual = LVFaldones.SelectedItem as Faldon;
            Faldon modificado = new Faldon();
            var textLineas = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };
            modificado.id = actual.id;
            modificado.posicion = int.Parse(txtorden.Text);
            modificado.tipo = new Tipo();
            modificado.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            modificado.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;
            modificado.tipo.numLineas = (cmbTiposEditor.SelectedValue as Tipo).numLineas;
            //modificado.lineas = actual.lineas;

           // for (int i = 0; i < modificado.lineas.Count; i++)
           // {
           //     modificado.lineas[i].texto = textLineas[i].Text;
           // }
            ModificarFaldon(modificado);
        }
    }
    private async void ModificarFaldon(Faldon modificado)
    {
        await ViewModel.GuardarFaldon(modificado);
    }

    private void btnGuardarFaldon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLinea1.Text))
        {
            Faldon nuevoFaldon = new Faldon();
            var textLineas = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };
            var maxPosicion = ViewModel.Faldones.Max(r => r.posicion);
            List<Linea> lineas = new List<Linea>();
            nuevoFaldon.id = 0;
            nuevoFaldon.posicion = maxPosicion + 1;
            nuevoFaldon.tipo = new Tipo();
            nuevoFaldon.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            nuevoFaldon.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;

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
          //  nuevoFaldon.lineas = lineas;
            GuardarFaldonNuevo(nuevoFaldon);
            TipAddNuevoFaldon.IsOpen = true;
        }
    }
    private async void GuardarFaldonNuevo(Faldon nuevo)
    {
        await ViewModel.GuardarFaldon(nuevo);
    }

    private void TipAddNuevoFaldon_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
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
