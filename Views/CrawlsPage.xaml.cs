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

public sealed partial class CrawlsPage : Page
{
    public CrawlsViewModel ViewModel
    {
        get;
    }

    public CrawlsPage()
    {
        ViewModel = App.GetService<CrawlsViewModel>();
        InitializeComponent();
        IniciarListas();
    }

    private bool playLista = false;

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarCrawls();
        LVCrawls.ItemsSource = ViewModel.Crawls;
        LVCrawlsEmision.ItemsSource = ViewModel.CrawlsEmision;
    }


    //LISTA INDIVIDUAL
    private void LVCrawls_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        var ordenado = sender.Items.Cast<Crawl>().ToList();
        ViewModel.ActualizarPosiciones(ordenado);
    }

    private void LVCrawls_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LVCrawls.SelectedIndex != -1)
        {
            Crawl seleccionado = (Crawl)LVCrawls.SelectedItem;
            txtContenido.Text = seleccionado.linea.texto;
            txtorden.Text = seleccionado.posicion.ToString();
        }
    }

    private void LVCrawls_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var clickedItem = ((FrameworkElement)e.OriginalSource).DataContext;

        if (clickedItem != null)
        {
            LVCrawls.SelectedItem = clickedItem;
        }
    }

    private void LVCrawls_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        iconPlay.Glyph = "\uE768";
        listViewBorder.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        playLista = false;
    }

    private void LVCrawls_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {

        if (e.Items.Count > 0 && e.Items[0] is Crawl selected)
        {
            var jsonData = JsonSerializer.Serialize(selected);
            e.Data.SetData("StandardDataFormats.Text", jsonData);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
    }

    private void MenuFlyoutAddToList_Click(object sender, RoutedEventArgs e)
    {

        if (LVCrawls.SelectedItem != null)
        {
            var seleccionado = LVCrawls.SelectedItem as Crawl;
            if (!ViewModel.CrawlsEmision.Contains(seleccionado))
            {
                ViewModel.CrawlsEmision.Add(seleccionado);
            }
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
        if (LVCrawls.SelectedItem != null)
        {
            Crawl actual = LVCrawls.SelectedItem as Crawl;
            EliminarCrawl(actual);
            if (ViewModel.CrawlsEmision.Contains(actual))
            {
                ViewModel.CrawlsEmision.Remove(actual);
            }
        }
    }


    //LISTA EMISION
    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var clickedItem = ((FrameworkElement)e.OriginalSource).DataContext;
        if (clickedItem != null && clickedItem is Crawl selection)
        {
           // Crawl actual = LVCrawlsEmision.SelectedItem as Crawl;
            ViewModel.CrawlsEmision.Remove(selection);
        }
    }

    private void LVCrawlsEmision_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {

    }

    private void LVCrawlsEmision_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        iconPlay.Glyph = "\uEC57";
        listViewBorder.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Green);
        playLista = true;
    }

    private void LVCrawlsEmision_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }
    private async void LVCrawlsEmision_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.Text))
        {
            var deferral = e.GetDeferral();

            // Recupera el objeto arrastrado
            var jsonData = await e.DataView.GetTextAsync(StandardDataFormats.Text);
            var droppedCrawl = JsonSerializer.Deserialize<Crawl>(jsonData);

            if (droppedCrawl != null && !ViewModel.CrawlsEmision.Contains(droppedCrawl))
            {
                ViewModel.CrawlsEmision.Add(droppedCrawl);
            }
            deferral.Complete();
        }
    }

    //OPCIONES DE FILTRADO
    private void FiltradoPorTexto_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allCrawls.Where(r => r.linea.texto.Contains(FiltradoPorTexto.Text));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPosicion_TextChanged(object sender, TextChangedEventArgs e)
    {
        IEnumerable<Crawl> filtrada;
        if (int.TryParse(FiltradoPorPosicion.Text, out int value))
        {
            filtrada = ViewModel.allCrawls.Where(r => r.posicion >= value);
        }
        else
        {
            filtrada = ViewModel.allCrawls;
        }
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }


    //ACCIONES EN EDICIÓN
    private void BtnAddCrawl_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!tggEditor.IsOn) { tggEditor.IsOn = true; }
        if (ViewModel.Crawls.Count > 0)
        {
            var maxPosicion = ViewModel.Crawls.Max(r => r.posicion);
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
            stckEditior0.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            txtContenido.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            txtContenido.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    private void sliderCrawlVel_DragLeave(object sender, DragEventArgs e)
    {

    }

    private void chboxEsTitulo_Checked(object sender, RoutedEventArgs e)
    {

    }

    private void chboxEsTitulo_Unchecked(object sender, RoutedEventArgs e)
    {

    }

    private void btnEliminarCrawl_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (LVCrawls.SelectedItem != null)
        {
            Crawl actual = LVCrawls.SelectedItem as Crawl;
            EliminarCrawl(actual);
        }
    }
    private async void EliminarCrawl(Crawl aEliminar)
    {
        await ViewModel.EliminarCrawl(aEliminar);
    }

    private void btnModificarCrawl_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (LVCrawls.SelectedItem != null)
        {
            Crawl actual = LVCrawls.SelectedItem as Crawl;
            Crawl modificado = new Crawl();
            modificado.id = actual.id;
            modificado.posicion = int.Parse(txtorden.Text);
            // modificado.tipo = new Tipo();
            // modificado.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            // modificado.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;
            // modificado.tipo.numLineas = (cmbTiposEditor.SelectedValue as Tipo).numLineas;
            // modificado.lineas = actual.lineas;
            //
            // for (int i = 0; i < modificado.lineas.Count; i++)
            // {
            //     modificado.lineas[i].texto = textLineas[i].Text;
            // }
            ModificarCrawl(modificado);
        }
    }
    private async void ModificarCrawl(Crawl modificado)
    {
        await ViewModel.GuardarCrawl(modificado);
    }

    private void btnGuardarCrawl_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtContenido.Text))
        {
            Crawl nuevoCrawl = new Crawl();
            var maxPosicion = ViewModel.Crawls.Max(r => r.posicion);
            nuevoCrawl.id = 0;
            nuevoCrawl.posicion = maxPosicion + 1;
            Linea linea = new Linea();
            linea.id = 0;
            linea.texto = txtContenido.Text;
            nuevoCrawl.linea = linea;
            GuardarCrawlNuevo(nuevoCrawl);

            // TipGuardarAjustes.IsOpen = true;
        }
    }
    private async void GuardarCrawlNuevo(Crawl nuevo)
    {
        await ViewModel.GuardarCrawl(nuevo);
    }

    private void TipAddNuevoCrawl_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        TipAddNuevoCrawl.IsOpen = false;
    }

    // private async void btnDeleteList_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    // {
    //     var dialog = new ContentDialog
    //     {
    //         XamlRoot = this.XamlRoot,
    //         Title = "Confirmación de borrado",
    //         Content = "¿Estás seguro de que quieres vaciar la lista?",
    //         CloseButtonText = "Cancelar",
    //         PrimaryButtonText = "Confirmar",
    //         DefaultButton = ContentDialogButton.Close
    //     };
    //
    //     var result = await dialog.ShowAsync();
    //
    //     if (result.Equals(ContentDialogResult.Primary))
    //     {
    //         //TODO: VaciarLista();
    //     }
    // }

    private void TipGuardarAjustes_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        //   TipGuardarAjustes.IsOpen = false;
    }

    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {
        if (playLista) { } else { }
    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {

    }

}
