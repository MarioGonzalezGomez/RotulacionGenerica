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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Generico_Front.Views;

public sealed partial class RodillosPage : Page
{
    Config.Config config;
    public RodillosViewModel ViewModel
    {
        get;
    }

    public RodillosPage()
    {
        ViewModel = App.GetService<RodillosViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarRodillos();
        ViewModel.CargarTipos();
        txtRuta.Text = config.RotulacionSettings.RutaRodillo;
        treeRodillo.ItemsSource = ViewModel.cargos;
        cmbTipos.ItemsSource = ViewModel.Tipos;
        cmbTiposEdicion.ItemsSource = ViewModel.Tipos;
        cmbTipos.SelectedItem = ViewModel.Tipos.FirstOrDefault(t => t.descripcion.Equals(config.RotulacionSettings.TipoRodillo));
        slideVelocidad.Value = config.RotulacionSettings.VelocidadRodillo;
    }

    private void treeRodillo_DragOver(object sender, DragEventArgs e)
    {
        // Evita el reordenamiento de elementos dentro del TreeView
        e.AcceptedOperation = DataPackageOperation.None;
    }
    private void treeRodillo_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
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

        menu.ShowAt(treeRodillo, e.GetPosition(treeRodillo));
    }

    private void DesplegarTodos_Click(object sender, RoutedEventArgs e)
    {
        // Recorre todos los elementos principales del TreeView
        foreach (var item in treeRodillo.RootNodes)
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
        foreach (var item in treeRodillo.RootNodes)
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
    private void FiltradoPorPersona_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allCargos
        .Where(c => c.personas.Any(p => p.nombre.IndexOf(FiltradoPorPersona.Text, StringComparison.OrdinalIgnoreCase) >= 0));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allCargos
        .Where(c => c.nombre.IndexOf(FiltradoPorCargo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    //EDICIÓN
    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Visibility.Visible;
            linkAjustes.Visibility = Visibility.Visible;
            cmbTipos.Visibility = Visibility.Visible;
            stckVelocidad.Visibility = Visibility.Visible;
            stckBotonera.Visibility = Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Visibility.Collapsed;
            linkAjustes.Visibility = Visibility.Collapsed;
            cmbTipos.Visibility = Visibility.Collapsed;
            stckVelocidad.Visibility = Visibility.Collapsed;
            stckBotonera.Visibility = Visibility.Collapsed;
        }
    }

    private void AbrirPanelAjustes(object sender, RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = true;
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
            config.RotulacionSettings.RutaRodillo = file.Path;
            PonerRodilloEnLista();
        }
        else
        {
            txtRuta.Text = config.RotulacionSettings.RutaRodillo;
        }

        senderButton.IsEnabled = true;
    }

    private async Task<Windows.Storage.StorageFile> abrirFilePicker(FileOpenPicker openPicker)
    {
        var file = await openPicker.PickSingleFileAsync();
        return file;
    }

    private void PonerRodilloEnLista()
    {
        try
        {
            config.RotulacionSettings.RutaRodillo = txtRuta.Text;
            Config.Config.SaveConfig(config);
            ViewModel.CargarRodillos();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void cmbTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTipos.SelectedIndex != -1)
        {
            Tipo seleccionado = (Tipo)cmbTipos.SelectedValue;
            config.RotulacionSettings.TipoRodillo = seleccionado.descripcion;
        }
    }

    private async void btnModificar_Click(object sender, RoutedEventArgs e)
    {
        await AbrirFicheroYRefrescarUI(config.RotulacionSettings.RutaRodillo);

    }
    public async Task AbrirFicheroYRefrescarUI(string rutaFichero)
    {
        if (!File.Exists(rutaFichero))
        {
            ShowDialog("El fichero no existe", $"No puede encontrarse el fichero de créditos en la ruta {rutaFichero}");
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
            ViewModel.CargarRodillos();
        }
        catch (Exception ex)
        {
            ShowDialog("Error al abrir el fichero", ex.Message);
        }
    }

    private void btnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (cmbTipos.SelectedItem != null)
        {
            Tipo seleccionado = (Tipo)cmbTipos.SelectedValue;
            config.RotulacionSettings.TipoRodillo = seleccionado.descripcion;
        }
        config.RotulacionSettings.VelocidadRodillo = (int)slideVelocidad.Value;

        Config.Config.SaveConfig(config);

        Tip.Target = btnGuardar;
        Tip.Title = "Guardado con éxito";
        AbrirTip();
    }

    //PANEL DESPLEGABLE AJUSTES EXTRA
    private void CerrarPanelAjustes(object sender, RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = false;
    }

    private void cmbTiposEdicion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTiposEdicion.SelectedItem != null)
        {
            Tipo seleccionado = (Tipo)cmbTiposEdicion.SelectedValue;
            txtNombreTipo.Text = seleccionado.descripcion;
        }
    }

    private async void btnDeleteAjustes_Click(object sender, RoutedEventArgs e)
    {
        if (cmbTiposEdicion.SelectedItem != null)
        {
            Tipo seleccionado = cmbTiposEdicion.SelectedItem as Tipo;
            await ViewModel.EliminarTipo(seleccionado);
            Tip.Target = btnDeleteAjustes;
            Tip.Title = "Eliminado con éxito";
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
        if (cmbTiposEdicion.SelectedItem != null && !string.IsNullOrEmpty(nuevoNombre))
        {
            Tipo seleccionado = cmbTiposEdicion.SelectedItem as Tipo;
            seleccionado.descripcion = nuevoNombre;
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
            if (cmbTiposEdicion.SelectedItem != null)
            {
                Tipo seleccionado = cmbTiposEdicion.SelectedItem as Tipo;
                if (string.Equals(seleccionado.descripcion, nuevoNombre, StringComparison.OrdinalIgnoreCase))
                {
                    ShowDialog("Cambio de nombre", "No se puede guardar un nuevo tipo con el mismo nombre que uno ya registrado, por favor, edite el nombre actual si quiere guardarlo como un nuevo tipo de rodillo.");
                    return;
                }
            }
            Tipo nuevoTipo = new Tipo();
            nuevoTipo.id = 0;
            nuevoTipo.seAplicaA = "Rodillos";
            nuevoTipo.descripcion = nuevoNombre;

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
