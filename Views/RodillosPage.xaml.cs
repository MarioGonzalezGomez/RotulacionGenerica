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
        cmbTipos.SelectedItem = ViewModel.Tipos.FirstOrDefault(t => t.descripcion.Equals(config.RotulacionSettings.TipoRodillo));
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

    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    //EDICIÓN
    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Visibility.Visible;
            cmbTipos.Visibility = Visibility.Visible;
            stckVelocidad.Visibility = Visibility.Visible;
            btnModificar.Visibility = Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Visibility.Collapsed;
            cmbTipos.Visibility = Visibility.Collapsed;
            stckVelocidad.Visibility = Visibility.Collapsed;
            btnModificar.Visibility = Visibility.Collapsed;
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

    private void btnModificar_Click(object sender, RoutedEventArgs e)
    {

    }

    private void slideVelocidad_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {

    }

    private void Tip_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        Tip.IsOpen = false;
    }

    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {

    }

   
}
