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
        config = Config.Config.GetInstance();
        InitializeComponent();
        IniciarListas();
    }

    private bool playLista = false;
    Crawl seleccionado = null;

    Config.Config config;

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
            seleccionado = (Crawl)LVCrawls.SelectedItem;
            txtContenido.Text = seleccionado.linea.texto;
            txtorden.Text = seleccionado.posicion.ToString();
            chboxEsTitulo.IsChecked = seleccionado.esTitulo;
            sliderCrawlVel.Value = seleccionado.velocidad;
            if (playLista)
            {
                iconPlay.Glyph = "\uE768";
                listViewBorder.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
                playLista = false;
            }
            if (tggEditor.IsOn)
            {
                stckEditior1.Visibility = Visibility.Visible;
                txtContenido.Visibility = Visibility.Visible;
                btnEliminarCrawl.Visibility = Visibility.Visible;
                btnGuardarCrawl.Visibility = Visibility.Visible;
            }
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

        if (tggEditor.IsOn)
        {
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            txtContenido.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            btnEliminarCrawl.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            btnGuardarCrawl.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
    }

    private void LVCrawls_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {

        if (e.Items.Count > 0 && e.Items[0] is Crawl selected)
        {
            var jsonData = JsonSerializer.Serialize(selected);
            e.Data.SetData(StandardDataFormats.Text, jsonData);
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

        //Cambiar zona de edicion para mostrar velocidad de la lista de emision
        sliderCrawlVel.Value = config.RotulacionSettings.VelocidadCrawl;
        if (tggEditor.IsOn)
        {
            stckEditior1.Visibility = Visibility.Collapsed;
            txtContenido.Visibility = Visibility.Collapsed;
            btnEliminarCrawl.Visibility = Visibility.Collapsed;
            btnGuardarCrawl.Visibility = Visibility.Collapsed;
        }
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

            //Compruebo que el elemento no está ya en la lista, para evitar duplicidades
            if (droppedCrawl != null && !ViewModel.CrawlsEmision.Any(c => c.id == droppedCrawl.id))
            {
                ViewModel.CrawlsEmision.Add(droppedCrawl);
            }
            deferral.Complete();
        }
    }

    private async void BtnClearEmision_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CrawlsEmision.Count > 0)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Confirmación de vaciado",
                Content = "¿Estás seguro de que quieres vaciar la lista de reproducción de crawls?",
                CloseButtonText = "Cancelar",
                PrimaryButtonText = "Confirmar",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await dialog.ShowAsync();

            if (result.Equals(ContentDialogResult.Primary))
            {
                ViewModel.CrawlsEmision.Clear();
            }
        }
    }

    //OPCIONES DE FILTRADO
    private void FiltradoPorTexto_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allCrawls
   .Where(r => r.linea.texto.IndexOf(FiltradoPorTexto.Text, StringComparison.OrdinalIgnoreCase) >= 0);
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
    private void BtnAddCrawl_Click(object sender, RoutedEventArgs e)
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

    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            if (!playLista)
            {
                stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                txtContenido.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
            else
            {
                btnEliminarCrawl.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                btnGuardarCrawl.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }
        else
        {
            stckEditior0.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            txtContenido.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }



    private void btnEliminarCrawl_Click(object sender, RoutedEventArgs e)
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

    private void btnModificarCrawl_Click(object sender, RoutedEventArgs e)
    {
        if (playLista)
        {
            config.RotulacionSettings.VelocidadCrawl = (int)sliderCrawlVel.Value;
            Config.Config.SaveConfig(config);
        }
        else if (LVCrawls.SelectedItem != null)
        {
            Crawl actual = LVCrawls.SelectedItem as Crawl;
            Crawl modificado = new Crawl();
            modificado.id = actual.id;
            modificado.posicion = int.Parse(txtorden.Text);
            modificado.esTitulo = chboxEsTitulo.IsChecked.Value;
            modificado.velocidad = (int)sliderCrawlVel.Value;
            actual.linea.texto = txtContenido.Text;
            modificado.linea = actual.linea;

            ModificarCrawl(modificado);
        }
    }
    private async void ModificarCrawl(Crawl modificado)
    {
        await ViewModel.GuardarCrawl(modificado);
    }

    private void btnGuardarCrawl_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtContenido.Text))
        {
            Crawl nuevoCrawl = new Crawl();
            var maxPosicion = ViewModel.Crawls.Max(r => r.posicion);
            nuevoCrawl.id = 0;
            nuevoCrawl.posicion = maxPosicion + 1;
            nuevoCrawl.esTitulo = chboxEsTitulo.IsChecked.Value;
            nuevoCrawl.velocidad = (int)sliderCrawlVel.Value;
            Linea linea = new Linea();
            linea.id = 0;
            linea.texto = txtContenido.Text;
            nuevoCrawl.linea = linea;
            GuardarCrawlNuevo(nuevoCrawl);

            TipAddNuevoCrawl.IsOpen = true;
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

    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {
        if (playLista && LVCrawlsEmision.Items.Count > 0)
        {
            //Sacaremos la velocidad de config
        }
        else
        {
            //Sacaremos la velocidad de  seleccionado.velocidad
            if (seleccionado != null)
            {

            }
        }
    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {

    }


}
