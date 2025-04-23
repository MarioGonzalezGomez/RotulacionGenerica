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
using Windows.Storage.Pickers;

namespace Generico_Front.Views;

public sealed partial class PremiosPage : Page
{

    Config.Config config;
    public PremiosViewModel ViewModel
    {
        get;
    }

    public PremiosPage()
    {
        ViewModel = App.GetService<PremiosViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarPremios();
        treePremios.ItemsSource = ViewModel.premios;
        txtRuta.Text = config.RotulacionSettings.RutaPremios;
    }

    //TREE VIEW
    private void treePremios_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        var selectedItem = args.AddedItems.FirstOrDefault();

        if (selectedItem is Premio premio)
        {
            // Acción para Premio
            MostrarDetallesPremio(premio);
        }
        else if (selectedItem is Nominado nominado)
        {
            // Acción para Nominado
            MostrarDetallesNominado(nominado);
        }
    }
    private void MostrarDetallesPremio(Premio premio)
    {
        var textBoxes = new[] { txtInfo1, txtInfo2, txtInfo3, txtInfo4, txtInfo5, txtInfo6 };
        foreach (var textBox in textBoxes)
        {
            textBox.Visibility = Visibility.Collapsed;
        }

        if (premio.entregadores.Count > 0)
        {
            headerInfo.Text = "Entrega";
            headerInfo.Visibility = Visibility.Visible;
            for (int i = 0; i < premio.entregadores.Count; i++)
            {
                textBoxes[i].Text = premio.entregadores[i];
                textBoxes[i].Visibility = Visibility.Visible;
            }
        }
        else
        {
            headerInfo.Visibility = Visibility.Collapsed;
        }
        headerInfo2.Text = "Ganador";
        if (premio.ganador != null)
        {
            txtRecoge.Text = premio.ganador.nombre;
            headerInfo2.Visibility = Visibility.Visible;
            txtRecoge.Visibility = Visibility.Visible;
        }
        else
        {
            headerInfo2.Visibility = Visibility.Collapsed;
            txtRecoge.Visibility = Visibility.Collapsed;
        }
    }
    private void MostrarDetallesNominado(Nominado nominado)
    {

        var textBoxes = new[] { txtInfo1, txtInfo2, txtInfo3, txtInfo4, txtInfo5, txtInfo6 };
        foreach (var textBox in textBoxes)
        {
            textBox.Visibility = Visibility.Collapsed;
        }

        headerInfo.Text = "Nominado por:";
        txtInfo1.Text = nominado.trabajo;
        headerInfo.Visibility = Visibility.Visible;
        txtInfo1.Visibility = Visibility.Visible;

        headerInfo2.Text = "Recoge";
        if (!string.IsNullOrEmpty(nominado.recoge))
        {
            txtRecoge.Text = nominado.recoge;
            headerInfo2.Visibility = Visibility.Visible;
            txtRecoge.Visibility = Visibility.Visible;
        }
        else
        {
            headerInfo2.Visibility = Visibility.Collapsed;
            txtRecoge.Visibility = Visibility.Collapsed;
        }
        txtGanador.Visibility = nominado.ganador ? Visibility.Visible : Visibility.Collapsed;
    }
    private void treePremios_DragOver(object sender, DragEventArgs e)
    {
        // Evita el reordenamiento de elementos dentro del TreeView
        e.AcceptedOperation = DataPackageOperation.None;
    }
    private void treePremios_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var menu = new MenuFlyout();

        // Opción para Desplegar Todos
        var desplegarTodos = new MenuFlyoutItem { Text = "Desplegar todos" };
        desplegarTodos.Click += DesplegarTodos_Click;
        menu.Items.Add(desplegarTodos);

        // Opción para Replegar Todos
        var replegarTodos = new MenuFlyoutItem { Text = "Replegar todos" };
        replegarTodos.Click += ReplegarTodos_Click;
        menu.Items.Add(replegarTodos);

        menu.ShowAt(treePremios, e.GetPosition(treePremios));
    }

    private void DesplegarTodos_Click(object sender, RoutedEventArgs e)
    {
        // Recorre todos los elementos principales del TreeView
        foreach (var item in treePremios.RootNodes)
        {
            ExpandAllItems(item);
        }
    }
    private void ExpandAllItems(TreeViewNode node)
    {
        node.IsExpanded = true; // Despliega el nodo actual

        foreach (var child in node.Children)
        {
            ExpandAllItems(child); // Llama recursivamente para expandir los hijos
        }
    }

    private void ReplegarTodos_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in treePremios.RootNodes)
        {
            CollapseAllItems(item);
        }
    }
    private void CollapseAllItems(TreeViewNode node)
    {
        node.IsExpanded = false;

        foreach (var child in node.Children)
        {
            CollapseAllItems(child);
        }
    }

    //FILTRADO
    private void FiltradoPorPremio_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allPremios
       .Where(c => c.nombre.IndexOf(FiltradoPorPremio.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPersona_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allPremios
       .Where(c => c.nominados.Any(n => n.nombre.IndexOf(FiltradoPorPersona.Text, StringComparison.OrdinalIgnoreCase) >= 0));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPelicula_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allPremios
      .Where(c => c.nominados.Any(n =>
           n.trabajo?.Contains(FiltradoPorPelicula.Text, StringComparison.OrdinalIgnoreCase) == true
        ));
        ViewModel.RemoverNoCoincidentes(filtrada);
        if (string.IsNullOrEmpty(FiltradoPorPelicula.Text))
            ViewModel.RecuperarLista(ViewModel.allPremios);
    }

    //EDICIÓN
    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Visibility.Visible;
            stckEditior1.Visibility = Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Visibility.Collapsed;
            stckEditior1.Visibility = Visibility.Collapsed;
        }
    }

    private async void filePicker_Click(object sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;

        txtRuta.Text = "";

        var openPicker = new FileOpenPicker();

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".txt");
        openPicker.FileTypeFilter.Add(".csv");

        var file = await abrirFilePicker(openPicker);
        if (file != null)
        {
            txtRuta.Text = file.Path;
            config.RotulacionSettings.RutaPremios = file.Path;
            PonerPremiosEnLista();
        }
        else
        {
            txtRuta.Text = config.RotulacionSettings.RutaPremios;
        }

        senderButton.IsEnabled = true;
    }
    private async Task<Windows.Storage.StorageFile> abrirFilePicker(FileOpenPicker openPicker)
    {
        var file = await openPicker.PickSingleFileAsync();
        return file;
    }
    private void PonerPremiosEnLista()
    {
        try
        {
            config.RotulacionSettings.RutaPremios = txtRuta.Text;
            Config.Config.SaveConfig(config);
            ViewModel.CargarPremios();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async void btnModificarPremio_Click(object sender, RoutedEventArgs e)
    {
        await AbrirFicheroYRefrescarUI(config.RotulacionSettings.RutaPremios);

    }
    public async Task AbrirFicheroYRefrescarUI(string rutaFichero)
    {
        if (!File.Exists(rutaFichero))
        {
            ShowDialog("El fichero no existe", $"No puede encontrarse el fichero de premios en la ruta {rutaFichero}");
            return;
        }

        try
        {
            var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{rutaFichero}\"",
                    UseShellExecute = true
                }
            };

            proceso.Start();
            await proceso.WaitForExitAsync();
            ViewModel.CargarPremios();
        }
        catch (Exception ex)
        {
            ShowDialog("Error al abrir el fichero", ex.Message);
        }
    }

    //METODOS DE LANZADO DE MENSAJES
    private async void AbrirTip()
    {
        Tip.IsOpen = true;
        await Task.Delay(1500);
        Tip.IsOpen = false;
    }
    private void Tip_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        Tip.IsOpen = false;
    }

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

    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {

    }


}
