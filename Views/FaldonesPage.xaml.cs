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
    private static bool faldonIn = false;

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarFaldones();
        LVFaldones.ItemsSource = ViewModel.Faldones;
        ViewModel.CargarTipos();
        cmbTipos.ItemsSource = ViewModel.Tipos;
        cmbTiposEditor.ItemsSource = ViewModel.Tipos;
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

            if (seleccionado.titulo != null)
            {
                txtTitulo.Text = seleccionado.titulo.texto;
            }
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
    private void tggFiltrado_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggFiltrado.IsOn)
        {
            FiltradoPorNombre.Visibility = Visibility.Visible;
            FiltradoPorPosicion.Visibility = Visibility.Visible;
            FiltradoPorCargo.Visibility = Visibility.Visible;
        }
        else
        {
            FiltradoPorNombre.Visibility = Visibility.Collapsed;
            FiltradoPorPosicion.Visibility = Visibility.Collapsed;
            FiltradoPorCargo.Visibility = Visibility.Collapsed;
        }
    }

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

    //  private void BtnAddFaldon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    //  {
    //      if (!tggEditor.IsOn) { tggEditor.IsOn = true; }
    //      if (ViewModel.Faldones.Count > 0)
    //      {
    //          var maxPosicion = ViewModel.Faldones.Max(r => r.posicion);
    //          txtorden.Text = $"{maxPosicion + 1}";
    //      }
    //      else
    //      {
    //          txtorden.Text = "1";
    //      }
    //  }

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
                    txtTitulo.Visibility = Visibility.Collapsed;
                    txtCuerpo.Visibility = Visibility.Visible;
                    break;
                case 2:
                    txtTitulo.Visibility = Visibility.Visible;
                    txtCuerpo.Visibility = Visibility.Visible;
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
            if (modificado.titulo != null)
            {
                modificado.titulo.texto = txtTitulo.Text;
            }
            modificado.texto = actual.texto;
            modificado.texto.texto = txtCuerpo.Text;

            ModificarFaldon(modificado);

            if (string.Equals(actual.tipo.descripcion, modificado.tipo.descripcion))
            {
                ModificarFaldon(modificado);
            }
            else
            {
                EliminarFaldon(actual);
                modificado.id = 0;
                GuardarFaldonNuevo(modificado);
            }
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

            if (txtTitulo.Visibility == Visibility.Visible && !string.IsNullOrEmpty(txtTitulo.Text))
            {
                Linea titulo = new Linea();
                titulo.id = 0;
                titulo.texto = txtTitulo.Text;
                nuevoFaldon.titulo = titulo;
            }

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

    private void AbrirPanelAjustes(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = true;
    }

    private void CerrarPanelAjustes(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = false;
    }

    private void cmbTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTipos.SelectedIndex != -1)
        {
            Tipo tipoSeleccionado = (Tipo)cmbTipos.SelectedItem;
            txtNombreTipo.Text = tipoSeleccionado.descripcion;
            cmbNumLineas.SelectedIndex = tipoSeleccionado.numLineas - 1;
            txtNombreTipo.Visibility = Visibility.Visible;
            stackNumlineas.Visibility = Visibility.Visible;
        }
        else
        {
            txtNombreTipo.Visibility = Visibility.Collapsed;
            stackNumlineas.Visibility = Visibility.Collapsed;
        }
    }

    private async void btnDeleteAjustes_Click(object sender, RoutedEventArgs e)
    {
        if (cmbTipos.SelectedItem != null)
        {
            Tipo selected = cmbTipos.SelectedItem as Tipo;
            await ViewModel.EliminarTipo(selected);
            Tip.Target = btnDeleteAjustes;
            Tip.Title = "Tipo eliminado";
            AbrirTip();
        }
        else
        {
            ShowDialog("Seleccionar tipo", "No se ha seleccionado ningún tipo para ser eliminado");
        }
    }

    private async void btnEditAjustes_Click(object sender, RoutedEventArgs e)
    {
        string nuevoNombre = txtNombreTipo.Text?.Trim();
        if (cmbTipos.SelectedItem != null && !string.IsNullOrEmpty(nuevoNombre))
        {
            Tipo seleccionado = cmbTipos.SelectedItem as Tipo;
            seleccionado.descripcion = nuevoNombre;
            seleccionado.numLineas = cmbNumLineas.SelectedItem != null ? cmbNumLineas.SelectedIndex + 1 : seleccionado.numLineas;
            await ViewModel.GuardarTipo(seleccionado);
            Tip.Target = btnEditAjustes;
            Tip.Title = "Editado con éxito";
            AbrirTip();
        }
        else
        {
            ShowDialog("Seleccionar tipo", "No se ha seleccionado ningún tipo para ser editado");
        }
    }

    private async void btnSaveAjustes_Click(object sender, RoutedEventArgs e)
    {
        string nuevoNombre = txtNombreTipo.Text?.Trim();

        if (!string.IsNullOrEmpty(nuevoNombre))
        {
            if (cmbTipos.SelectedItem != null)
            {
                Tipo seleccionado = cmbTipos.SelectedItem as Tipo;
                if (string.Equals(seleccionado.descripcion, nuevoNombre, StringComparison.OrdinalIgnoreCase))
                {
                    ShowDialog("Cambio de nombre", "No se puede guardar un nuevo tipo con el mismo nombre que uno ya registrado, por favor, edite el nombre actual si quiere guardarlo como un nuevo tipo de Faldón.");
                    return;
                }
            }
            Tipo nuevoTipo = new Tipo();
            nuevoTipo.id = 0;
            nuevoTipo.seAplicaA = "Faldones";
            nuevoTipo.descripcion = nuevoNombre;
            if (cmbNumLineas.SelectedItem != null)
            {
                nuevoTipo.numLineas = cmbNumLineas.SelectedIndex + 1;
            }
            else
            {
                ShowDialog("Falta número de líneas", "No se ha especificado el número de líneas que tendrá este nuevo Tipo. Por favor, rellene ese campo");
                return;
            }


            await ViewModel.GuardarTipo(nuevoTipo);
            Tip.Target = btnSaveAjustes;
            Tip.Title = "Creado con éxito";
            AbrirTip();
        }
        else
        {
            ShowDialog("Seleccionar tipo", "No se ha seleccionado ningún tipo para ser editado");
            return;
        }
    }

    private async void btnDeleteList_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Confirmación de borrado",
            Content = "¿Estás seguro de que quieres vaciar la lista?\nEsto eliminará de la base de datos todos los faldones actuales",
            CloseButtonText = "Cancelar",
            PrimaryButtonText = "Confirmar",
            DefaultButton = ContentDialogButton.Close
        };

        var result = await dialog.ShowAsync();

        if (result.Equals(ContentDialogResult.Primary))
        {
            //TODO
            // await ViewModel.BorrarTodos();
        }
    }

    //ENVIAR MENSAJES UI
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

    //ACCIONES GRAFICAS
    private void btnEntra_Click(object sender, RoutedEventArgs e)
    {
        if (seleccionado != null)
        {
            if (!faldonIn)
            {
                ViewModel.Entra(seleccionado);
                faldonIn = true;
            }
            else
            {
                ViewModel.Encadena(seleccionado);
            }

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
