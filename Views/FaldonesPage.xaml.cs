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

    private Faldon seleccionado = null;
    private bool faldonIn = false;

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
            seleccionado = (Faldon)LVFaldones.SelectedItem;

            Tipo tipo = ViewModel.Tipos.FirstOrDefault(t => t.id == seleccionado.tipo.id);
            cmbTiposEditor.SelectedIndex = ViewModel.Tipos.IndexOf(tipo);

            txtTitulo.Text = seleccionado.titulo.texto;
            txtCuerpo.Text = seleccionado.texto.texto;
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

    private void MenuFlyoutEditar_Click(object sender, RoutedEventArgs e)
    {
        if (!tggEditor.IsOn)
        {
            tggEditor.IsOn = true;
        }
    }

    private void MenuFlyoutBorrar_Click(object sender, RoutedEventArgs e)
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
        var filtrada = ViewModel.allFaldones
    .Where(r => r.titulo.texto.IndexOf(FiltradoPorNombre.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allFaldones
    .Where(r => r.texto.texto.IndexOf(FiltradoPorCargo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPosicion_TextChanged(object sender, TextChangedEventArgs e)
    {
        IEnumerable<Faldon> filtrada;
        if (int.TryParse(FiltradoPorPosicion.Text, out int value))
        {
            filtrada = ViewModel.allFaldones.Where(r => r.posicion >= value);
        }
        else
        {
            filtrada = ViewModel.allFaldones;
        }
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
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
        Tip.Target = btnEliminarFaldon;
        Tip.Title = "Faldón eliminado";
        await ViewModel.EliminarFaldon(aEliminar);
        AbrirTip();
    }

    private void btnModificarFaldon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (LVFaldones.SelectedItem != null)
        {
            Faldon actual = LVFaldones.SelectedItem as Faldon;
            Faldon modificado = new Faldon();
            modificado.id = actual.id;
            modificado.posicion = int.Parse(txtorden.Text);
            modificado.tipo = new Tipo();
            modificado.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            modificado.tipo.seAplicaA = (cmbTiposEditor.SelectedValue as Tipo).seAplicaA;
            modificado.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;
            modificado.tipo.numLineas = (cmbTiposEditor.SelectedValue as Tipo).numLineas;

            modificado.titulo = actual.titulo;
            modificado.titulo.texto = txtTitulo.Text;
            modificado.texto = actual.texto;
            modificado.texto.texto = txtCuerpo.Text;

            ModificarFaldon(modificado);
        }
    }
    private async void ModificarFaldon(Faldon modificado)
    {
        Tip.Target = btnModificarFaldon;
        Tip.Title = "Modificado con éxito";
        await ViewModel.GuardarFaldon(modificado);
        AbrirTip();
    }

    private void btnGuardarFaldon_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtCuerpo.Text))
        {
            Faldon nuevoFaldon = new Faldon();
            var maxPosicion = ViewModel.Faldones.Count > 0 ? ViewModel.Faldones.Max(r => r.posicion) : 0;
            nuevoFaldon.id = 0;
            nuevoFaldon.posicion = maxPosicion + 1;
            nuevoFaldon.tipo = new Tipo();
            nuevoFaldon.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            nuevoFaldon.tipo.seAplicaA = (cmbTiposEditor.SelectedValue as Tipo).seAplicaA;
            nuevoFaldon.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;

            Linea titulo = new Linea();
            titulo.id = 0;
            titulo.texto = txtTitulo.Text;
            nuevoFaldon.titulo = titulo;

            Linea cuerpo = new Linea();
            cuerpo.id = 0;
            cuerpo.texto = txtCuerpo.Text;
            nuevoFaldon.texto = cuerpo;

            GuardarFaldonNuevo(nuevoFaldon);
        }
    }
    private async void GuardarFaldonNuevo(Faldon nuevo)
    {
        Tip.Target = btnGuardarFaldon;
        Tip.Title = "Guardado con éxito";
        await ViewModel.GuardarFaldon(nuevo);
        AbrirTip();
    }

    private async void AbrirTip()
    {
        Tip.IsOpen = true;
        await Task.Delay(1500);
        Tip.IsOpen = false;
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
        //TODO: Logica de comprobaciones antes de guardar
        //GuardarAjustes()
    }

    //Acciones Graficas
    private void btnEntra_Click(object sender, RoutedEventArgs e)
    {
        if (seleccionado != null)
        {
            ViewModel.Entra(seleccionado);
            faldonIn = true;
        }
    }

    private void btnSale_Click(object sender, RoutedEventArgs e)
    {
        if (faldonIn)
        {
            ViewModel.Sale();
            faldonIn = false;
        }
    }
}
